#include "stm32f10x.h"
#include "tick.h"
#include "barcode.h"

#define BARCODE_EN_OFF              (GPIOC->BSRR = GPIO_Pin_13)
#define BARCODE_EN_ON               (GPIOC->BRR = GPIO_Pin_13)
#define BARCODE_TRIGGER_ON          (GPIOC->BRR = GPIO_Pin_12)
#define BARCODE_TRIGGER_OFF         (GPIOC->BSRR = GPIO_Pin_12)    
#define BARCODE_BUF_SIZE             32
static char BarcodeBuf[BARCODE_BUF_SIZE];
static uint8_t RxCount;
bool bDataRx;

//=================================================================================================
//! Init
/*! 
*/
void BarcodeInit(void)
{
    USART_InitTypeDef USART_InitStructure;
    GPIO_InitTypeDef GPIO_InitStructure;
    NVIC_InitTypeDef NVIC_InitStructure;
      
    // Init barcode enable and trigger line
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_12 | GPIO_Pin_13;
    GPIO_Init(GPIOC, &GPIO_InitStructure);    
    BARCODE_TRIGGER_OFF;
    BARCODE_EN_OFF;
    
    // Init UART 4
    //Configure USART4 Tx (PC 10) as alternate function push-pull
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_10;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
    GPIO_Init(GPIOC, &GPIO_InitStructure);

    // Enable USART4 clock
    RCC_APB1PeriphClockCmd(RCC_APB1Periph_UART4, ENABLE);

    USART_InitStructure.USART_BaudRate = 9600;
    USART_InitStructure.USART_WordLength = USART_WordLength_8b;
    USART_InitStructure.USART_StopBits = USART_StopBits_1;
    USART_InitStructure.USART_Parity = USART_Parity_No;
    USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
    USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;

    // Configure USART4
    USART_Init(UART4, &USART_InitStructure);

    // Enable USART4 Receive interrupt
    USART_ITConfig(UART4, USART_IT_RXNE, ENABLE);

    // Enable the USART4
    USART_Cmd(UART4, ENABLE);

    // Enable the USART4 Interrupt
    NVIC_InitStructure.NVIC_IRQChannel = UART4_IRQn;
  
    NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 5;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
    NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
    NVIC_Init(&NVIC_InitStructure); 
    
    RxCount = 0;
    bDataRx = false;
    BARCODE_EN_ON;
}

//=================================================================================================
//! Barcode / Uart4 Receive interrupt
/*! A complete barcode will be sent with carriage return
*/
void BarcodeRxIsr(void)
{
    uint8_t data;
    data = USART_ReceiveData(UART4);
    
    if (data == '\r')
    {
        bDataRx = true;
        data = 0;       // Null terminate the buffer
        BARCODE_TRIGGER_OFF;
    }
    BarcodeBuf[RxCount++] = data;
    if (RxCount >= BARCODE_BUF_SIZE)
    {
        RxCount = 0;
    }
}

//=================================================================================================
//! Turns barcode laser on and clears buffer
/*! 
*/
void BarcodeTriggerOn(void)
{
    BARCODE_TRIGGER_OFF;
    TickDelayMs(1);
    RxCount = 0;
    bDataRx = false;
    // Turns barcode trigger on
    BARCODE_TRIGGER_ON;
}

//=================================================================================================
//! Turns off barcode laser
/*! 
*/
void BarcodeTriggerOff(void)
{
    BARCODE_TRIGGER_OFF;
}

//=================================================================================================
//! Returns pointer to barcode string data
/*! 
*/
char * BarcodeGetString(void)
{
    if (bDataRx)
    {
        return BarcodeBuf;
    }
    else
    {
        return 0;
    }
}
//=================================================================================================
//! Returns true if barcode data is available
/*! 
*/
bool BarcodeHasData(void)
{
    return bDataRx;
}

//=================================================================================================
//! Clears received flag
/*! 
*/
void BarcodeClearStatus(void)
{
    bDataRx = false;
}
