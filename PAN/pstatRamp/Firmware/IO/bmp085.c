/*! \file bmp085.c
    \brief Get ambient temperature and pressure from sensor
*/
#include "bmp085.h"

#define BMP085_I2C_ADDRESS          (0xEE)
#define TEMP_MEASURE                (0x2E)
#define PRESSURE_MEASURE            (0x34)
#define OSS                         (1)

typedef struct {
    int16_t AC1;
    int16_t AC2;
    int16_t AC3;
    uint16_t AC4;
    uint16_t AC5;
    uint16_t AC6;
    int16_t B1;
    int16_t B2;
    int16_t MB;
    int16_t MC;
    int16_t MD;
}tParams;

//typedef struct 
static uint8_t RxBuffer[32];
static tParams CalValues;
static int32_t RawTemperature;
static int32_t RawPressure;
static int32_t CalcPressure;
static int32_t CalcTemperature;

static void getCalibrationParams(void);
static void readRegister(uint8_t regAddr, uint8_t numBytes, uint8_t * pBuffer);
static void startMeasure(uint8_t regAddr);
static void calcPressure(void);
static void checkEvent(uint32_t I2C_EVENT);

static eBmpState BmpState;
//=================================================================================================
//! Init
/*! 
*/
void Bmp085Init(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;
    I2C_InitTypeDef  I2C_InitStructure;

    // Enable GPIOC clock (I2C_ADx pins), I2C
    RCC_APB1PeriphClockCmd(RCC_APB1Periph_I2C1, ENABLE);
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB | RCC_APB2Periph_AFIO, ENABLE);

    I2C_DeInit(I2C1);
    // Configure I2C1 pins: SCL and SDA
    GPIO_InitStructure.GPIO_Pin =  GPIO_Pin_6 | GPIO_Pin_7;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_OD;
    GPIO_Init(GPIOB, &GPIO_InitStructure);

    // I2C configuration
    I2C_InitStructure.I2C_ClockSpeed = 150000;
    I2C_InitStructure.I2C_Mode       = I2C_Mode_I2C;
    I2C_InitStructure.I2C_DutyCycle  = I2C_DutyCycle_2;
    I2C_InitStructure.I2C_OwnAddress1 = 0;
    I2C_InitStructure.I2C_Ack         = I2C_Ack_Enable;
    I2C_InitStructure.I2C_AcknowledgedAddress = I2C_AcknowledgedAddress_7bit;
    I2C_Init(I2C1, &I2C_InitStructure);

    I2C_Cmd(I2C1, ENABLE); 

    BmpState = eIDLE_BMP;
    
    getCalibrationParams();    
}

//=================================================================================================
//! 10 ms tick
/*! 
*/
void Bmp085Tick10Ms(void)
{
    switch (BmpState)
    {
    case eTEMP_MEASURE_BMP:
        startMeasure(TEMP_MEASURE);
        BmpState = eTEMP_READ_BMP;
        break;
    case eTEMP_READ_BMP:
        readRegister(0xF6, 2, RxBuffer);
        RawTemperature = (RxBuffer[0] << 8) | RxBuffer[1];
        BmpState = ePRESSURE_MEASURE_BMP;
        break;
    case ePRESSURE_MEASURE_BMP:
        startMeasure(PRESSURE_MEASURE + (OSS<<6));
        BmpState = ePRESSURE_READ_BMP;
        break;
    case ePRESSURE_READ_BMP:
        readRegister(0xF6, 3, RxBuffer);
        RawPressure =((RxBuffer[0]<<16) | (RxBuffer[1]<<8) | RxBuffer[2]) >> (8-OSS);
        calcPressure();
        BmpState = eIDLE_BMP;
        break;
    default:
        break;
    }
}
//=================================================================================================
//! Schedules a temperature and pressure measurement
/*! 
*/
void Bmp085SetState(eBmpState state)
{
#ifndef DEV_BOARD
    BmpState = state;
#endif  
}
//=================================================================================================
//! Returns state machine
/*! 
*/
eBmpState Bmp085GetState(void)
{   
    return BmpState;
}

