#include <stdio.h>
#include <string.h>
#include "stm32f10x.h"
#include <stdbool.h>
#include <string.h>
#include "cpld.h"
#include "adc.h"
#include "ioPstat.h"
#include "pstatDac.h"
#include "pstatAdc.h"
#include "tick.h"
#include "uart1.h"
#include "cd_class.h"
#include "logger.h"

typedef enum {
    eBOTTOM_VAL,
    eTOP_VAL,
    eOUTPUT_ADC,   
}eWavePos;

typedef enum {
    eFLUID_IDLE,
    eFLUID_START,
    eFLUID_MEASURE_CURRENT,
}eFluidDetectState;

typedef struct {
    eFluidDetectState state;
    bool detected;
    bool * bAckVar;
}tPstatFluid;

typedef struct {
    int16_t Channel[ePSTAT_LAST_CHANNEL];
}tCurrentDiff;


#define ADC_NUM_POINTS                  (400)
#define ADC_MAX_READINGS                (4)

static uint16_t TickCount;
static bool bRampUp;
static int16_t RampValue;
static int16_t RampIncTick;
static uint16_t T2Value;
static tPstatParams Params;
static ePstatState PstatState;
static int16_t SweepVolts;
static int16_t EndVolts;
static int16_t TopValue;
static eWavePos WavePos;
static uint16_t AdcIndex;
static uint16_t AdcStartIndex;
#define MILLIVOLTS_TO_DACVAL(x)            (((x + 1650) * 4095) / 3300)

//#define IOPSTAT_TEST
//#define EMC_IMMUNITY_TEST

#define SAMPLE_HIGH                 (GPIOB->BSRR = GPIO_Pin_5)  // PB5
#define SAMPLE_LOW                  (GPIOB->BRR = GPIO_Pin_5)   // PB5

static int16_t M1Adc[NUM_ADC_CHANNELS];
static int16_t M2Adc[NUM_ADC_CHANNELS];

static tPstatMetrics DefaultMetrics;
static tPstatMetrics TestMetrics;
static tPstatFluid  FluidDetect;
static bool bShowAdcValues;
static uint32_t PstatTickCount =0;

#ifdef IOPSTAT_TEST
static char DbgBuf[32];
#endif
#ifdef EMC_IMMUNITY_TEST

static uint32_t Beat = 0;
#endif

static tCurrentDiff DiffAdc[ADC_NUM_POINTS];

static void rampUpDown(ePstatState nextState);

//=================================================================================================
//! Ramps up or down according to RampValue and SweepVolts value
/*! 
*/
static void rampUpDown(ePstatState nextState)
{
    if (bRampUp)
    {
        if (RampValue++ >= SweepVolts)
        {
            PstatState = nextState;                
        }
    }
    else
    {
        if (RampValue-- <= SweepVolts)
        {
            PstatState = nextState;
        }
    }
    PstatDacSetVoltage(RampValue);  
}

//=================================================================================================
//! Init
/*! 
*/
void ioPstatInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB, ENABLE);
    // Configure PB5 as Output push-pul
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_5;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
    GPIO_Init(GPIOB, &GPIO_InitStructure);
    SAMPLE_HIGH;
    
    PstatState = eIDLE_PSTAT;  
    FluidDetect.state = eFLUID_IDLE;

    PStatDacInit();    
    PStatAdcInit();
#ifdef EMC_IMMUNITY_TEST
    PstatDacSetVoltage(1000);        
#else
    PstatDacSetVoltage(0); 
#endif
    TickCount = 0;
    memset(&FluidDetect, 0, sizeof(tPstatFluid));
    
    
#ifdef IOPSTAT_TEST
    static bool bIsDone;
    tPstatParams params;
    
    params.V3 = 5;
    params.V4 = 150;
    params.V1 = 100;
    params.V2 = 50;
    params.T1 = 30;
    params.T2 = 30;    
    params.bAckVar = &bIsDone;    
    SAMPLE_HIGH;
    ioPstatSetParams(&params);
    ioPstatStart(eDIFF_PULSE);      // eSQUARE_WAVE  
#endif
    bShowAdcValues = false;
    PstatTickCount = 0;
}

