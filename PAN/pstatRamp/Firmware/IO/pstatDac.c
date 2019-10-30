/**
\file pstatDac.c
\brief Potentiostat DAC module
*/
#include "stm32f10x.h"
#include "pstatDac.h"

#define DAC_VREF                    (5000)
#define DAC_CS_HIGH                 (GPIOB->BSRR = GPIO_Pin_10) 
#define DAC_CS_LOW                  (GPIOB->BRR = GPIO_Pin_10)  

//=================================================================================================
//! Set DAC output
/*! 
*/
void PStatDacInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;
    
    // Configure PB10 as Output push-pull, used as DAC Chip select 
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
    GPIO_Init(GPIOB, &GPIO_InitStructure);
    DAC_CS_HIGH;
}

//=================================================================================================
//! Set DAC output in millivolts
/*! 
*/
void PstatDacSetVoltage(int16_t voltage)
{
    // Convert voltage into a 16 bit value
    uint32_t value;
    uint16_t dacValue;

    voltage = (DAC_VREF/2) + voltage;
    
    value = ((voltage * 65536)/DAC_VREF);
    dacValue = (uint16_t)value;    
    DAC_CS_LOW;
    Spi1SendData((uint16_t *)&dacValue, 1);
    DAC_CS_HIGH;
}
