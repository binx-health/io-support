/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/
/**
\file Controller.c
\brief Controller module
*/
#include "Controller.h"
#include "Logger.h"
#include "ScriptEngine.h"
#include "util.h"
#include "ModelTime.h"
#include "emagnet.h"
#include "bmp085.h"

#include <stdio.h>
#include <string.h>

#define SOLENOID_TIMEOUT           50           // in 1ms unit
#define DRAWER_CLOSED_CHECK_TIMEOUT 300

#define MAX_VAL(x,y)              ((x>y)?x:y)

typedef struct {
    uint16_t topPlate;
    uint16_t bottomPlate;
}tPlatesTemperature;

static tPlatesTemperature Peltiers[eLAST_PELTIER];
static uint8_t Steppers[eLAST_STEPPER];
static uint16_t ResPressures[eLAST_RES];
static uint16_t SolenoidTick;
static uint16_t DrawerClosedTick;
static uint16_t TickCount;
static eCtrlState CtrlState;
static char ScriptName[MAX_FILENAME_SIZE];
static bool bReportAmbient = false;
static tReportMetrics DefaultReportMetrics= {
  500, 1000
};
static tReportMetrics TestReportMetrics = {
    500,1000
};

static void setPumpValues(tPumpMetrics * pMetrics, char * pParam, uint32_t value);
static uint16_t getAbsDiff(uint16_t val1, uint16_t val2);

#ifdef TEST_PSTAT_DATA
static bool bTestOutput = false;
#endif


//=================================================================================================
//! Init 
/*! 
*/
void CtrlInit(void)
{
    CtrlState = eIDLE_CTRL;
    memset(Peltiers, 0, sizeof(tPlatesTemperature) * eLAST_PELTIER);
    memset(ResPressures, 0, sizeof(uint16_t) * eLAST_RES);
    memset(Steppers, 0, sizeof(Steppers));
    SolenoidTick = 0;
    TickCount = 0;
    DrawerClosedTick = 0;
}

//=================================================================================================
//! Set reservoir
/*! 
*/
void CtrlReservoir(eReservoirType type, tReservoirParams * pParams)
{
    if (pParams->action == eDUMP_RES)
    {
        LoggerSend(eDEVICE_STATE, "res%d,%d\r\n", type+1, pParams->action);
    }
    else
    {
        LoggerSend(eDEVICE_STATE, "res%d,%d\r\n", type+1, pParams->action);
    }
    ResSetPressure(type, pParams);
}

//=================================================================================================
//! Set valves
/*! 
*/
void CtrlValve(eValveType type, eOnOffAction action)
{
    // Report back valve state to reader, valves in UI start from index 1
    LoggerSend(eLOGGING_TEXT, "v%d,%d\r\n", type+1, action);
    CpldSetValve(type, action);
    LoggerSend(eDEVICE_STATE, "v%d,%d\r\n", type+1, action);
}

//=================================================================================================
//! Set regulator valve voltage
/*! 
*/
void CtrlSetRegValve(uint16_t voltage)
{
    LoggerSend(eLOGGING_TEXT, "reg,%d\n", voltage);    
    ResSetRegValve(voltage);
}

//=================================================================================================
//! Solenoid control
/*! 
*/
void CtrlSolenoid(eSolenoidType type, eOnOffAction action)
{
    LoggerSend(eLOGGING_TEXT, "SOLENOID %d %d\n", type, action);
    CpldSolenoid(type, action);
    if (type == eDRAWER_SOL)
    {
        if (action == eON_ACTION)
        {
            SolenoidTick = SOLENOID_TIMEOUT;
            DrawerClosedTick = DRAWER_CLOSED_CHECK_TIMEOUT;
        }
    }
    else
    {
        if (action == eOFF_ACTION)
        {
            EMagnetDiskReleased();
        }
    }    
}

//=================================================================================================
//! Digital Pressure Regulator
/*! 
*/
void CtrlDigitalPressReg(tDigPressReg * pParams)
{
    LoggerSend(eLOGGING_TEXT, "dpr,%d,%d,%d\n", pParams->action, pParams->percent, pParams->rampPeriod);
    ResSetDpr1(pParams);
}