//=================================================================================================
//! Sets/Resets the test metrics to the default value
/*! 
*/
void ioPstatDefaultMetrics(void)
{
    memcpy(&TestMetrics, &DefaultMetrics, sizeof(tPstatMetrics));
    // initialise fluid detect array
    memset(&FluidDetect, 0, sizeof(tPstatFluid));
}
//=================================================================================================
//! 10 ms tick called from main loop
/*! 
*/
void ioPstat10MsTick(void)
{
#ifdef EMC_IMMUNITY_TEST
    if (PstatTickCount++ >= 100)
    {
        PstatTickCount = 0;
        PstatAdcGetVoltage(M2Adc);
        LoggerSend(eDEVICE_STATE, "heartbeat,%d\r\n", Beat++);
        LoggerSend(eDEVICE_STATE, "psval1,%d\r\n", M2Adc[0]);
        LoggerSend(eDEVICE_STATE, "psval2,%d\r\n", M2Adc[1]);
        LoggerSend(eDEVICE_STATE, "psval3,%d\r\n", M2Adc[2]);
        LoggerSend(eDEVICE_STATE, "psval4,%d\r\n", M2Adc[3]);
    }
#else
    if (bShowAdcValues && PstatTickCount++ >= 100)
    {
        PstatTickCount = 0;
        PstatAdcGetVoltage(M2Adc);
        LoggerSend(eDEVICE_STATE, "psval1,%d\r\n", M2Adc[0]);
        LoggerSend(eDEVICE_STATE, "psval2,%d\r\n", M2Adc[1]);
        LoggerSend(eDEVICE_STATE, "psval3,%d\r\n", M2Adc[2]);
        LoggerSend(eDEVICE_STATE, "psval4,%d\r\n", M2Adc[3]);

    }
    if (PstatState == eOUTPUT_RESULT_PSTAT)
    {
        uint16_t i;
        uint16_t count= 0;
        if (AdcStartIndex == 0)
        {
            // All done, send data back to PC
            printf("*U=%d,%d", Params.V1, Params.V3);  
        }
        for (i=AdcStartIndex; i<AdcIndex; i++)
        {
            if (UsbCdcTxCount() + 100 > 1024)
            {
                AdcStartIndex += count;
                return;
            }
            printf(",%d,%d,%d,%d", DiffAdc[i].Channel[0], DiffAdc[i].Channel[1],
                DiffAdc[i].Channel[2], DiffAdc[i].Channel[3]);
            count++;
        }
        printf("\r\n");
        // Reset all values to zero
        memset(M1Adc, 0, sizeof(M1Adc));
        memset(M2Adc, 0, sizeof(M2Adc));
        PstatState = eIDLE_PSTAT;
    }
#endif
}
//=================================================================================================
//! 1ms tick called from isr for accurate timing
/*! 
*/
void ioPstatTick(void)
{  
    if (PstatState == eRAMP_START_PSTAT)
    {
        if (TickCount++ >= RampIncTick)
        {
            TickCount = 0;
            rampUpDown(eWAVE_OUT_PSTAT);
        }   
    }
    else if (PstatState == eWAVE_OUT_PSTAT)
    {        
        if (WavePos == eBOTTOM_VAL)
        {
            if (TickCount == 0)
            {
                PstatDacSetVoltage(SweepVolts);
            }
            if (TickCount == T2Value - 1)
            {
                PstatAdcGetVoltage(M1Adc);                
            }
            if (TickCount == T2Value)
            {
                TopValue = SweepVolts+Params.V2;
                PstatDacSetVoltage(TopValue);
                TickCount = 0;              
                WavePos = eTOP_VAL;
            }
            TickCount++;
        }
        else if (WavePos == eTOP_VAL)
        {
            TickCount++;
            if (TickCount == Params.T1 - 1)
            {
                PstatAdcGetVoltage(M2Adc);                
            }
            if (TickCount == Params.T1)
            {                
                uint16_t i;
                for (i=0; i<ePSTAT_LAST_CHANNEL; i++)
                {
                    DiffAdc[AdcIndex].Channel[i] = M2Adc[i] - M1Adc[i];
                    LoggerSend(eLOGGING_TEXT, "%d: %d %d\r\n", i, M2Adc[i], M1Adc[i]);
                }
                if (AdcIndex < ADC_NUM_POINTS)
                {
                    AdcIndex++;
                }
                else
                {
                    LoggerSend(eTHROW_TEXT, "Index error\r\n");
                }
                #ifdef IOPSTAT_TEST
                LoggerSend(eUART_DEBUG, "%d, %d\r\n", M1Adc[0], M2Adc[0]);
                #endif
                SweepVolts += Params.V3;
                WavePos = eBOTTOM_VAL;
                TickCount = 0;
                if (SweepVolts  >= EndVolts)
                {
                    PstatState = eRAMP_END_PSTAT;
                }
            }
        }
    }
    else if (PstatState == eRAMP_END_PSTAT)
    {
        PstatDacSetVoltage(SweepVolts--);
        // Decrement SweepVolts until zero
        if (SweepVolts <= 0)
        {
            PstatState = eOUTPUT_RESULT_PSTAT;
            if (Params.bAckVar != NULL)
            {
                *Params.bAckVar = true;
            } 
            AdcStartIndex = 0;
        }
    }
    else if (PstatState == eFLUID_DETECT_PSTAT)
    {        
        bool bFinished = false;
        if (FluidDetect.state == eFLUID_START)
        {
            PstatDacSetVoltage(TestMetrics.V0);
            FluidDetect.state = eFLUID_MEASURE_CURRENT;
        }
        else if (FluidDetect.state == eFLUID_MEASURE_CURRENT)
        {
            PstatAdcGetVoltage(M1Adc);                
            if (M1Adc[0] >= TestMetrics.Threshold && M1Adc[1] >= TestMetrics.Threshold &&
                M1Adc[2] >= TestMetrics.Threshold && M1Adc[3] >= TestMetrics.Threshold)
            {
                if (FluidDetect.bAckVar != NULL)
                {
                    *FluidDetect.bAckVar = true;
                }
                FluidDetect.state = eFLUID_IDLE;
            }
            if (FluidDetect.state == eFLUID_IDLE)
            {
                bFinished = true;
            }
            else
            {
                bFinished = false;
            }
        }
        if (bFinished)
        {
            PstatState = eIDLE_PSTAT;    
        }
    }
}

