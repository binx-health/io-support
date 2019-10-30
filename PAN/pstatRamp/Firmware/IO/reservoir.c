/**
\file reservoir.c
\brief Manages the pressure in the reservoirs
*/
#include "stm32f10x.h"
#include "reservoir.h"
#include "adc.h"
#include "cpld.h"
#include "bmp085.h"
#include "logger.h"
#include "controller.h"
#include <string.h>

//#define RES_TEST

#define PRESSURE_FULL_SCALE              (1650)  // Full scale in millibars
#define VOLTS_FULL_SCALE                 (3300) 
#define PRESSURE_ACCURACY               (50)
#define DPR_FULL_SCALE                  (960)   // regulator full scale pressure

#define VALVE_OPEN                      eOFF_ACTION
#define VALVE_CLOSED                    eON_ACTION

typedef struct {
    uint16_t pumpTime;
}tResControl;

static eReservoirType ReservoirType;
static tReservoirParams ReservoirParams[eLAST_RES];
static tResControl ResControl[eLAST_RES];
static tDigPressReg DprParams;
static uint16_t DprFinalPressure;
static int16_t DprIncrement;
static uint16_t DprIncrementRemainder;
static uint16_t DprSteps;
static uint16_t DprRampPressure;
static uint32_t VacuumPumpTime;
static uint32_t PressurePumpTime;
static tPumpMetrics PressurePumpDefault = {
  18000, 45
};
static tPumpMetrics VacuumPumpDefault = {
  18000, 45
};
static tPumpMetrics PressurePumpTest;
static tPumpMetrics VacuumPumpTest;

static tSensorMetrics SensorDefault = {
    800, 1200, 50
};

static void checkPumpTime(uint16_t ticksElapsed, uint16_t minTime, uint16_t actualPressure, uint16_t limitPressure);
static void ctrlValve(eReservoirType resType, eOnOffAction action);
static void ctrlPump(eReservoirType resType, uint8_t pwm);
static void dacInit(void);
static void setDpr1(uint16_t milliBar);


//=================================================================================================
//! Init
/*! 
*/
void ResInit(void)
{
    dacInit();
    ResReset();
    DAC_SetChannel1Data(DAC_Align_12b_R, 180);  // Set to value above 140 to prevent
                                                // pump switching with no pressure 
}
//=================================================================================================
//! Sets all valves in a safe state
/*! 
*/
void ResReset(void)
{
    eValveType i;

    ReservoirType = eVACUUM_RES;
    // Start with all vavles open
    for (i=eV1_VALVE; i< eLAST_VALVE; i++)
    {
        CpldSetValve(i, VALVE_OPEN);    
    }

    ResDumpAll();
    DprSteps = 0;   
}
//=================================================================================================
//! Resets the accumulated pump time
/*! 
*/
void ResResetAccPumpTime(void)
{
    VacuumPumpTime = 0;
    PressurePumpTime = 0;
}

//=================================================================================================
//! Returns the accumulated pump time for a given reservoir in seconds
/*! 
*/
uint16_t ResGetAccPumpTime(ePumpType type)
{
    if (type == ePRESSURE_PUMP)
    {
        return (PressurePumpTime+50)/100;
    }
    else
    {
        return (VacuumPumpTime+50)/100;
    }
}

