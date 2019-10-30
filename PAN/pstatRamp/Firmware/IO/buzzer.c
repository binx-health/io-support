#include "stm32f10x.h"
#include "buzzer.h"

#define BUZZER_ON                       (GPIOC->BSRR = GPIO_Pin_6)
#define BUZZER_OFF                      (GPIOC->BRR = GPIO_Pin_6)

static uint16_t TickCount;
static uint16_t RunTicks;
static uint16_t OffTime;
static uint16_t RunTime;
static uint16_t Period;

//=================================================================================================
//! Init
/*! 
*/
void BuzzerInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;  
    
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_6;
    GPIO_Init(GPIOC, &GPIO_InitStructure);    
  
    BUZZER_OFF;
    TickCount = 0;
    RunTime = 0;
    
#ifdef BUZZER_TEST
    BuzzerCmd(1, 50, 3);
#endif    
}

//=================================================================================================
//! Buzzer command for beeping
/*! 
*/
void BuzzerCmd(uint8_t period, uint8_t dutyCycle, uint8_t runTime)
{
    // Convert to 10 ms units
    RunTime = runTime * 100;
    Period = period * 100;
    OffTime = (Period * dutyCycle)/100;    
    TickCount = 0;
    RunTicks = 0;
}

//=================================================================================================
//! 10 ms tick for buzzer
/*! 
*/
void BuzzerTick10Ms(void)
{
    if (RunTime > 0)
    {        
        if (TickCount++ == 0)
        {
            BUZZER_ON;
        }
        else if (TickCount == Period)
        {
            TickCount = 0;
        }
        else if (TickCount == OffTime)
        {
            BUZZER_OFF;
        }

        // Check for end of buzzer
        if (RunTicks++ == RunTime)
        {
            BUZZER_OFF;
            RunTime = 0;
        }
    }
}
