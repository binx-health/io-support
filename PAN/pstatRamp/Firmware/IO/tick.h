#ifndef _TICK_H
#define _TICK_H

#ifdef __cplusplus
extern "C" {
#endif


void TickInit(void);
void Tick1Ms(void);
void Tick10Ms(void);
void TickDelayMs(uint32_t delay);
void TickIsr(void);

#ifdef __cplusplus
};
#endif


#endif
 