//=================================================================================================
//! Stepper control
/*! 
*/
bool CtrlStepper(tStepperParams * pParams)
{
    LoggerSend(eLOGGING_TEXT, "STEPPER: %d %d %d\n", pParams->type, pParams->speed, pParams->steps);
    if (DrawerGetState() != eCLOSED_DRAWER)
    {
        LoggerSend(eTHROW_TEXT, "30002, Drawer not closed\r\n");
        CtrlAbortExecute();
        return true;
    }
    else
    {
        return (CpldSetStepper(pParams));
    }
}
//=================================================================================================
//! Thermal control
/*! 
*/
void CtrlThermal(ePeltierType peltier, tPeltierParams * pParams, char * pDevice)
{    
    LoggerSend(eLOGGING_TEXT,"THERM: %d %d %d\n", peltier, pParams->mode, 
        pParams->topPlateTarget);
    // Send hardware state, peltiers index start from 1 in UI
    LoggerSend(eDEVICE_STATE,"%s,%d\r\n", pDevice, pParams->topPlateTarget);
    PeltierSetParams(peltier, pParams);
}
//=================================================================================================
//! Potentiostat parameters
/*! 
*/
void CtrlPstatParams(eVoltammetryType type, tPstatParams * pParams)
{
    ioPstatSetParams(pParams);
    ioPstatStart(type);
}
//=================================================================================================
//! Controls the potentiostat output voltage and load resistance
/*! 
*/
void CtrlPstatControl(uint16_t millivolts, bool bInternalLoad)
{
    ioPstatSetVoltage(millivolts);  
    ioPstatSetLoad(bInternalLoad);
    ioPstatShowAdcValues();
}
//=================================================================================================
//! Sets all the peltiers to mid rail and all fans on for testing
/*! 
*/
void CtrlPeltiers(bool bIsOn, bool isHeating)
{
    PeltierTest(bIsOn, isHeating);    
}

//=================================================================================================
//! Detects presence of fluid using potentiostat
/*! 
*/
void CtrlFluidDetect(ePstatType type, bool * pAckVar)
{
    LoggerSend(eLOGGING_TEXT,"FLUID: %d\n", type);
    ioPstatFluidDetectStart(pAckVar);
}

//=================================================================================================
//! Turns buzzer on 
/*! \params period buzzer period for beeping
            dutyCycle percent of period when buzzer is on
            runTime time to run buzzer for
*/
void CtrlBuzzer(uint8_t period, uint8_t dutyCycle, uint8_t runTime)
{
    LoggerSend(eLOGGING_TEXT, "Buzzer: %d %d %d\n", period, dutyCycle, runTime);
    BuzzerCmd(period, dutyCycle, runTime);
}

//=================================================================================================
//! Starts execution of a script
/*! 
*/
bool CtrlScriptExecute(char * scriptName)
{
    if (CtrlState != eEXE_SCRIPT_CTRL)
    {
        CtrlState = eSTARTUP_CHECK_CTRL;
        strcpy(ScriptName, scriptName);
#ifdef TEST_PSTAT_DATA       
        bTestOutput = false;
#endif        
        // Schedule ambient temperature and pressure measurement
        Bmp085SetState(eTEMP_MEASURE_BMP);
        return(ScriptEngineCheckName(scriptName));
    }
    else
    {
        return false;
    }
}
//=================================================================================================
//! 1ms tick called from main loop
/*! 
*/
void CtrlTick1Ms(void)
{
    TickCount++;
    if (SolenoidTick > 0)
    {        
        if (DrawerGetState() != eCLOSED_DRAWER)
        {
            // Turn solenoid off, in case it was left on
            CpldSolenoid(eDRAWER_SOL, eOFF_ACTION);
            LoggerSend(eTHROW_TEXT, "10003, Drawer opening\r\n");
            SolenoidTick = 0;
        }
        else if (--SolenoidTick == 0)
        {
            // Turn solenoid off, in case it was left on
            CpldSolenoid(eDRAWER_SOL, eOFF_ACTION);
        }

    }
    if (DrawerClosedTick > 0)
    {
        if (--DrawerClosedTick == 0)
        {
            // Check drawer state and throw error if still in closed state
            if (DrawerGetState() == eCLOSED_DRAWER)
            {
                LoggerSend(eTHROW_TEXT, "20002, Drawer failed to open\r\n");
            }           
        }
    }
      
    if (TickCount % TestReportMetrics.FastPeriod == 0 && 
        ioPstatGetState() != eOUTPUT_RESULT_PSTAT)
    {
        uint8_t i;
        tPeltierParams * pParams;
        LoggerSend(eTICK_COUNT, "%lld\r\n", ModelTimeGetTime().Ticks);
        // Report top and bottom peltier temperatures
        for (i=0; i<eLAST_PELTIER; i++)
        {
            uint8_t peltierId;
            pParams = PeltierGetParams((ePeltierType)i);
            if (i == ePCR_PELTIER)
            {
                peltierId = 2;
            }
            else if (i == eDETECT_PELTIER)
            {
                peltierId = 3;
            }
            else
            {
                peltierId = 1;
            }
            LoggerSend(eDEVICE_STATE,"therm%d.top,%d\r\n", peltierId, pParams->topPlateActual);
            Peltiers[i].topPlate = pParams->topPlateActual;
            LoggerSend(eDEVICE_STATE,"therm%d.bottom,%d\r\n", peltierId, pParams->bottomPlateActual);
            Peltiers[i].bottomPlate = pParams->bottomPlateActual;
        }
    }
    if (TickCount % TestReportMetrics.SlowPeriod == 0 && 
            ioPstatGetState() != eOUTPUT_RESULT_PSTAT)
    {
            uint8_t i;
            uint8_t stepper;
            uint16_t pressure;
            TickCount = 0;
            LoggerSend(eTICK_COUNT, "%lld\r\n", ModelTimeGetTime().Ticks);
            // Ambient temperature and pressure if report is turned on
            if (bReportAmbient)
            {
                LoggerSend(eDEVICE_STATE, "ps5,%d\r\n", Bmp085AmbientPressure());
                LoggerSend(eDEVICE_STATE, "therm4,%d\r\n", Bmp085AmbientTemperature());
                LoggerSend(eDEVICE_STATE, "ps5Ticks, %lld\r\n", ModelTimeGetTime().Ticks);
            }
            // Report reservoir pressures
            for (i=0; i<eLAST_RES; i++)
            {
                pressure = ResReadPressure((eReservoirType)i);
                if (ResPressures[i] != pressure)
                {
                    LoggerSend(eDEVICE_STATE,"ps%d, %d\r\n", i+1, pressure);
                    ResPressures[i] = pressure;
                }

            }
            // Steppers
            for (i=0; i<eLAST_STEPPER; i++)
            {
                stepper = CpldGetStepper((eStepperType)i);
                if (Steppers[i] != stepper)
                {
                    LoggerSend(eDEVICE_STATE,"l%d,%d\r\n", i+1, stepper);
                    Steppers[i] = stepper;
                }
            }
    }  
}