//=================================================================================================
//! Sets action to dump all reservoirs
/*! 
*/
void ResDumpAll(void)
{
    eReservoirType resType;
    // Set all reservoirs to dump status
    for (resType=eVACUUM_RES; resType < eLAST_RES; resType++)
    {
        ReservoirParams[resType].action = eDUMP_RES;
    }
}
//=================================================================================================
//! DAC1 init
/*! 
*/
static void dacInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;
    DAC_InitTypeDef DAC_InitStructure;
    
    // GPIOA Periph clock enable
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOA, ENABLE);
    // DAC Periph clock enable
    RCC_APB1PeriphClockCmd(RCC_APB1Periph_DAC, ENABLE);

    /* Once the DAC channel is enabled, the corresponding GPIO pin is automatically 
    connected to the DAC converter. In order to avoid parasitic consumption, 
    the GPIO pin should be configured in analog */
    GPIO_InitStructure.GPIO_Pin =  GPIO_Pin_4;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AIN;
    GPIO_Init(GPIOA, &GPIO_InitStructure);
        
    // DAC channel1 Configuration
    DAC_InitStructure.DAC_Trigger = DAC_Trigger_None;
    DAC_InitStructure.DAC_WaveGeneration = DAC_WaveGeneration_None;
    DAC_InitStructure.DAC_OutputBuffer = DAC_OutputBuffer_Disable;
    DAC_InitStructure.DAC_LFSRUnmask_TriangleAmplitude = 0;
    DAC_Init(DAC_Channel_1, &DAC_InitStructure);
    DAC_Cmd(DAC_Channel_1, ENABLE);
    
    DAC_SetChannel1Data(DAC_Align_12b_R, 0);    // Set to zero
}
//=================================================================================================
//! 10ms tick for reservoir module
/*! Cycle through the 3 types of reservoir every 10 ms
*/
void ResTick10Ms(void)
{
    uint16_t actualPressure;
#ifdef RES_TEST    
    static uint16_t ResTickCount = 0;
    if (ResTickCount != 6)
    {
        ResTickCount++;
        if (ResTickCount == 6)
        {
            tDigPressReg testParams = { eON_ACTION, 130, 5000 };
            ResSetDpr1(&testParams);
        }
    }
#endif 
    if (ReservoirType++ == eHIGH_PRESSURE_RES)
    {
        ReservoirType= eVACUUM_RES;
    }
    if (ReservoirParams[ReservoirType].action == eDUMP_RES)
    {
        ctrlValve(ReservoirType, VALVE_OPEN);
        ctrlPump(ReservoirType, 0);
        ReservoirParams[ReservoirType].action = eIDLE_RES;
    }
    else if (ReservoirParams[ReservoirType].action == eHOLD_RES)
    {
        actualPressure = ResReadPressure(ReservoirType);
        if (actualPressure < ReservoirParams[ReservoirType].pressure - PRESSURE_ACCURACY)
        {
            uint32_t maxPumpTime;
            ctrlValve(ReservoirType, VALVE_CLOSED);
            if (ReservoirType == eVACUUM_RES)
            {
                maxPumpTime = VacuumPumpTest.maxRunTime;
                ctrlPump(ReservoirType, VacuumPumpTest.pwm);
            }
            else
            {
                maxPumpTime = PressurePumpTest.maxRunTime;
                ctrlPump(ReservoirType, PressurePumpTest.pwm);
            }
            if (ResControl[ReservoirType].pumpTime > maxPumpTime)
            {
                LoggerSend(eTHROW_TEXT, "30000,Excessive Pumping\r\n");
                CtrlAbortExecute();
            }
            ResControl[ReservoirType].pumpTime += 3;            // Add 30 ms to pump Time
            if (ReservoirType == eVACUUM_RES)
            {
                VacuumPumpTime += 3;
            }
            else
            {
                PressurePumpTime += 3;
            }
        } 
        else if (actualPressure > ReservoirParams[ReservoirType].pressure + PRESSURE_ACCURACY)
        { 
            checkPumpTime(ResControl[ReservoirType].pumpTime, ReservoirParams[ReservoirType].minTime, 
                          actualPressure, ReservoirParams[ReservoirType].maxPressure); 
        }
        else // Hold pressure
        {            
            checkPumpTime(ResControl[ReservoirType].pumpTime, ReservoirParams[ReservoirType].minTime,
                          actualPressure, ReservoirParams[ReservoirType].maxPressure); 
            // Check for notification
            if (ReservoirParams[ReservoirType].pAckVariable != NULL)
            {
                *ReservoirParams[ReservoirType].pAckVariable = true;
            }
        }
    }
    if (DprSteps > 0)
    {
        if (--DprSteps == 0)
        {
            setDpr1(DprFinalPressure);
        }
        else
        {
            int16_t calcVal = DprIncrement/1000;
            setDpr1(DprRampPressure);
            // Get remainder for 2 decimal point
            if (DprIncrement < 0)
            {
                DprIncrementRemainder -= (DprIncrement/10) % 100; 
            }
            else
            {
                DprIncrementRemainder += (DprIncrement/10) % 100;
            }
            if (DprIncrementRemainder >= 100)
            {
                DprIncrementRemainder -= 100;
                calcVal++;
            }
            if (DprIncrement < 0 && calcVal > 0)
            {
                DprRampPressure -= calcVal;
            }
            else
            {
                DprRampPressure += calcVal;
            }
        }
    }
}       

//=================================================================================================
//! Sets the reservoir pressure
/*! 
*/
void ResSetPressure(eReservoirType type, tReservoirParams * pParams)
{
    memcpy(&ReservoirParams[type], pParams, sizeof(tReservoirParams));
    ResControl[type].pumpTime = 0;
}

//=================================================================================================
//! reads back reservoir pressure
/*! 
*/
uint16_t ResReadPressure(eReservoirType resType)
{
    eAdcChannel adcChannel;
    uint32_t retValue;
    uint16_t adcValue;
     
    if (resType == eVACUUM_RES)
    {
        adcChannel = eVACUUM_PRESSURE_ADC;
    }
    else if (resType == eLOW_PRESSURE_RES)
    {
        adcChannel = eLOW_PRESSURE_ADC;
    }
    else if (resType == eHIGH_PRESSURE_RES)
    {
        adcChannel = eHIGH_PRESSURE_ADC;
    }
    else if (resType == eVARIABLE_PRESSURE_RES)
    {
        adcChannel = eVARIABLE_PRESSURE_ADC;
    }

    adcValue = AdcGetValue(adcChannel);
    // Convert adc value to millibar pressure
    retValue = (adcValue * PRESSURE_FULL_SCALE)/4096;

    return (uint16_t)retValue;
}

