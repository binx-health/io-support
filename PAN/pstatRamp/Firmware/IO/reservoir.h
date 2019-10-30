#ifndef RESERVOIR_H
#define RESERVOIR_H

#ifdef __cplusplus
extern "C" {
#endif
#include "cpld.h"
#include <stdint.h>
#include <stdbool.h>  

typedef enum {
    eVACUUM_RES,
    eLOW_PRESSURE_RES,
    eHIGH_PRESSURE_RES,
    eVARIABLE_PRESSURE_RES,
    eLAST_RES
}eReservoirType;

typedef enum {
    eIDLE_RES,
    eDUMP_RES,
    eHOLD_RES,
}eReservoirAction;

typedef struct {
    eReservoirAction action;
    uint16_t pressure;
    uint16_t maxPressure;
    uint16_t minTime;           // in ms ticks
    bool * pAckVariable;
}tReservoirParams;

typedef struct {
    eOnOffAction action;
    uint16_t percent;
    uint16_t rampPeriod;
}tDigPressReg;

typedef struct {
    uint32_t maxRunTime;         // Max Run time in 10 ms
    uint8_t pwm;                 // PWM of motors
}tPumpMetrics;

typedef struct {
    uint16_t minAmbient;
    uint16_t maxAmbient;
    uint16_t tolerance;
}tSensorMetrics;

void ResInit(void);
void ResReset(void);
void ResTick10Ms(void);
void ResSetPressure(eReservoirType type, tReservoirParams * pParams);
uint16_t ResReadPressure(eReservoirType resType);
void ResSetDpr1(tDigPressReg * pParams);
void ResSetRegValve(uint16_t milliVolts);
uint16_t ResGetDpr1(void);
void ResSetDefaultMetrics(void);
tPumpMetrics * ResMetricsPtr(eMetricType metric, ePumpType pump);
tSensorMetrics * ResSensorMetricsPtr(void);

void ResDumpAll(void);
void ResResetAccPumpTime(void);
uint16_t ResGetAccPumpTime(ePumpType type);

#ifdef __cplusplus
};
#endif


#endif