//=================================================================================================
//! Called from main loop for updating status
/*! 
*/
void CtrlHandler(void)
{
    if (CtrlState == eEXE_SCRIPT_CTRL)
    {
        if (ScriptEngineNumFibres() == 0)
        {       
            // A voltammetry could still be in progress at end of script (no active fibres)
            // Check voltammerty is not running before sending script end notification to reader
            if (ioPstatGetState() == eIDLE_PSTAT)
            {
                CtrlState = eIDLE_CTRL;            
                printf("*F=%s\r\n", ScriptName);
                // Do not initialise metrics for special script name, used for single stepping
                // in developer software
                if (ScriptName[0] != '?')
                {  
                    // Reinitialise test metrics to default values
                    CtrlInitTestMetrics();
                    CpldStepperStop();
                    PeltierStop();
                    ResReset();
                }
                LoggerSend(eDEVICE_STATE, "therm2.cycles,%d\r\n", PeltierGetPcrCycles());
                LoggerSend(eDEVICE_STATE, "p1.time,%d\r\n", ResGetAccPumpTime(ePRESSURE_PUMP));
                LoggerSend(eDEVICE_STATE, "p2.time,%d\r\n", ResGetAccPumpTime(eVACUUM_PUMP));
            }
        }        
    }
    else if (CtrlState == eABORT_SCRIPT_CTRL)
    {
        CtrlState = eIDLE_CTRL;
    }
    else if (CtrlState == eSTARTUP_CHECK_CTRL)
    {
        // Wait till measurement is done
        if (Bmp085GetState() == eIDLE_BMP)
        {
            LoggerSend(eDEVICE_STATE, "therm4,%d\r\n", Bmp085AmbientTemperature()); 
            PeltierResetPcrCycles();
            ResResetAccPumpTime();
            ScriptEngineExecute(ScriptName); 
            CtrlState = eEXE_SCRIPT_CTRL;
        }
    }
}


