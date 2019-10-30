/**
    \file
    \brief UART Module, uses the ST Firmware libraries. USART2 is used as a debug tool
*/
#include <stdio.h>
#include "stm32f10x.h"
#include <stdbool.h>
#include "uart1.h"

#define LINE_FEED           10
#define CR                  13

// UART Buffer Defines
#define USART_RX_BUFFER_SIZE 512
#define USART_TX_BUFFER_SIZE 512

#define USART_RX_BUFFER_MASK ( USART_RX_BUFFER_SIZE - 1 )
#define USART_TX_BUFFER_MASK ( USART_TX_BUFFER_SIZE - 1 )

#if ( USART_RX_BUFFER_SIZE & USART_RX_BUFFER_MASK )
    #error RX buffer size is not a power of 2
#endif
#if ( USART_TX_BUFFER_SIZE & USART_TX_BUFFER_MASK )
    #error TX buffer size is not a power of 2
#endif

// Tx Buffer
static uint8_t TxBuffer[USART_TX_BUFFER_SIZE];
static volatile uint32_t TxPos;
static volatile uint32_t TxCount;

// Rx Buffer
static uint8_t RxBuffer[USART_RX_BUFFER_SIZE];
static volatile uint32_t RxPos;
static volatile uint32_t RxCount;
static volatile uint32_t RxLineCount;

static void storeToRxBuffer(uint8_t rxChar);

//=================================================================================================
//! Uart Initialisation routine
/*!
*/
void Uart1Init(void)
{
    USART_InitTypeDef USART_InitStructure;
    GPIO_InitTypeDef GPIO_InitStructure;
    NVIC_InitTypeDef NVIC_InitStructure;
    
    // Reset buffer pointers
    Uart1TxBufferReset();
    Uart1RxBufferReset();
    
    //Configure USART1 Tx (PA 9) as alternate function push-pull
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_9;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
    GPIO_Init(GPIOA, &GPIO_InitStructure);

    // Enable USART1 clock
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_USART1, ENABLE);

    USART_InitStructure.USART_BaudRate = 921600;
    USART_InitStructure.USART_WordLength = USART_WordLength_8b;
    USART_InitStructure.USART_StopBits = USART_StopBits_1;
    USART_InitStructure.USART_Parity = USART_Parity_No;
    USART_InitStructure.USART_HardwareFlowControl = USART_HardwareFlowControl_None;
    USART_InitStructure.USART_Mode = USART_Mode_Rx | USART_Mode_Tx;

    // Configure USART1
    USART_Init(USART1, &USART_InitStructure);

    // Enable USART1 Receive interrupt, transmit interrupt is enabled when Tx buffer
    // has bytes to send
    USART_ITConfig(USART1, USART_IT_RXNE, ENABLE);

    // Enable the USART1
    USART_Cmd(USART1, ENABLE);

    // Enable the USART1 Interrupt
    NVIC_InitStructure.NVIC_IRQChannel = USART1_IRQn;
  
    NVIC_InitStructure.NVIC_IRQChannelPreemptionPriority = 4;
    NVIC_InitStructure.NVIC_IRQChannelSubPriority = 0;
    NVIC_InitStructure.NVIC_IRQChannelCmd = ENABLE;
    NVIC_Init(&NVIC_InitStructure);   
}

//=================================================================================================
//! Uart receive interrupt service routine
/*!
*/
void Uart1RxInterrupt(void)
{
    uint8_t data;
   
    data = USART_ReceiveData(USART1);

    //Ignore line feed and store carriage return in ASCII MODE
    if (RxCount < USART_RX_BUFFER_SIZE)
    {
        if (data != CR)
        {
            storeToRxBuffer(data);
        }
        if (data == LINE_FEED)
        {
            RxLineCount++;  
        }
    }
}

//=================================================================================================
//! Uart transmit interrupt service routine
/*!
*/
void Uart1TxInterrupt(void)
{
    uint32_t tmpCount = TxCount;
    uint32_t index;
    if (tmpCount)
    {
        index = (TxPos - tmpCount) & USART_TX_BUFFER_MASK;
        TxCount--;      
        USART_SendData(USART1, TxBuffer[index]);       
    }
    else
    {
        // Disable Tx interrupt when buffer is empty       
        USART_ITConfig(USART1, USART_IT_TXE, DISABLE);       
    }
}

//=================================================================================================
//! Writes byte in the Tx buffer
/*! If transmit buffer is full, the byte is ignored
    \param databyte byte to store in Tx Buffer
    \return false if transmit buffer is full
*/
bool Uart1TxChar(uint8_t databyte)
{
    bool bResult= false;
    if (TxCount != USART_TX_BUFFER_SIZE)
    {
        USART_ITConfig(USART1, USART_IT_TXE, DISABLE);
        TxBuffer[TxPos++] = databyte;
        TxPos &= USART_TX_BUFFER_MASK;
        TxCount++;
        USART_ITConfig(USART1, USART_IT_TXE, ENABLE);
        bResult = true;
    }
    return bResult;
}
//=================================================================================================
//! Writes a null terminated string to the Tx buffer
/*! If transmit buffer is full, the byte is ignored
    \param databyte byte to store in Tx Buffer
    \return false if transmit buffer is full
*/
void Uart1TxString(char * pStr)
{
    while (*pStr) 
    {
        Uart1TxChar(*pStr++);  
    }
}

//=================================================================================================
//! Returns the next character from the Rx buffer
/*! \return character in the Rx buffer
    \return 0 if Rx buffer is empty
*/
uint8_t Uart1RxByte(void)
{
    uint32_t tmpCount = RxCount;
    uint8_t b;

    if (RxCount == 0)
    {
        b = 0;
    }
    else
    { 	
        USART_ITConfig(USART1, USART_IT_RXNE, DISABLE);
        b=RxBuffer[(RxPos-tmpCount) & USART_RX_BUFFER_MASK];
        RxCount--;
        if ((b == LINE_FEED) && (RxLineCount > 0))
        {
            RxLineCount--;
        }
        USART_ITConfig(USART1, USART_IT_RXNE, ENABLE);
    }
    return b;
}

//=================================================================================================
//! Returns the number of carriage-returns in the rx buffer
/*!
*/
uint32_t Uart1RxLineCount(void)
{
    return RxLineCount;
}

//=================================================================================================
//! Returns a pointer to the start of the RxBuffer
/*!
*/
uint8_t * Uart1RxBufferPointer(void)
{
    return  RxBuffer;
}
//=================================================================================================
//! Returns a pointer to the start of the TxBuffer
/*!
*/
uint8_t * Uart1TxBufferPointer(void)
{
    return TxBuffer; 
}

//=================================================================================================
//! Sets Tx buffer pointers for sending 1k of data
/*!
*/
void Uart1TxBufferSend(void)
{
    TxPos = 0;
    TxCount = 2170;
    USART_ITConfig(USART1, USART_IT_TXE, ENABLE);
}
//=================================================================================================
//! Resets the rx buffer
/*!
*/
void Uart1RxBufferReset(void)
{
    RxPos = RxCount = 0;
    RxLineCount = 0;
}

//=================================================================================================
//! Resets the rx buffer
/*!
*/
void Uart1TxBufferReset(void)
{
    TxPos = TxCount = 0;
}

//=================================================================================================
//! Stores a byte in the Rx circular buffer
/*!
*/
static void storeToRxBuffer(uint8_t rxChar)
{
    RxBuffer[RxPos++] = rxChar;
    RxPos &= (USART_RX_BUFFER_MASK);
    RxCount++;
}




