#ifndef _IO_PSTAT_H
#define _IO_PSTAT_H


#ifdef __cplusplus
extern "C" {
#endif
#include <stdint.h>
#include <stdbool.h>
#include "cpld.h"
  
typedef enum {
    eIDLE_PSTAT,
    eRAMP_START_PSTAT,
    eWAVE_OUT_PSTAT,
    eRAMP_END_PSTAT,
    eOUTPUT_RESULT_PSTAT,
    eFLUID_DETECT_PSTAT
}ePstatState;

typedef struct {
    int16_t V1;
    int16_t V2;
    int16_t V3;
    int16_t V4;
    uint16_t T1;
    uint16_t T2;
    bool * bAckVar;
}tPstatParams;

typedef struct {
    int16_t V0;
    uint16_t Threshold;
    int16_t VTest;
    uint16_t Adc1;
    uint16_t Adc2;
    uint16_t Adc3;
    uint16_t Adc4;
    uint16_t Tolerance;
}tPstatMetrics;

typedef enum {
    ePSTAT_CHANNEL1,
    ePSTAT_CHANNEL2,
    ePSTAT_CHANNEL3,
    ePSTAT_CHANNEL4,
    ePSTAT_LAST_CHANNEL
}ePstatAdcChannel;

typedef enum {
    eSQUARE_WAVE,
    eDIFF_PULSE
}eVoltammetryType;

void ioPstatInit(void);
void ioPstatTick(void);
void ioPstat10MsTick(void);
void ioPstatSetParams(tPstatParams * pParams);
void ioPstatStart(eVoltammetryType type);
ePstatState ioPstatGetState(void);
void ioPstatSetState(ePstatState state);
uint8_t ioPstatFluidDetect(ePstatAdcChannel pstat);
void ioPstatFluidDetectStart(bool * bAckVar);
void ioPstatDefaultMetrics(void);
tPstatMetrics * ioPstatGetMetrics(eMetricType metric);
void ioPstatSetVoltage(uint16_t millivolts);
void ioPstatSetLoad(bool bInternalLoad);
void ioPstatShowAdcValues(void);
void ioPstatTest(void);
#ifdef __cplusplus
};
#endif


#endif
 
