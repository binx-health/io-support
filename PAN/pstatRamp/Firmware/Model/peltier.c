/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/
/**
\file peltier.c
\brief peltier logic control
*/
//#define PELTIER_DEBUG
#ifdef PELTIER_DEBUG
#include "Uart1.h"
#endif
#include <string.h>
#include <stdio.h>
#include "Logger.h"
#include "peltier.h"
#include "Controller.h"


#define MIN_PWM_STEP            50

#define PELTIER_HEAT            true
#define PELTIER_COOL            false

#define ADC_VREF                2500
#define ADC_TO_MILLIVOLTS(x)    ((x*ADC_VREF)/4096) // convert to millivolts for 12 bit ADC

static eCycleType CycleType;
static tPeltierParams PeltierParams[eLAST_PELTIER];

#define TEST_PELTIER eDETECT_PELTIER            // ePCR_PELTIER, eLYSIS_PELTIER
#define TARGET_TEMP     500   // 0.1 degree unit
static tPeltierMetrics DefaultMetrics[eLAST_PELTIER] = {
  //  Heating cycle            
  //  Kp   Ki Kd IMax   MinTemp Max Temp PwmMax
  { 150000, 5, 0, 12000, 20, 105, 50 } , 
  { 10000, 0, 0, 6000, 20 , 105, 75 }  ,
  { 50000, 1, 0, 6000, 20 , 105, 20 }
};
static tPeltierMetrics TestMetrics[eLAST_PELTIER];
static uint32_t FanTimeout = 0;
#ifdef PELTIER_DEBUG
static uint32_t OutTicks;
#endif
static uint16_t LastPcrDemand;
static uint8_t MaxPwmRun[3];            // Array for dynamically changing max pwm
static void getAdcValues(ePeltierType peltier, tPeltierParams * pPeltier);
static uint32_t PcrCycleCount;


//=================================================================================================
//! Init
/*! 
*/
void PeltierInit(void)
{
    PeltierStop();
    PeltierSetDefaultMetrics();
    PcrCycleCount = 0;
    LastPcrDemand = 0;
}
//=================================================================================================
//! Stops all the peltiers and send updated hardware state
/*! 
*/
void PeltierStop(void)
{
    uint8_t i;

    memset(PeltierParams, 0, sizeof(PeltierParams));
    for (i=0; i<eLAST_PELTIER; i++)
    {
        PeltierParams[i].mode = ePELTIER_OFF;
        PeltierParams[i].previousMode = ePELTIER_OFF;
        LoggerSend(eDEVICE_STATE,"therm%d,0\r\n", i+1);
    } 
}
//=================================================================================================
//! Reset test metrics to default metrics
/*! 
*/
void PeltierSetDefaultMetrics(void)
{
    uint8_t i;
    // Reset test metrics to default metrics
    memcpy(&TestMetrics, &DefaultMetrics, sizeof(DefaultMetrics));
    for (i=0; i<eLAST_PELTIER; i++)
    {
        MaxPwmRun[i] = TestMetrics[i].PwmMax;
    }  
}
//=================================================================================================
//! Returns a pointer to the metric values
/*! 
*/
tPeltierMetrics * PeltierGetMetrics(eMetricType metric, ePeltierType peltier)
{
    if (metric == eDEFAULT_METRIC)
    {
        return &DefaultMetrics[peltier];
    }
    else
    {
        return &TestMetrics[peltier];
    }
}

