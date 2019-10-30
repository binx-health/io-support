/**
\file Spi1Init.c
\brief SPI2 module
*/
#include "spi1.h"
//#define SPI1_TEST


static uint16_t sendData(uint16_t data);
//=================================================================================================
//! Init
/*! 
*/
void Spi1Init(void)
{
    SPI_InitTypeDef  SPI_InitStructure;
    GPIO_InitTypeDef GPIO_InitStructure;

    // Enable SPI1 and GPIOA clocks 
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB | RCC_APB2Periph_SPI1, ENABLE);
    
    // Configure SPI1 pins: SCK, MISO and MOSI 
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_5 | GPIO_Pin_6 | GPIO_Pin_7;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
    GPIO_Init(GPIOA, &GPIO_InitStructure);
    
    // SPI1 configuration  
    SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex;
    SPI_InitStructure.SPI_Mode = SPI_Mode_Master;
    SPI_InitStructure.SPI_DataSize = SPI_DataSize_16b;
    SPI_InitStructure.SPI_CPOL = SPI_CPOL_High;
    SPI_InitStructure.SPI_CPHA = SPI_CPHA_2Edge;
    SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;
    // APB1 Clock (36 MHz) divide by 4
    SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_4;
    SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;
    SPI_InitStructure.SPI_CRCPolynomial = 7;
    SPI_Init(SPI1, &SPI_InitStructure);

    // Enable SPI1  
    SPI_Cmd(SPI1, ENABLE); 
    
#ifdef SPI1_TEST
    uint16_t testData[2] = { 0xAAAA, 0x5555 };
    Spi1SendData(testData, 2);
#endif    
}

//=================================================================================================
//! Send 16 bit data over SPI
/*! 
*/
void Spi1SendData(uint16_t * pData, uint16_t len)
{
    uint16_t i;
    for (i=0; i<len; i++)
    {
        sendData(pData[i]);
    }
}
//=================================================================================================
//! Reads 16 bit data over SPI
/*! 
*/
void Spi1ReadData(int16_t * pData, uint16_t len)
{
    uint16_t i;
    for (i=0; i<len; i++)
    {
        pData[i] = sendData(0);
    }
}

//=================================================================================================
//! Send 16 bit data over SPI
/*! 
*/
static uint16_t sendData(uint16_t data)
{
    // Loop while DR register in not emplty 
    while(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_TXE) == RESET);

    // Send byte through the SPI1 peripheral 
    SPI_I2S_SendData(SPI1, data);

    // Wait to receive a byte 
    while(SPI_I2S_GetFlagStatus(SPI1, SPI_I2S_FLAG_RXNE) == RESET);

    // Return the byte read from the SPI bus 
    return SPI_I2S_ReceiveData(SPI1);  
}
