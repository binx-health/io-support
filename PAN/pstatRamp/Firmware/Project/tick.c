/**
    \file
    \brief Tick module
*/
#include "stm32f10x.h"
#include "ioPstat.h"
#include "Command.h"
#include "peltier.h"
#include "tick.h"
#include "ModelTime.h"
#include "ScriptEngine.h"
#include "bmp085.h"
#include "cpld.h"
#include "reservoir.h"
#include "buzzer.h"
#include "drawer.h"
#include "power.h"
#include "emagnet.h"
#include "controller.h"
#ifdef DEV_BOARD
#include "button.h"
#endif

#define TICKS_HOUR      (3600000)

static volatile uint32_t OtherTick;
static volatile uint32_t TickCount;
static volatile uint32_t DelayCount;

extern void MainTick(void);

//=================================================================================================
//! Tick Initialisation routine
/*! Start a 1us tick timer, interrupt handler is in stm32f10x_it.c
*/
void TickInit(void)
{
    RCC_ClocksTypeDef RCC_ClockFreq;
    uint32_t counter;    

      /* Setup SysTick Timer for 1 msec interrupts.
     ------------------------------------------
    1. The SysTick_Config() function is a CMSIS function which configure:
       - The SysTick Reload register with value passed as function parameter.
       - Configure the SysTick IRQ priority to the lowest value (0x0F).
       - Reset the SysTick Counter register.
       - Configure the SysTick Counter clock source to be Core Clock Source (HCLK).
       - Enable the SysTick Interrupt.
       - Start the SysTick Counter.
    
    2. You can change the SysTick Clock source to be HCLK_Div8 by calling the
       SysTick_CLKSourceConfig(SysTick_CLKSource_HCLK_Div8) just after the
       SysTick_Config() function call. The SysTick_CLKSourceConfig() is defined
       inside the misc.c file.

    3. You can change the SysTick IRQ priority by calling the
       NVIC_SetPriority(SysTick_IRQn,...) just after the SysTick_Config() function 
       call. The NVIC_SetPriority() is defined inside the core_cm3.h file.

    4. To adjust the SysTick time base, use the following formula:
                            
         Reload Value = SysTick Counter Clock (Hz) x  Desired Time base (s)
    
       - Reload Value is the parameter to be passed for SysTick_Config() function
       - Reload Value should not exceed 0xFFFFFF
   */
    RCC_GetClocksFreq(&RCC_ClockFreq);
    counter = RCC_ClockFreq.HCLK_Frequency/1000;
    SysTick_Config(counter);
    NVIC_SetPriority(SysTick_IRQn, 2);
    TickCount = 0;
    DelayCount = 0;   
    OtherTick = 0;   
    
    DrawerInit();
    ResInit();
    BuzzerInit();
#ifdef DEV_BOARD   
    ButtonInit();
#else
    Bmp085Init();     
#endif
}

//=================================================================================================
//! 10 ms tick
/*! Calls all the modules that need a tick
*/
void Tick10Ms(void)
{
    Bmp085Tick10Ms();
    ResTick10Ms();
    BuzzerTick10Ms();
    DrawerTick10Ms();
    PowerTick10Ms();
    EMagnetTick10Ms();
    ioPstat10MsTick();
#ifdef DEV_BOARD    
    ButtonTick();
#endif    
}

//=================================================================================================
//! 1 ms system tick
/*! Called from the main loop
*/
void Tick1Ms(void)
{     
    ScriptEngineTick();
    CmdTick();
    CtrlTick1Ms();
    if (TickCount++ % 5 == 0)
    {
        PeltierTick5Ms();
        CpldTick5Ms();
    }
    if (TickCount %10 == 0)
    {
        Tick10Ms();
    }
    if (TickCount == TICKS_HOUR)
    {
        TickCount = 0;
    }
}
//=================================================================================================
//! 1 ms system tick interrupt
/*! Called from the 1ms interrupt handler
*/
void TickIsr(void)
{
    DelayCount--;
    ioPstatTick();
    MainTick();
    ModelTimeTick();
}

//=================================================================================================
//! Delay routine, returns after x ms delay
/*! \param delay amount of ms to delay
*/
void TickDelayMs(uint32_t delay)
{
    DelayCount = delay;
    while (DelayCount > 0);
}