//=================================================================================================
//! Sets the peltier parameters, which will be checked on the next 5ms cycle
/*! 
*/
void PeltierSetParams(ePeltierType type, tPeltierParams * pParams)
{
    // convert to tenth of degree unit    
    pParams->topPlateTarget *= 10;
    memcpy(&PeltierParams[type], pParams, sizeof(tPeltierParams));

    if (type == ePCR_PELTIER && pParams->mode != ePELTIER_OFF)
    {    
        if (LastPcrDemand > pParams->topPlateTarget)
        {
            PcrCycleCount++;          
        }
        LastPcrDemand = pParams->topPlateTarget;
    }
    
    // Determine if cooling or heating
    getAdcValues(type, &PeltierParams[type]);
    if (pParams->topPlateTarget < PeltierParams[type].topPlateActual)
    {
        CycleType = eCOOLING_CYCLE;
    }
    else
    {
        CycleType = eHEATING_CYCLE;
    }
    PeltierParams[type].smoothedDiffError = 0;
    memset(PeltierParams[type].differentialErrorValues, 0, sizeof(int32_t) * DIFFERENTIAL_SMOOTHING_WIDTH);
    PeltierParams[type].diffValueIndex = 0;
    PeltierParams[type].integralError = 0;
    PeltierParams[type].previousError = 0;
#ifdef PELTIER_DEBUG
    OutTicks = 0;
#endif    
}
//=================================================================================================
//! Returns pointer to peltier parameters
/*! 
*/
tPeltierParams * PeltierGetParams(ePeltierType peltier)
{
    return &PeltierParams[peltier];
}
//=================================================================================================
//! Sets all the peltiers to mid rail and all fans on
/*! 
*/
void PeltierTest(bool start, bool heat)
{
    ePeltierMode mode;
    uint8_t dutyCycle;
    bool peltierFan;
    
    ePeltierType i;
    if (start)
    {
        mode = ePELTIER_TEST;
        dutyCycle = 50;
        peltierFan = true;
    }
    else
    {
        mode = ePELTIER_OFF;
        dutyCycle = 0;
        peltierFan = false;
    }
    for (i= (ePeltierType)0; i<eLAST_PELTIER; i++)
    {
        PeltierParams[i].mode = mode;
        CpldPeltierFan(i, peltierFan);
        CpldPeltierSetPWM(i, dutyCycle);
        CpldPeltierHeatCool(i, heat);
    }
}
//=================================================================================================
//! Peltier 5 ms tick - called from main loop 
/*! 
*/
void PeltierTick5Ms(void)
{
    ePeltierType i;
    tPeltierParams * pPeltier;
    int32_t dutyCycleDemand = 0; 
    int32_t currentDemand = 0;
    uint32_t pwmDutyCycle;
    
    // Go round all three peltiers and check if PID needs to be run
    for (i= (ePeltierType)0; i<eLAST_PELTIER; i++)
    {        
        pPeltier =  &PeltierParams[i];
        getAdcValues(i, pPeltier);
        if (pPeltier->mode == ePELTIER_ON && pPeltier->topPlateTarget >= 0)
        {
            int32_t output;
            int32_t dError;
            int32_t diffError;            
            CpldPeltierFan(i, true);
            pPeltier->fanOnTicks = 0;
            if (i == ePCR_PELTIER)
            {
              CpldPeltierFan(eDETECT_PELTIER, true);
              CpldPeltierFan(eLYSIS_PELTIER, true);  
              CpldPeltierFan(eLAST_PELTIER, true);
            }
            if (pPeltier->topPlateActual > TestMetrics[i].MaxTemp || 
                pPeltier->bottomPlateActual > TestMetrics[i].MaxTemp)
            {
                LoggerSend(eTHROW_TEXT, "30000,MaxTemp %d Exceeded %d %d\r\n", TestMetrics[i].MaxTemp,
                    pPeltier->topPlateActual, pPeltier->bottomPlateActual);
                CtrlAbortExecute();
            }
            else if (pPeltier->topPlateActual < TestMetrics[i].MinTemp || 
                     pPeltier->bottomPlateActual < TestMetrics[i].MinTemp)
            {
                LoggerSend(eTHROW_TEXT, "30000,MinTemp %d Exceeded %d %d\r\n",  TestMetrics[i].MinTemp,
                    pPeltier->topPlateActual, pPeltier->bottomPlateActual);
                CtrlAbortExecute();
            }
            if (CycleType == eCOOLING_CYCLE)
            {
                if (pPeltier->topPlateActual <= pPeltier->topPlateTarget)
                {
                    if (pPeltier->bAckVar != NULL)
                    {
                        *pPeltier->bAckVar = true;
                    }
                }
            }
            else
            {
                if (pPeltier->topPlateActual >= pPeltier->topPlateTarget)
                {
                    if (pPeltier->bAckVar != NULL)
                    {
                        *pPeltier->bAckVar = true;
                    }
                }
            }
            dError = pPeltier->topPlateTarget - pPeltier->topPlateActual;
            diffError = (dError - pPeltier->previousError);// / (5*DIFFERENTIAL_SMOOTHING_WIDTH);
            pPeltier->integralError += ((dError + pPeltier->previousError) * 5)/2;
            
            pPeltier->smoothedDiffError += diffError;
            pPeltier->smoothedDiffError -= pPeltier->differentialErrorValues[pPeltier->diffValueIndex];
            pPeltier->differentialErrorValues[pPeltier->diffValueIndex] = diffError;

            output = (TestMetrics[i].Kp * dError) + 
                (TestMetrics[i].Ki * pPeltier->integralError) +
                (TestMetrics[i].Kd * diffError);
            output /= 10000;
#ifdef PELTIER_DEBUG
            if (OutTicks++ % 10 == 0)
            {
              LoggerSend(eUART_DEBUG,"#%d Top %d P %d I %d D %d Output %d PWM %d\r\n", i,
                  pPeltier->topPlateActual, dError, pPeltier->integralError, pPeltier->smoothedDiffError, output, pPeltier->dutyCycleActual);               
            }
#endif
            if (output >= 0)
            {
                if (output > 1000)
                {
                    dutyCycleDemand = 1000;
                }
                else
                {
                    dutyCycleDemand = output;
                }
                CpldPeltierHeatCool(i, PELTIER_HEAT);
            }
            else
            {
                if (-output > 1000)
                {
                    dutyCycleDemand = 1000;
                }
                else if (-output > 30)
                {
                    dutyCycleDemand = -output;

                }
                CpldPeltierHeatCool(i, PELTIER_COOL);
            }
            if (pPeltier->diffValueIndex++ >= DIFFERENTIAL_SMOOTHING_WIDTH)
            {
                pPeltier->diffValueIndex = 0;
            }
            pPeltier->previousError = dError;
            // Calculate the demand current
            currentDemand = (dutyCycleDemand * TestMetrics[i].IMax)/1000;
            if (dutyCycleDemand <= MIN_PWM_STEP)
            {
                pwmDutyCycle = 0;
            }
            else
            {
                int32_t diff = ((dutyCycleDemand - pPeltier->dutyCycleActual))*100/dutyCycleDemand;
                if (diff < -50)
                {
                    // We are running more than 50% above demand
                    pPeltier->dutyCycleActual = (dutyCycleDemand * 10)/5;
                }
                else if (diff < 50)
                {
                    int32_t iMin = TestMetrics[i].IMax / 
                        (MaxPwmRun[i]*10);
                    if (pPeltier->CurrentActual < currentDemand - iMin)
                    {
                        pPeltier->dutyCycleActual += MIN_PWM_STEP;
                    }
                    else if (pPeltier->CurrentActual > currentDemand + iMin)
                    {
                        pPeltier->dutyCycleActual -= MIN_PWM_STEP;
                    }
                    else
                    {

                    }
                }
                else
                {
                    // We are running more than 50% below demand
                    pPeltier->dutyCycleActual = dutyCycleDemand/2;
                }
                pwmDutyCycle = pPeltier->dutyCycleActual/10;
                if (pwmDutyCycle >= MaxPwmRun[i])
                {
                    // Check current is not more than imax
                    if (pPeltier->CurrentActual > TestMetrics[i].IMax)
                    {
                        if (MaxPwmRun[i] > (TestMetrics[i].PwmMax * 80)/100)     // Back off Max PWM
                        {
                            MaxPwmRun[i]--;
                        }
                    }                    
                    else 
                    {
                        if (MaxPwmRun[i] < TestMetrics[i].PwmMax)
                        {
                            MaxPwmRun[i]++;
                        }
                    }
                    pwmDutyCycle = MaxPwmRun[i];
                }
            }
            if (pPeltier->dutyCycleActual != pwmDutyCycle)
            {
                //static int count = 0;
                pPeltier->dutyCycleActual = pwmDutyCycle;               
                /*if (count++ > 10)
                {
                    count = 0;
                    LoggerSend(eUART_DEBUG, "\r\n");
                }
                LoggerSend(eUART_DEBUG, "%d ", pwmDutyCycle);*/
                CpldPeltierSetPWM(i, pPeltier->dutyCycleActual);
            }
        }
        else if(pPeltier->mode == ePELTIER_OFF)
        {            
            CpldPeltierSetPWM(i, 0);
            if (pPeltier->fanOnTicks > FanTimeout) 
            {
                CpldPeltierFan(i, false);
                if (i == ePCR_PELTIER)
                {
                    CpldPeltierFan(eDETECT_PELTIER, false);
                    CpldPeltierFan(eLYSIS_PELTIER, false);   
                    CpldPeltierFan(eLAST_PELTIER, false);
                }
                pPeltier->fanOnTicks = 0;
            }
            else
            {
                pPeltier->fanOnTicks++;
            }
        }
    }
}

