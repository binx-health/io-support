/**
\file pstatDac.c
\brief Potentiostat DAC module
*/
#include "stm32f10x.h"
#include "tick.h"
#include "pstatAdc.h"

#define ADC_VREF                    (5000)
#define NUM_ADC_READINGS            (1)
#ifdef DEV_BOARD
#define CONVST_HIGH                 
#define CONVST_LOW                  
#else
#define CONVST_HIGH                 (GPIOB->BSRR = GPIO_Pin_4)  // PB4
#define CONVST_LOW                  (GPIOB->BRR = GPIO_Pin_4)   // PB4
#endif
#define BUSY_PIN_STATUS             (GPIOB->IDR & GPIO_Pin_3)   // PB3
#define ADC_CS_HIGH                 (GPIOB->BSRR = GPIO_Pin_9)  // PB9
#define ADC_CS_LOW                  (GPIOB->BRR = GPIO_Pin_9)   // PB9
#define ADC_RESET_HIGH              (GPIOB->BSRR = GPIO_Pin_8)  // PB8   
#define ADC_RESET_LOW               (GPIOB->BRR = GPIO_Pin_8)   // PB8   

#define RESISOR_GAIN_VALUE          (400000)
#define BUSY_TIMEOUT                (0x50)                // ~ 25 us  

//=================================================================================================
//! Init
/*! 
*/
void PStatAdcInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;

    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB, ENABLE);
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_4 | GPIO_Pin_8 | GPIO_Pin_9;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
    GPIO_Init(GPIOB, &GPIO_InitStructure);
    
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_3;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
    GPIO_Init(GPIOB, &GPIO_InitStructure);

    // Generate reset pulse ( ~ 100 ns) for ADC chip 
    ADC_CS_HIGH;
    CONVST_HIGH;
    ADC_RESET_LOW;
    TickDelayMs(2);
    ADC_RESET_HIGH;    
    __no_operation();
    __no_operation();
    ADC_RESET_LOW;
    CONVST_LOW;    
}

//=================================================================================================
//! Start conversion, wait for it to finish and get results
/*! Takes an average of 5 values 
*/
void PstatAdcGetVoltage(int16_t * pValues)
{
    uint32_t i, j;
    int32_t accum;
    int16_t adcValues[NUM_ADC_READINGS][NUM_ADC_CHANNELS];
    uint16_t timeout = 0;
    
    for (i=0; i<NUM_ADC_READINGS; i++)
    {
        CONVST_HIGH;
        timeout = 0;
        //wait for busy line to go high
        while(!BUSY_PIN_STATUS)
        {
            if (timeout++ > BUSY_TIMEOUT)
            {
                CONVST_LOW;
                return;
            }
        }
        // Wait for Busy line to go low, timeout and return if error
        while (BUSY_PIN_STATUS)
        {
            if (timeout++ > BUSY_TIMEOUT)
            {
                CONVST_LOW;
                return;
            }
        }
        ADC_CS_LOW;
        Spi1ReadData(&adcValues[i][0], 4);
        ADC_CS_HIGH;
        CONVST_LOW;
    }

    // Return average values
    for (i=0; i<NUM_ADC_CHANNELS; i++)
    {
        accum = 0;
        for (j=0; j<NUM_ADC_READINGS; j++)
        {
            accum += adcValues[j][i];  
        }
        accum = (accum + NUM_ADC_READINGS-1)/NUM_ADC_READINGS;    
#ifdef CONVERT_TO_MILLIVOLTS        
        accum = (ADC_VREF * accum)/32768;    
#endif        
        pValues[i] = accum;
    }
}
