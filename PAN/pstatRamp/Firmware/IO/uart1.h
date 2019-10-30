#ifndef UART1_H
#define UART1_H
#include "stm32f10x.h"
#include <stdbool.h>

#ifdef __cplusplus
extern "C" {
#endif

void Uart1Init(void);
void Uart1RxInterrupt(void);
void Uart1TxInterrupt(void);
bool Uart1TxChar(uint8_t databyte);
uint8_t Uart1RxByte(void);
uint32_t Uart1RxLineCount(void);

void Uart1RxBufferReset(void);
void Uart1TxBufferReset(void);
uint8_t * Uart1RxBufferPointer(void);
uint8_t * Uart1TxBufferPointer(void);
void Uart1TxBufferSend(void);
void Uart1TxString(char * pStr);

#ifdef __cplusplus
};
#endif


#endif
 