//=================================================================================================
//! Gets the calibration constants
/*! 
*/
static void getCalibrationParams(void)
{
    readRegister(0xAA, 22, RxBuffer);
    CalValues.AC1 = RxBuffer[0] << 8 | RxBuffer[1];
    CalValues.AC2 = RxBuffer[2] << 8 | RxBuffer[3];
    CalValues.AC3 = RxBuffer[4] << 8 | RxBuffer[5];
    CalValues.AC4 = RxBuffer[6] << 8 | RxBuffer[7];
    CalValues.AC5 = RxBuffer[8] << 8 | RxBuffer[9];
    CalValues.AC6 = RxBuffer[10] << 8 | RxBuffer[11];
    CalValues.B1 = RxBuffer[12] << 8 | RxBuffer[13];
    CalValues.B2 = RxBuffer[14] << 8 | RxBuffer[15];
    CalValues.MB = RxBuffer[16] << 8 | RxBuffer[17];
    CalValues.MC = RxBuffer[18] << 8 | RxBuffer[19];
    CalValues.MD = RxBuffer[20] << 8 | RxBuffer[21];
}

//=================================================================================================
//! Reads register address
/*! 
*/
static void readRegister(uint8_t regAddr, uint8_t numBytes, uint8_t * pBuffer)
{
    uint8_t count = 0; 
    uint16_t timeout = 0;
    if (numBytes == 2)
    {
        I2C1->CR1 |= (1<<11);                   // Set POS bit
    }    
    while(I2C_GetFlagStatus(I2C1, I2C_FLAG_BUSY));
    I2C_GenerateSTART(I2C1, ENABLE);
    // Test on EV5 and clear it 
    checkEvent(I2C_EVENT_MASTER_MODE_SELECT);
    
    // Send EEPROM address for write 
    I2C_Send7bitAddress(I2C1, BMP085_I2C_ADDRESS, I2C_Direction_Transmitter);
    // Test on EV6 and clear it 
    checkEvent(I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED);
    
    // Send the internal address to read from
    I2C_SendData(I2C1, regAddr); 
    // Test on EV8 and clear it
    checkEvent(I2C_EVENT_MASTER_BYTE_TRANSMITTED);    

    // Send START condition a second time
    I2C_GenerateSTART(I2C1, ENABLE);
    // Test on EV5 and clear it
    checkEvent(I2C_EVENT_MASTER_MODE_SELECT);

    // Send EEPROM address for read
    I2C_Send7bitAddress(I2C1, BMP085_I2C_ADDRESS, I2C_Direction_Receiver);

    // Test on EV6 and clear it
    checkEvent(I2C_EVENT_MASTER_RECEIVER_MODE_SELECTED);
    
    if (numBytes == 2)
    {
        // Disable Acknowledgement
        I2C_AcknowledgeConfig(I2C1, DISABLE);        
        while(I2C_GetFlagStatus(I2C1, I2C_FLAG_BTF) == RESET)
        {
            if (timeout++ >= 0xF000)
            {
                timeout = 0;
                break;
            }
        }
        I2C_GenerateSTOP(I2C1, ENABLE);
        pBuffer[count++] = I2C_ReceiveData(I2C1);
        pBuffer[count] = I2C_ReceiveData(I2C1);        
    }
    else
    {    
        // While there is data to be read
        while(numBytes)  
        {
            if(numBytes == 1)
            {
                // Disable Acknowledgement
                I2C_AcknowledgeConfig(I2C1, DISABLE);

                // Send STOP Condition
                I2C_GenerateSTOP(I2C1, ENABLE);
            }

            // Test on EV7 and clear it
            if(I2C_CheckEvent(I2C1, I2C_EVENT_MASTER_BYTE_RECEIVED))  
            {      
                // Read a byte from the device
                pBuffer[count++] = I2C_ReceiveData(I2C1);

                // Decrement the read bytes counter
                numBytes--;        
            } 
            if (timeout++ >= 0xF000)
            {
                break;
            }
        }
    }
    // Re-enable acknowledge
    I2C_AcknowledgeConfig(I2C1, ENABLE);
    // Clear POS bit
    I2C1->CR1 &= ~(1<<11);                   // Clear POS bit
}
//=================================================================================================
//! Starts a measurement
/*! 
*/
static void startMeasure(uint8_t regAddr)
{
    // Send STRAT condition
    I2C_GenerateSTART(I2C1, ENABLE);

    // Test on EV5 and clear it
    checkEvent(I2C_EVENT_MASTER_MODE_SELECT);

    // Send address for write
    I2C_Send7bitAddress(I2C1, BMP085_I2C_ADDRESS, I2C_Direction_Transmitter);

    // Test on EV6 and clear it
    checkEvent(I2C_EVENT_MASTER_TRANSMITTER_MODE_SELECTED);

    // Send the register address
    I2C_SendData(I2C1, 0xF4);

    // Test on EV8 and clear it
    checkEvent(I2C_EVENT_MASTER_BYTE_TRANSMITTED);

    // Write start
    I2C_SendData(I2C1, regAddr);

    /* Test on EV8 and clear it */
    checkEvent(I2C_EVENT_MASTER_BYTE_TRANSMITTED);

    // Send STOP condition
    I2C_GenerateSTOP(I2C1, ENABLE);  
}

