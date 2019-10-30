#ifndef PSTAT_ADC_H
#define PSTAT_ADC_H

#ifdef __cplusplus
extern "C" {
#endif
#include "spi1.h"
#include <stdint.h>

#define NUM_ADC_CHANNELS            (4)

void PStatAdcInit(void);
void PstatAdcGetVoltage(int16_t * pValues);

#ifdef __cplusplus
};
#endif


#endif