//=================================================================================================
//! Checks the minimum pump time before stopping pump
/*! 
*/
static void checkPumpTime(uint16_t ticksElapsed, uint16_t minTime, uint16_t actualPressure, 
                            uint16_t limitPressure)
{
    if (ticksElapsed > minTime || actualPressure > limitPressure)
    {
        ctrlPump(ReservoirType, 0);
        ResControl[ReservoirType].pumpTime = 0;
    }
    else if (ResControl[ReservoirType].pumpTime > 0)
    {
        ResControl[ReservoirType].pumpTime += 3;
        if (ReservoirType == eVACUUM_RES)
        {
            VacuumPumpTime += 3;
        }
        else
        {
            PressurePumpTime += 3;
        }
    }
}

//=================================================================================================
//! Controls reservoir valves
/*! 
*/
static void ctrlValve(eReservoirType resType, eOnOffAction action)
{
    eValveType valve;

    if (resType == eVACUUM_RES)
    {
        valve = eV11_VALVE;
    }
    else if (resType == eLOW_PRESSURE_RES)
    {           
        valve = eV14_VALVE;
    }
    else
    {
        valve = eV15_VALVE;
    }  
    CpldSetValve(valve, action);
}

//=================================================================================================
//! Controls the pump 
/*! 
*/
static void ctrlPump(eReservoirType resType, uint8_t pwm)
{
    if (resType == eVACUUM_RES)
    {
        CpldSetPump(eVACUUM_PUMP, pwm);  
    }
    else 
    {
        CpldSetPump(ePRESSURE_PUMP, pwm);
        if (pwm != 0)
        {
            if (resType == eLOW_PRESSURE_RES)
            {
                CpldSetValve(eV13_VALVE, VALVE_OPEN);
            }
            else
            {
                CpldSetValve(eV13_VALVE, VALVE_CLOSED);
            }
        }
    }
}

//=================================================================================================
//! Sets Dpr1 as a percentage of the atmospheric pressure
/*! 
*/
void ResSetDpr1(tDigPressReg * pParams)
{
    memcpy(&DprParams, pParams, sizeof(tDigPressReg));
    if (pParams->action == eOFF_ACTION)
    {
        DprFinalPressure = 0;  
        DprIncrement = 0;        
        DprSteps = 1;
    }
    else
    {
        uint16_t ambient = Bmp085AmbientPressure();
        // Calculate increment and steps from rampPeriod
        DprRampPressure = DprFinalPressure;  // Set starting point to previous value
        DprFinalPressure = (Bmp085AmbientPressure() * pParams->percent) / 100;        
        DprSteps = (pParams->rampPeriod/10);
        DprIncrement = (DprFinalPressure - DprRampPressure)*1000 / (pParams->rampPeriod/10);    
        DprIncrementRemainder = 0;
        if (DprSteps == 0)
        {
            DprSteps = 1;
        }
    }
}

//=================================================================================================
//! Returns dpr1 pressure (mbar)
/*! 
*/
uint16_t ResGetDpr1(void)
{
    return DprFinalPressure;
}
//=================================================================================================
//! Sets Dpr1 value according to high pressure reservoir
/*! 
*/
static void setDpr1(uint16_t milliBar)
{
    uint16_t dacValue;
    if (milliBar > 1000)
    {
        milliBar = 1000;
    }
    dacValue = (milliBar * 0xFFF )/DPR_FULL_SCALE;
    DAC_SetChannel1Data(DAC_Align_12b_R, dacValue);  
}
//=================================================================================================
//! Sets DAC voltage for the regulator valve
/*! 
*/
void ResSetRegValve(uint16_t milliVolts)
{
    uint16_t dacValue;
    dacValue = (milliVolts * 0xFFF) / VOLTS_FULL_SCALE;
    DAC_SetChannel1Data(DAC_Align_12b_R, dacValue); 
}

//=================================================================================================
//! Sets the test metrics to the default metrics
/*! 
*/
void ResSetDefaultMetrics(void)
{
    memcpy(&PressurePumpTest, &PressurePumpDefault, sizeof(tPumpMetrics));
    memcpy(&VacuumPumpTest, &VacuumPumpDefault, sizeof(tPumpMetrics));
}

//=================================================================================================
//! Returns pointer to metrics
/*! 
*/
tPumpMetrics * ResMetricsPtr(eMetricType metric, ePumpType pump)
{
    if (metric == eDEFAULT_METRIC)
    {
        if (pump == ePRESSURE_PUMP)
        {
            return &PressurePumpDefault;
        }
        else
        {
            return &VacuumPumpDefault;
        }
    }
    else
    {
        if (pump == ePRESSURE_PUMP)
        {
            return &PressurePumpTest;
        }
        else
        {
            return &VacuumPumpTest;
        }
    }
}

//=================================================================================================
//! Returns pointer to sensor metrics
/*! 
*/
tSensorMetrics * ResSensorMetricsPtr(void)
{
    return &SensorDefault;
}