//=================================================================================================
//! Stops the scripts execution by reinitialising everything
/*! 
*/
void CtrlAbortExecute(void)
{
    CtrlState = eABORT_SCRIPT_CTRL;
    ScriptEngineInit();
    PeltierStop();
    CtrlInitTestMetrics();
    ResDumpAll();
    CpldAbortRun();
}
//=================================================================================================
//! Returns the controller state
/*! 
*/
eCtrlState CtrlGetState(void)
{
    return CtrlState;
}
//=================================================================================================
//! Sets a default or test metric
/*! 
*/
bool CtrlSetMetric(eMetricType metric, char * pString, uint32_t value)
{
    char * pCmd[10];
    
    uint8_t index = 0;
    bool bRetVal = false;
    pCmd[0] = strtok(pString, ".");
    index = 1;
    // Split string into tokens
    if (pCmd[0] != NULL)
    {
        while(1)
        {
            pCmd[index] = strtok(NULL, ".");
            if (pCmd[index] == NULL)
            {
                break;
            }
            else
            {
                index++;
            }
            if (index == 10)
            {
                return false;
            }
        }        
    }
    if (UtilStrcmpI(pCmd[0], "peltier") == 0)
    {
        tPeltierMetrics * pMetrics;
        ePeltierType peltier;
        if (UtilStrcmpI(pCmd[1], "fantimeout") == 0)
        {
            PeltierSetFanTimeout(value);
            bRetVal = true;
        }
        else 
        {
            if (UtilStrcmpI(pCmd[1], "pcr") == 0)
            {
                peltier = ePCR_PELTIER;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[1], "lysis") == 0)
            {
                peltier = eLYSIS_PELTIER;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[1], "detection") == 0)
            {
                peltier = eDETECT_PELTIER;
                bRetVal = true;
            }
            if (bRetVal)
            {
                pMetrics = PeltierGetMetrics(metric, peltier);
                if (UtilStrcmpI(pCmd[2], "kp") == 0)
                {
                    pMetrics->Kp = value;
                }
                else if (UtilStrcmpI(pCmd[2], "ki") == 0)
                {
                    pMetrics->Ki = value;
                }
                else if (UtilStrcmpI(pCmd[2], "kd") == 0)
                {
                    pMetrics->Kd = value;
                }
                else if (UtilStrcmpI(pCmd[2], "imax") == 0)
                {
                    pMetrics->IMax = value;
                }
                else if (UtilStrcmpI(pCmd[2], "PwmMax") == 0)
                {
                    pMetrics->PwmMax = value;
                }
                else if (UtilStrcmpI(pCmd[2], "MinTemp") == 0)
                {
                    pMetrics->MinTemp = value * 10;
                }
                else if (UtilStrcmpI(pCmd[2], "MaxTemp") == 0)
                {
                    pMetrics->MaxTemp = value * 10;
                }
                else
                {
                    bRetVal = false;
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "pump") == 0)
    {
        tPumpMetrics * pMetrics;
        if (UtilStrcmpI(pCmd[1], "vacuum") == 0)
        {
            pMetrics = ResMetricsPtr(metric, eVACUUM_PUMP);
            setPumpValues(pMetrics, pCmd[2], value);
            bRetVal = true;
        }
        else if (UtilStrcmpI(pCmd[1], "pressure") == 0)
        {
            pMetrics = ResMetricsPtr(metric, ePRESSURE_PUMP);
            setPumpValues(pMetrics, pCmd[2], value);
            bRetVal = true;
        }
    }
    else if (UtilStrcmpI(pCmd[0], "psensor") == 0)
    {
        tSensorMetrics * pMetrics = ResSensorMetricsPtr();
        if (UtilStrcmpI(pCmd[1], "ambient") == 0)
        {
            if (UtilStrcmpI(pCmd[1], "min") == 0)
            {
                pMetrics->minAmbient = value;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[1], "max") == 0)
            {
                pMetrics->maxAmbient = value;
                bRetVal = true;
            }
        }
        else if (UtilStrcmpI(pCmd[1], "tolerance") == 0)
        {
            pMetrics->tolerance = value;
            bRetVal = true;
        }
    }
    else if (UtilStrcmpI(pCmd[0], "motor") == 0)
    {
        tMotorMetrics * pMetrics;
        if (UtilStrcmpI(pCmd[1], "solenoid") == 0)
        {
            pMetrics = CpldGetSolMetrics(metric, eDRAWER_SOL);
            pMetrics->pwm = value;
            bRetVal = true;
        }
        else if (UtilStrcmpI(pCmd[1], "electromagnet") == 0)
        {
            pMetrics = CpldGetSolMetrics(metric, eELECTROMAGNET_SOL);
            pMetrics->pwm = value;
            bRetVal = true;
        }
        else if (UtilStrcmpI(pCmd[1], "steppers") == 0)
        {
            pMetrics = CpldGetStepperMetrics(metric);
            pMetrics->pwm = value;
            bRetVal = true;
        }
    }
    else if (UtilStrcmpI(pCmd[0], "pstat") == 0)
    {
        tPstatMetrics * pMetrics = ioPstatGetMetrics(metric);
        if (UtilStrcmpI(pCmd[1], "fluiddetect") == 0)
        {
            if (UtilStrcmpI(pCmd[2], "v0") == 0)
            {
                pMetrics->V0 = value;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[2], "threshold") == 0)
            {
                pMetrics->Threshold = value;
                bRetVal = true;
            }
        }
        else if (UtilStrcmpI(pCmd[1], "test") == 0)
        {
            if (UtilStrcmpI(pCmd[2], "vout") == 0)
            {
                pMetrics->VTest = value;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[2], "adc1") == 0)
            {
                pMetrics->Adc1 = value;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[2], "adc2") == 0)
            {
                pMetrics->Adc2 = value;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[2], "adc3") == 0)
            {
                pMetrics->Adc3 = value;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[2], "adc4") == 0)
            {
                pMetrics->Adc4 = value;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[2], "tolerance") == 0)
            {
                pMetrics->Tolerance = value;
                bRetVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "general") == 0)
    {
        if (UtilStrcmpI(pCmd[1], "log") == 0)
        {
            tReportMetrics * pMetrics = CtrlGetReportMetrics(metric);
            if (UtilStrcmpI(pCmd[2], "fastreporting") == 0)
            {
                pMetrics->FastPeriod = value;
                bRetVal = true;
            }
            else if (UtilStrcmpI(pCmd[2], "slowreporting") == 0)
            {
                pMetrics->SlowPeriod = value;
                bRetVal = true;
            }
        }
    }
    return bRetVal;
}

