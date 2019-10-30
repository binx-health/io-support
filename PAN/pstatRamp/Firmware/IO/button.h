#ifndef _BUTTON_H
#define _BUTTON_H

#ifdef __cplusplus
extern "C" {
#endif
#include "stm32f10x.h" 
#include <stdint.h>

void ButtonInit(void);
void ButtonTick(void);
uint8_t ButtonGetStatus(void);
void ButtonClearStatus(void);

#ifdef __cplusplus
};
#endif


#endif
