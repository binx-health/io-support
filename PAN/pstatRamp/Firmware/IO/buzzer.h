#ifndef BUZZER_H
#define BUZZER_H
#include <stdint.h>

void BuzzerInit(void);
void BuzzerCmd(uint8_t period, uint8_t dutyCycle, uint8_t runTime);
void BuzzerTick10Ms(void);

#endif
