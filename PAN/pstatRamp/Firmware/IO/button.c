#include "button.h"
#include <stdbool.h>

static bool bKeyUp;
static uint8_t LastStatus;
static uint8_t Status;


//=================================================================================================
//! Init
/*! 
*/
void ButtonInit(void)
{


    GPIO_InitTypeDef GPIO_InitStructure;

    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_4 | GPIO_Pin_13;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
    GPIO_Init(GPIOC, &GPIO_InitStructure);
  
    bKeyUp = true;
    LastStatus = 0;
    Status = 0;
}

//=================================================================================================
//! Tick - called every 10 ms
/*! 
*/
void ButtonTick(void)
{
    uint8_t btnStatus = GPIO_ReadInputDataBit(GPIOC, GPIO_Pin_4);
    if (GPIO_ReadInputDataBit(GPIOC, GPIO_Pin_13))
    {
        btnStatus |= 0x02;
    }
    
    if (btnStatus)
    {
        if (LastStatus != btnStatus)
        {
            LastStatus = btnStatus;
        }
        else
        {
            if (bKeyUp)
            {
                Status = btnStatus;
                bKeyUp = false;
            }
        }
    }
    else
    {
        bKeyUp = true;
    }
}

//=================================================================================================
//! 
/*! 
*/
uint8_t ButtonGetStatus(void)
{
    return Status;
}

void ButtonClearStatus(void)
{
    Status = 0;
}
