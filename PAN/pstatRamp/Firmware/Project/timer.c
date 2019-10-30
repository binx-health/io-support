#include "stm32f10x.h"
#include "ioPstat.h"
#include "timer.h"

void TimerInit(void)
{
    TIM_TimeBaseInitTypeDef TIM1_TimeBaseInitStruct;
    NVIC_InitTypeDef NVIC_InitStructure;

    // Enable Timer1 clock and release reset
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_TIM1,ENABLE);
    RCC_APB2PeriphResetCmd(RCC_APB2Periph_TIM1,DISABLE);

    // Set timer period 10 us
    TIM1_TimeBaseInitStruct.TIM_Prescaler = 0;                  // no prescaler
    TIM1_TimeBaseInitStruct.TIM_CounterMode = TIM_CounterMode_Up;
    TIM1_TimeBaseInitStruct.TIM_Period = 7200;  // 100 us
    TIM1_TimeBaseInitStruct.TIM_ClockDivision = TIM_CKD_DIV1;
    TIM1_TimeBaseInitStruct.TIM_RepetitionCounter = 0;
    TIM_TimeBaseInit(TIM1,&TIM1_TimeBaseInitStruct);

    // Clear update interrupt bit
    TIM_ClearITPendingBit(TIM1,TIM_FLAG_Update);
    // Enable update interrupt
    TIM_ITConfig(TIM1,TIM_FLAG_Update,ENABLE);

    NVIC_InitStructure.NVIC_IRQChannel = TIM1_UP_IRQn;
    NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 7;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
    NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
    NVIC_Init(&NVIC_InitStructure);
#if 0
    {        
        GPIO_InitTypeDef GPIO_InitStructure;
        RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);
        
        GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
        GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
        GPIO_InitStructure.GPIO_Pin = GPIO_Pin_7;
        GPIO_Init(GPIOC, &GPIO_InitStructure);
        GPIO_WriteBit(GPIOC,GPIO_Pin_7, Bit_RESET);
    }
#endif

    // Enable timer counting
    TIM_Cmd(TIM1,ENABLE); 
}


void TimerIsr(void)
{
#if 0
    static bool bIsOn = FALSE;
    GPIO_WriteBit(GPIOC,GPIO_Pin_7 ,(bIsOn == FALSE)?Bit_RESET:Bit_SET);
    if (bIsOn == TRUE)
        bIsOn = FALSE;
    else
        bIsOn = TRUE;    
#endif
    TIM_ClearITPendingBit(TIM1,TIM_FLAG_Update);
}
