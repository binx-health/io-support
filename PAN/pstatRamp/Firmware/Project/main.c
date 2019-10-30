#include "includes.h"
#include "ModelTime.h"
#include "ioPstat.h"  
#include "buzzer.h"
#include "adc.h"
#include "uart1.h"
#include "tick.h"   
#include "spi1.h"
#include "command.h"
#include "peltier.h"
#include "ScriptEngine.h"
#include "Controller.h"
#include "power.h"
#include "timer.h"
#include "barcode.h"
#include "logger.h"
#include "cpld.h"
#include "button.h"
#include "emagnet.h"
#include "version.h"

#define CTRL_TICKINT_Set      ((u32)0x00000002)
#define CTRL_TICKINT_Reset    ((u32)0xFFFFFFFD)

#define MAIN_LED_ON                       (GPIOC->BSRR = GPIO_Pin_9)
#define MAIN_LED_OFF                      (GPIOC->BRR = GPIO_Pin_9)

//#define USB_LOOPBACK_TEST

uint32_t CriticalSecCntr;
uint32_t TickCount;

static bool bTickFlag;

static void enableWatchdog(void);
static void mainLed1MsTick(void);
static void ledGpioInit(void);

/*************************************************************************
* Function Name: main
* Parameters: none
*
* Return: none
*
* Description: main
*
*************************************************************************/
void main(void)
{
    __disable_interrupt();
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_AFIO, ENABLE);
    // Disable the JTAG, enabling 2 wire debug
    GPIO_PinRemapConfig(GPIO_Remap_SWJ_JTAGDisable, ENABLE);
    /* Setup STM32 system (clock, PLL and Flash configuration) */
    SystemInit();
    
    // NVIC init
    /* Set the Vector Table base location at 0x08000000 */
    NVIC_SetVectorTable(NVIC_VectTab_FLASH, 0x0);
    NVIC_PriorityGroupConfig(NVIC_PriorityGroup_4);
#ifndef DEV_BOARD    
    PowerInit();
#endif
    CpldInit();
    AdcInit();
    Spi1Init();
    Uart1Init();
    ScriptEngineInit();
    BarcodeInit();     
    TickInit();   
    EMagnetInit();
    __enable_interrupt();  
    
    ioPstatInit();    
    UsbCdcInit(); 
    PeltierInit();        
    ledGpioInit();
    CtrlInit();
    // Small delay here to allow power rails to come up, before connecting USB resisitor
    for (volatile uint32_t i=0; i<0xFFFF; i++);  
    USB_ConnectRes(true);            // Connect pull up resistor to enable USB detect

    LoggerSend(eUART_DEBUG, "\r\nIO Firmware %s\r\n", SVN_BUILD_VERSION);
    bTickFlag = false;
    TickCount = 0;
    
    enableWatchdog();
    while(1)
    {
      if (IsUsbCdcConfigure())
        {
#ifdef USB_LOOPBACK_TEST
          if (UsbCdcRxLineCount() > 0)
            {

                uint8_t c = UsbCdcGetChar();
                UsbCdcTxChar(c);
            }
#endif
            CmdHandler();
            CtrlHandler();
            ScriptEngineHandler();
            UsbCdcTxBufferSend();
        }
        if (bTickFlag)
        {
            Tick1Ms();
            mainLed1MsTick();
            SysTick->CTRL &= CTRL_TICKINT_Reset;           
            bTickFlag = false;
            SysTick->CTRL |= CTRL_TICKINT_Set;
        }
#ifdef DEV_BOARD      
        // Check buttons for testing only
        if (ButtonGetStatus() & 0x01)
        {
            LoggerSend(eTHROW_TEXT, "10001,132090870000000000\r\n");
        }
        else if (ButtonGetStatus() & 0x03)
        {
            LoggerSend(eTHROW_TEXT, "20001\r\n");
        }
        ButtonClearStatus();
#endif      
        IWDG_ReloadCounter();
        if (ShutdownCmdReceived())
        {
            // Check if micro needs to turn off Power supply
            PowerCheckS3();
        }
    }    
}

void MainTick(void)
{
    bTickFlag = true;
}

//=================================================================================================
//! Enables the IDWG 
/*! 
*/
static void enableWatchdog(void)
{
    // Enable write access to IWDG_PR and IWDG_RLR registers
    IWDG_WriteAccessCmd(IWDG_WriteAccess_Enable);

    // IWDG counter clock: 40KHz(LSI) / 32 = 1.25 KHz
    IWDG_SetPrescaler(IWDG_Prescaler_32);

    // Set counter reload value to 349
    IWDG_SetReload(349);
    // Enable IWDG (the LSI oscillator will be enabled by hardware) 
    IWDG_Enable();
}
//=================================================================================================
//! Init PC9 as output for LED control
/*! 
*/
static void ledGpioInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;  
    
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9;
    GPIO_Init(GPIOC, &GPIO_InitStructure);
    MAIN_LED_OFF;
}
//=================================================================================================
//! 1ms tick for controlling 'alive' led 
/*! 
*/
static void mainLed1MsTick(void)
{
    if(TickCount++ == 1000)
    {
        TickCount = 0;
        GPIOC->ODR ^= (1<<9);
    }
}