//=================================================================================================
//! Calculates the calibrated pressure
/*! 
*/
static void calcPressure(void)
{
    int32_t x1, x2, b5, b6;
    int32_t x3, b3;
    uint32_t b4, b7;
    // calculate temperature
    x1 = ((RawTemperature - (int32_t)CalValues.AC6) * (int32_t)CalValues.AC5)>>15;
    x2 = ((int32_t)CalValues.MC <<11)/(x1 + CalValues.MD);
    b5 = x1+x2;    
    CalcTemperature = (b5+8)>>4;
    
    // calculate pressure
    b6 = b5 - 4000;
    x1 = (CalValues.B2*((b6*b6)>>12))>>11;
    x2 = (CalValues.AC2 * b6)>>11;
    x3 = x1 + x2;
    b3 = (((int32_t)(CalValues.AC1*4+x3)<<OSS)+2)/4;
    x1 = (CalValues.AC3 * b6)>>13;
    x2 = (CalValues.B1*(b6*b6)>>12)>>16;
    x3 = (x1 + x2 + 2)>>2;
    b4 = (CalValues.AC4 * (uint32_t)(x3+32768))>>15;
    b7 = ((uint32_t)RawPressure -b3)*(50000>>OSS);
    if (b7 < 0x80000000)
    {
        CalcPressure = (b7 * 2)/b4;
    }
    else
    {
        CalcPressure = (b7/b4)*2;       
    }
    x1 = (CalcPressure>>8) * (CalcPressure>>8);
    x1 = (x1 * 3038)>>16;
    x2 = (-7357 * CalcPressure)>>16;
    CalcPressure += (x1+x2+3791)>>4;
}

//=================================================================================================
//! returns the ambient pressure in milli bar
/*! 
*/
uint16_t Bmp085AmbientPressure(void)
{
    return ((uint16_t)((CalcPressure+50)/100));
}
//=================================================================================================
//! returns the ambient temperature
/*! 
*/
int16_t Bmp085AmbientTemperature(void)
{
    return (int16_t)CalcTemperature;
}
//=================================================================================================
//! Checks I2C event with timeout
/*! 
*/
static void checkEvent(uint32_t I2C_EVENT)
{
    uint16_t timeout = 0;
    while(1)
    {
        if (I2C_CheckEvent(I2C1, I2C_EVENT))
        {
            break;
        }
        if (timeout++ >= 0xF000)
        {
            break;
        }
    }
}
