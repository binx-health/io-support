#ifndef SPI1_H
#define SPI1_H

#ifdef __cplusplus
extern "C" {
#endif
#include "stm32f10x.h"
#include <stdint.h>

void Spi1Init(void);
void Spi1SendData(uint16_t * pData, uint16_t len);
void Spi1ReadData(int16_t * pData, uint16_t len);

#ifdef __cplusplus
};
#endif


#endif