//=================================================================================================
//! Sets the pump metric values
/*! 
*/
static void setPumpValues(tPumpMetrics * pMetrics, char * pParam, uint32_t value)
{
    if (UtilStrcmpI(pParam, "maxruntime") == 0)
    {
        // Convert to 10 ms units
        pMetrics->maxRunTime = value * 100;
    }
    else if (UtilStrcmpI(pParam, "pwm") == 0)
    {
        if (value > 100)
        {
            value = 100;
        }
        pMetrics->pwm = value;
    }
}
//=================================================================================================
//! Resets the test metrics to the default values
/*! 
*/
void CtrlInitTestMetrics(void)
{
    PeltierSetDefaultMetrics();
    ResSetDefaultMetrics();
    CpldSetSolDefaultMetrics();
    CpldSetStepperDefaultMetrics();
    ioPstatDefaultMetrics();
    memcpy(&TestReportMetrics, &DefaultReportMetrics, sizeof(tReportMetrics));
}
//=================================================================================================
//! Returns pointer to reporting metrics
/*! 
*/
tReportMetrics * CtrlGetReportMetrics(eMetricType metric)
{
    if (metric == eTEST_METRIC)
    {
        return &TestReportMetrics;
    }
    else
    {
        return &DefaultReportMetrics;
    }
}
//=================================================================================================
//! Turns atmospheric pressure report and temperature on/off
/*! 
*/
void CtrlPreport(bool bIsOn)
{
    bReportAmbient = bIsOn;
}
//=================================================================================================
//! Checks the potentiostat drive circuit and adc measurement
/*! 
*/
void CtrlPstatTest(void)
{
    ioPstatTest();
}

//=================================================================================================
//! Checks the pressure sensors
/*! 
*/
void CtrlPsensorTest(void)
{
    uint16_t ambientPressure = Bmp085AmbientPressure();
    tSensorMetrics * pMetrics = ResSensorMetricsPtr();
    uint16_t vacuumPressure = ResReadPressure(eVACUUM_RES);
    uint16_t lowPressure = ResReadPressure(eLOW_PRESSURE_RES);
    uint16_t highPressure = ResReadPressure(eHIGH_PRESSURE_RES);
    uint16_t maxDiff = 0;
    
    LoggerSend(eDEVICE_STATE, "ps5,%d\r\n", ambientPressure);
    if (ambientPressure < pMetrics->minAmbient || ambientPressure > pMetrics->maxAmbient)
    {
        LoggerSend(eTHROW_TEXT, "20007, Ambient Pressure error\r\n");
    }
    maxDiff = getAbsDiff(vacuumPressure, lowPressure);
    maxDiff = MAX_VAL(maxDiff, getAbsDiff(vacuumPressure, highPressure));
    maxDiff = MAX_VAL(maxDiff, getAbsDiff(lowPressure, highPressure));

    if (maxDiff > pMetrics->tolerance)
    {
        LoggerSend(eTHROW_TEXT, "20008, Pressure sensors tolerance error\r\n");
    }

}
//=================================================================================================
//! Returns the absolute difference value 
/*! 
*/
static uint16_t getAbsDiff(uint16_t val1, uint16_t val2)
{
    if (val1 > val2)
    {
        return (val1 - val2);
    }
    else
    {
        return (val2 - val1);
    }
}