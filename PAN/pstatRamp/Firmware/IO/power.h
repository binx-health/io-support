#ifndef _POWER_H
#define _POWER_H

#ifdef __cplusplus
extern "C" {
#endif
#include "stm32f10x.h" 

void PowerInit(void);
void PowerCheckS3(void);
void PowerTick10Ms(void);

#ifdef __cplusplus
};
#endif


#endif
