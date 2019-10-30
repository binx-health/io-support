/**
\file simIo.cpp
\brief simulated IO for unit testing and simulation
*/

#include "stdafx.h"
#include "Logger.h"
#include "Controller.h"
#include "logger.h"
#include "ioPstat.h"
#include "tick.h"
#include "emagnet.h"
#include "bmp085.h"

static char DebugBuf[256];

static tPumpMetrics PressurePumpDefault;
static tPumpMetrics VacuumPumpDefault;
static tPumpMetrics PressurePumpTest;
static tPumpMetrics VacuumPumpTest;
static tSensorMetrics SensorDefault;

//=================================================================================================
void ioPstatSetParams(tPstatParams * pParams)
{
    LoggerSend(eTHROW_TEXT, "PSTAT: %d %d %d %d %d %d\n", pParams->V1, pParams->V2, pParams->V3, pParams->V4,
        pParams->T1, pParams->T2);
} 

//=================================================================================================
void ioPstatStart(eVoltammetryType type)
{

}

//=================================================================================================
char * simIoGetDebugBuf(void)
{
    return DebugBuf;
}

//=================================================================================================
ePstatState ioPstatGetState(void)
{
    return eIDLE_PSTAT;
}

//=================================================================================================
void ioPstatTestOutput(void)
{

}

//============================================================================
bool CpldSetStepper(tStepperParams * pParams)
{
    return true;
}

//=================================================================================================
void ResSetPressure(eReservoirType type, tReservoirParams * pParams)
{
    if (pParams->action == eDUMP_RES)
    {
        LoggerSend(eLOGGING_TEXT, "res%d,%d\n", type+1, pParams->action);
    }
    else
    {
        LoggerSend(eLOGGING_TEXT, "res%d,%d,%d,%d\n", type+1, pParams->action, 
            pParams->pressure, pParams->minTime);   
    }
}

//=================================================================================================
void CpldSetValve(eValveType type, eOnOffAction action)
{

}

//=================================================================================================
void ResSetDpr1(tDigPressReg * pParams)
{

}
//=================================================================================================
void CpldSolenoid(eSolenoidType type, eOnOffAction action)
{

}
//=================================================================================================
void CpldPeltierSetPWM(ePeltierType peltier, uint8_t dutyCycle)
{
    printf("PWM to be set is %d\r\n", dutyCycle);
}

//=================================================================================================
void CpldPeltierGetAdcValues(ePeltierType peltier, tPeltierParams * pPeltier)
{
    pPeltier->topPlateActual += 10;
}

//============================================================================
void CpldPeltierHeatCool(ePeltierType peltier, bool bIsHeating)
{

}

//============================================================================
void CpldPeltierFan(ePeltierType peltier, bool bIsOn)
{

}

//============================================================================
void BuzzerCmd(uint8_t period, uint8_t dutyCycle, uint8_t runTime)
{

}

//==============================================================================
//! Sets the test metrics to the default metrics
/*! 
*/
void ResSetDefaultMetrics(void)
{
    memcpy(&PressurePumpDefault, &PressurePumpTest, sizeof(tPumpMetrics));
    memcpy(&VacuumPumpDefault, &VacuumPumpTest, sizeof(tPumpMetrics));
}

//==============================================================================
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
//==============================================================================
void ResResetAccPumpTime(void)
{

}
//==============================================================================
uint16_t ResGetAccPumpTime(ePumpType type)
{
    return 0;
}
//==============================================================================
tSensorMetrics * ResSensorMetricsPtr(void)
{
    return &SensorDefault;
}
//==============================================================================
tMotorMetrics * CpldGetSolMetrics(eMetricType metric, eSolenoidType sol)
{
    return 0;
}
//==============================================================================
void CpldSetSolDefaultMetrics(void)
{

}
//==============================================================================
tPstatMetrics * ioPstatGetMetrics(eMetricType metric)
{
    return 0;
}
//==============================================================================
void ioPstatDefaultMetrics(void)
{

}
//==============================================================================
void CpldAbortRun(void)
{

}
//==============================================================================
eOnOffAction CpldGetValve(eValveType type)
{
    return eOFF_ACTION;
}
//==============================================================================
uint8_t CpldGetSolenoid(eSolenoidType type)
{
    return 0;
}
//==============================================================================
void TickDelayMs(uint32_t delay)
{

}

//==============================================================================
uint16_t ResReadPressure(eReservoirType resType)
{
    return 0;
}

//============================================================================
uint8_t CpldGetStepper(eStepperType stepper)
{
    return 0;
}
//============================================================================
void CpldSetStepperDefaultMetrics(void)
{

}
//============================================================================
tMotorMetrics * CpldGetStepperMetrics(eMetricType metric)
{
    return 0;
}

//==============================================================================
eDrawerState DrawerGetState(void)
{
    return eCLOSED_DRAWER;
}
//==============================================================================
void EMagnetInit(void)
{

}

//==============================================================================
void EMagnetTick10Ms(void)
{

}

//==============================================================================
static uint8_t getStatus(void)
{
    return 0;
}

//==============================================================================
void EMagnetDiskReleased(void)
{

}
//==============================================================================
void ResDumpAll(void)
{

}
//==============================================================================
void Bmp085SetState(eBmpState state)
{

}
//==============================================================================
eBmpState Bmp085GetState(void)
{
    return eIDLE_BMP;
}
//==============================================================================
void ioPstatFluidDetectStart(bool * bAckVar)
{

}
//==============================================================================
void ResReset(void)
{

}
//==============================================================================
void CpldStepperStop(void)
{

}
//==============================================================================
void ResSetRegValve(uint16_t milliVolts)
{

}
//==============================================================================
uint16_t Bmp085AmbientPressure(void)
{
    return 0;
}
//==============================================================================
int16_t Bmp085AmbientTemperature(void)
{
    return 0;
}

//==============================================================================
void ioPstatSetVoltage(uint16_t millivolts)
{

}
//==============================================================================
void ioPstatSetLoad(bool bInternalLoad)
{

}
//==============================================================================
void ioPstatShowAdcValues(void)
{

}
//==============================================================================
void ioPstatTest(void)
{
    
}