//=================================================================================================
//! Gets the peltier temperatures and currents
/*! 
*/
static void getAdcValues(ePeltierType peltier, tPeltierParams * pPeltier)
{
    CpldPeltierGetAdcValues(peltier, pPeltier);

// Equation is  
//      10y = 1000*(100x - 172672) / 66488 - convert millivolts to temperature ( 0.1 C)

#define Y_DIVISOR               66488
#define X_MULTIPLIER            1000
#define C_OFFSET                172672
    
    // Convert raw adc to millivolts
    pPeltier->topPlateActual = ADC_TO_MILLIVOLTS(pPeltier->topPlateActual);
    pPeltier->bottomPlateActual = ADC_TO_MILLIVOLTS(pPeltier->bottomPlateActual);
    pPeltier->CurrentActual = ADC_TO_MILLIVOLTS(pPeltier->CurrentActual);
    
    // Convert to temperature
    pPeltier->topPlateActual = 
      (((pPeltier->topPlateActual*100) - C_OFFSET)*X_MULTIPLIER)/Y_DIVISOR;
    
    pPeltier->bottomPlateActual = 
      (((pPeltier->bottomPlateActual*100) - C_OFFSET)*X_MULTIPLIER)/Y_DIVISOR;
    
    // Convert to milliamps
    pPeltier->CurrentActual = (pPeltier->CurrentActual * 1000)/105;
}
//=================================================================================================
//! Sets the fan timeout after peltiers have run
/*! 
*/
void PeltierSetFanTimeout(uint32_t timeout)
{
    FanTimeout = timeout * 1000/5;
}
//=================================================================================================
//! Gets number of PCR cycles
/*! 
*/
uint32_t PeltierGetPcrCycles(void)
{
    return PcrCycleCount;
}
//=================================================================================================
//! Resets PCR cycles
/*! 
*/
void PeltierResetPcrCycles(void)
{
    PcrCycleCount = 0;
    LastPcrDemand = 0;
}
