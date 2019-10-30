/*! \file i2c.h
    \brief top level routines to read/write I2C bus
*/
#ifndef _BMP085_H
#define _BMP085_H
#include "stm32f10x.h"

#ifdef __cplusplus
extern "C" {
#endif

typedef enum {
    eIDLE_BMP,
    eTEMP_MEASURE_BMP,                  // Send I2C command to measure temperature
    eTEMP_READ_BMP,                     // Read back temperature register contents
    ePRESSURE_MEASURE_BMP,              // Send I2C command to measure pressure
    ePRESSURE_READ_BMP                  // Read back temperature register contents
}eBmpState;

void Bmp085Init(void);
void Bmp085Tick10Ms(void);
void Bmp085SetState(eBmpState state);
eBmpState Bmp085GetState(void);
uint16_t Bmp085AmbientPressure(void);
int16_t Bmp085AmbientTemperature(void);

  
#ifdef __cplusplus
};
#endif

#endif 