//=================================================================================================
//! starts the square wave voltammetry
/*! 
*/
void ioPstatStart(eVoltammetryType type)
{   
    if (type == eSQUARE_WAVE)
    {
        SweepVolts = (Params.V1 - (Params.V2/2));
        T2Value = Params.T1;
        EndVolts = Params.V4 + (Params.V2/2);
    }
    else
    {
        SweepVolts = Params.V1;
        T2Value = Params.T2;
        EndVolts = Params.V4;
    }
        
    TickCount = 0;
    WavePos = eBOTTOM_VAL;
    PstatState = eRAMP_START_PSTAT;
    RampIncTick = 500 / Params.V1;
    RampValue = 0;
    if (RampIncTick < 0)
    {
        RampIncTick = -RampIncTick;
        bRampUp = false;
    }
    else
    {
        bRampUp = true;
    }
    
    AdcIndex = 0;
}

//=================================================================================================
//! Configures the potentiostat parameters
/*! 
*/
void ioPstatSetParams(tPstatParams * pParams)
{
    memcpy(&Params, pParams, sizeof(tPstatParams));
}
//=================================================================================================
//! Returns the potentiostat state
/*! 
*/
ePstatState ioPstatGetState(void)
{
    return PstatState;
}

void ioPstatSetState(ePstatState state)
{
    PstatState = state;
}
//=================================================================================================
//! Returns status of potentiostat
/*! 
*/
uint8_t ioPstatFluidDetect(ePstatAdcChannel pstat)
{    
    return FluidDetect.detected;
}

//=================================================================================================
//! Starts the fluid detect
/*! 
*/
void ioPstatFluidDetectStart(bool * bAckVar)
{
    PstatState = eFLUID_DETECT_PSTAT;
    FluidDetect.bAckVar = bAckVar;
    FluidDetect.state = eFLUID_START;
}

//=================================================================================================
//! Returns pointer to test metrics
/*! 
*/
tPstatMetrics * ioPstatGetMetrics(eMetricType metric)
{
    if (metric == eDEFAULT_METRIC)
    {
        return &DefaultMetrics;
    }
    else
    {
        return &TestMetrics;
    }
}
//=================================================================================================
//! Sets the potentiostat voltage
/*! 
*/
void ioPstatSetVoltage(uint16_t millivolts)
{
    PstatDacSetVoltage(millivolts);
}
//=================================================================================================
//! Selects internal or external load
/*! 
*/
void ioPstatSetLoad(bool bInternalLoad)
{
    (bInternalLoad) ? SAMPLE_LOW : SAMPLE_HIGH;
}
//=================================================================================================
//! Sends back ADC values
/*! 
*/
void ioPstatShowAdcValues(void)
{
    bShowAdcValues = true;
}
//=================================================================================================
//! Sets the DAC voltage and check the ADC values
/*! Throws an error if measured values are out of tolerance
    Returns true if Test is OK, false if test failed
*/
void ioPstatTest(void)
{
    char buf[64];
    bool bPass = true;
    PstatDacSetVoltage(TestMetrics.VTest);
    TickDelayMs(10);
    PstatAdcGetVoltage(M2Adc);
    sprintf(buf, "20004, Pstat ");
    // Check ADC values are within tolerance
    if ((M2Adc[0]  > TestMetrics.Adc1 + TestMetrics.Tolerance) ||
        (M2Adc[0]  < TestMetrics.Adc1 - TestMetrics.Tolerance)) 
    {
        sprintf(&buf[strlen(buf)], "C1 ");
        bPass = false;
    }
    if ((M2Adc[1]  > TestMetrics.Adc2 + TestMetrics.Tolerance) ||
        (M2Adc[1]  < TestMetrics.Adc2 - TestMetrics.Tolerance)) 
    {
        sprintf(&buf[strlen(buf)], "C2 ");
        bPass = false;
    }
    if ((M2Adc[2]  > TestMetrics.Adc3 + TestMetrics.Tolerance) ||
        (M2Adc[2]  < TestMetrics.Adc3 - TestMetrics.Tolerance)) 
    {
        sprintf(&buf[strlen(buf)], "C3 ");
        bPass = false;
    }
    if ((M2Adc[3]  > TestMetrics.Adc4 + TestMetrics.Tolerance) ||
        (M2Adc[3]  < TestMetrics.Adc4 - TestMetrics.Tolerance)) 
    {
        sprintf(&buf[strlen(buf)], "C4");
        bPass = false;
    }
    if (!bPass)
    {
        sprintf(&buf[strlen(buf)], "\r\n");
        LoggerSend(eTHROW_TEXT, buf);
    }
}