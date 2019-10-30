#ifndef CPLD_H
#define CPLD_H

#ifdef __cplusplus
extern "C" {
#endif
#include <stdbool.h>
#include <stdint.h>

typedef enum {
    eDEFAULT_METRIC,
    eTEST_METRIC,
    eLAST_METRIC
}eMetricType;

typedef enum {
    eOFF_ACTION,
    eON_ACTION
}eOnOffAction;

typedef enum {
    eL1_STEPPER, eL2_STEPPER, eL3_STEPPER, eL4_STEPPER, eL5_STEPPER, eLAST_STEPPER
}eStepperType;

typedef enum {
    eDRAWER_SOL, eELECTROMAGNET_SOL, eLAST_SOL
}eSolenoidType;

typedef enum {
    eV1_VALVE, eV2_VALVE, eV3_VALVE, eV4_VALVE, eV5_VALVE, eV6_VALVE, eV7_VALVE, eV8_VALVE,
    eV9_VALVE, eV10_VALVE, eV11_VALVE, eV12_VALVE, eV13_VALVE, eV14_VALVE, eV15_VALVE, eV16_VALVE,
    eV17_VALVE, eV18_VALVE, eV19_VALVE, eV20_VALVE,
    eLAST_VALVE
}eValveType;

typedef struct {
    eStepperType type;
    uint16_t speed;
    int16_t steps;
    bool * bAckVar;
}tStepperParams;

typedef enum {
    eVACUUM_PUMP, ePRESSURE_PUMP, eDIG_PRESSREG_VALVE
}ePumpType;

typedef enum {
    ePCR_PELTIER,
    eDETECT_PELTIER,
    eLYSIS_PELTIER,    
    eLAST_PELTIER
}ePeltierType;

#define DIFFERENTIAL_SMOOTHING_WIDTH  20

typedef enum {
    ePELTIER_OFF,
    ePELTIER_ON,
    ePELTIER_TEST
}ePeltierMode;

typedef struct {
    int32_t Kp;
    int32_t Ki;
    int32_t Kd;
    int32_t IMax;
    uint16_t MinTemp;
    uint16_t MaxTemp;
    uint8_t PwmMax;
}tPeltierMetrics;

typedef struct {
    ePeltierMode mode;
    ePeltierMode previousMode;
    int32_t topPlateActual;
    int32_t topPlateTarget;
    int32_t bottomPlateActual;
    int32_t previousError;
    int32_t integralError;
    int32_t differentialError;
    int32_t differentialErrorValues[DIFFERENTIAL_SMOOTHING_WIDTH];
    int32_t smoothedDiffError;
    int32_t CurrentActual;
    int32_t dutyCycleActual;
    uint16_t diffValueIndex;
    uint32_t fanOnTicks;
    bool * bAckVar;
}tPeltierParams;

typedef struct {
    uint8_t pwm;
}tMotorMetrics;


void CpldInit(void);
void CpldSetValve(eValveType type, eOnOffAction action);
eOnOffAction CpldGetValve(eValveType type);
void CpldTick5Ms(void);
void CpldSetPump(ePumpType pump, uint8_t pwm);
uint8_t CpldGetPump(ePumpType pump);
bool CpldSetStepper(tStepperParams * pParams);
uint8_t CpldGetStepper(eStepperType stepper);
void CpldSolenoid(eSolenoidType type, eOnOffAction action);
uint8_t CpldGetSolenoid(eSolenoidType type);
void CpldPeltierOff(ePeltierType peltier);
void CpldPeltierSetPWM(ePeltierType peltier, uint8_t dutyCycle);
void CpldPeltierGetAdcValues(ePeltierType peltier, tPeltierParams * pPeltier);
uint16_t CpldPeltierGetTopPlate(ePeltierType peltier);
uint16_t CpldPeltierGetBottomPlate(ePeltierType peltier);
void CpldPeltierHeatCool(ePeltierType peltier, bool bIsHeating);
void CpldPeltierFan(ePeltierType peltier, bool bIsOn);
tMotorMetrics * CpldGetSolMetrics(eMetricType metric, eSolenoidType sol);
void CpldSetStepperDefaultMetrics(void);
tMotorMetrics * CpldGetStepperMetrics(eMetricType metric);
void CpldSetSolDefaultMetrics(void);
void CpldAbortRun(void);
void CpldStepperStop(void);
uint8_t CpldGetThermalVersion(void);
uint8_t CpldGetPowerVersion(void);
void CpldCheckComms(void);

#ifdef __cplusplus
};
#endif

#endif
