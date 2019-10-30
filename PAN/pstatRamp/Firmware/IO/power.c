/*! \file power.c
    \brief Handles the power controller chip
*/
#include "power.h"
#include <stdbool.h>

#define POWER_OFF_HIGH          (GPIOC->BSRR = GPIO_Pin_5)
#define POWER_OFF_LOW           (GPIOC->BRR = GPIO_Pin_5)
#define COMCV_FAN_ON            (GPIOC->BSRR = GPIO_Pin_7)        
#define COMCV_FAN_OFF           (GPIOC->BRR = GPIO_Pin_7)

#define SHUTDOWN_DELAY          (100)     // 1 second debounce delay 

static uint8_t DebounceTick;
static bool bCheckShutdown;

//=================================================================================================
//! Init - set power_off line to high
/*! 
*/
void PowerInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;

    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);
   
    GPIO_InitStructure.GPIO_Pin =  GPIO_Pin_5 | GPIO_Pin_7;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
    GPIO_Init(GPIOC, &GPIO_InitStructure);

    POWER_OFF_HIGH;
    COMCV_FAN_ON;
    
    // Set PC3 as floating input
    GPIO_InitStructure.GPIO_Pin =  GPIO_Pin_3;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
    GPIO_Init(GPIOC, &GPIO_InitStructure);

    bCheckShutdown = false;
}

//=================================================================================================
//! Checks the status of the S32 line from the COM Express chip
/*! 
*/
void PowerCheckS3(void)
{
    if (bCheckShutdown == false)
    {
        bCheckShutdown = true;
    }
}

//=================================================================================================
//! 10 ms tick 
/*! 
*/
void PowerTick10Ms(void)
{
    if (bCheckShutdown)
    {
        if (GPIO_ReadInputDataBit(GPIOC, GPIO_Pin_3) == Bit_RESET)
        {
            if (DebounceTick++ > SHUTDOWN_DELAY)
            {
                POWER_OFF_LOW;
            }
        }
        else
        {
            DebounceTick = 0;
        }
    }
}
