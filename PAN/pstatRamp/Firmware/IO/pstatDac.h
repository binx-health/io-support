#ifndef PSTAT_DAC_H
#define PSTAT_DAC_H

#ifdef __cplusplus
extern "C" {
#endif
#include "spi1.h"
#include <stdint.h>

void PStatDacInit(void);
void PstatDacSetVoltage(int16_t voltage);

#ifdef __cplusplus
};
#endif


#endif
