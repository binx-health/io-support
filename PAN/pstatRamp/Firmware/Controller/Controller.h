/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#ifndef _CONTROLLER_H
#define _CONTROLLER_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdint.h>
#include <stdbool.h>
#include "ioPstat.h"
#include "reservoir.h"
#include "cpld.h"
#include "buzzer.h"
#include "drawer.h"
#include "peltier.h"

typedef enum {
    eP1_SENSOR, eP2_SENSOR, eP3_SENSOR, eP4_SENSOR
}ePressureSensor;


typedef enum {
    ePSTAT1, ePSTAT2, ePSTAT3, ePSTAT4
}ePstatType;

typedef enum {
    eIDLE_CTRL,
    eSTARTUP_CHECK_CTRL,
    eEXE_SCRIPT_CTRL,
    eABORT_SCRIPT_CTRL,
    eEND_STATUS_CTRL
}eCtrlState;

typedef struct {
    uint32_t FastPeriod;
    uint32_t SlowPeriod;
}tReportMetrics;

void CtrlInit(void);
void CtrlReservoir(eReservoirType type, tReservoirParams * pParams);
void CtrlSetRegValve(uint16_t voltage);
void CtrlValve(eValveType type, eOnOffAction action);
void CtrlSolenoid(eSolenoidType type, eOnOffAction action);
void CtrlDigitalPressReg(tDigPressReg * pParams);
bool CtrlStepper(tStepperParams * pParams);
void CtrlThermal(ePeltierType peltier, tPeltierParams * pParams, char * pDevice);
void CtrlPstatParams(eVoltammetryType type, tPstatParams * pParams);
void CtrlFluidDetect(ePstatType type, bool * pAckVar);
void CtrlBuzzer(uint8_t period, uint8_t dutyCycle, uint8_t runTime);
void CtrlPreport(bool bIsOn);
bool CtrlScriptExecute(char * scriptName);
void CtrlHandler(void);
eCtrlState CtrlGetState(void);
bool CtrlSetMetric(eMetricType metric, char * pString, uint32_t value);
void CtrlInitTestMetrics(void);
void CtrlAbortExecute(void);
tReportMetrics * CtrlGetReportMetrics(eMetricType metric);
void CtrlPstatControl(uint16_t millivolts, bool bInternalLoad);
void CtrlPeltiers(bool bIsOn, bool isHeating);
void CtrlTick1Ms(void);
void CtrlPstatTest(void);
void CtrlPsensorTest(void);

#ifdef __cplusplus
};
#endif

#endif
