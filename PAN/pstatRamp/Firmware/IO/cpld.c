/**
\file adc.c
\brief ADC Module
*/

#include "stm32f10x.h"
#include <stdbool.h>
#include <string.h>
#include "cpld.h"
#include "Logger.h"

//#define CPLD_STEPPER_TEST

#define SPI_NUM_BYTES                   (54)
#ifdef POWER_BOARD_TEST
#define SPI_TRANSACTION_SIZE            (16)
#else
#define SPI_TRANSACTION_SIZE            (SPI_NUM_BYTES)
#endif
#define CPLD_RESET_HIGH                 (GPIOB->BSRR = GPIO_Pin_11)    
#define CPLD_RESET_LOW                  (GPIOB->BRR = GPIO_Pin_11)
#define CPLD_CS_HIGH                    (GPIOB->BSRR = GPIO_Pin_12)    
#define CPLD_CS_LOW                     (GPIOB->BRR = GPIO_Pin_12)

#define TICKS_LED_TOGGLE                (200)       // 5ms unit
#define OSS_MODE                        0

#define V1_V8_INDEX                     (45)                    
#define V9_V16_INDEX                    (44)
#define V17_V20_INDEX                   (47)
#define VACUUM_PUMP_INDEX               (38)
#define PRESSURE_PUMP_INDEX             (41)
#define LED1_INDEX                      (50)
#define LED2_INDEX                      (47)
#define MOTOR_CMD_INDEX                 (33)
#define MOTOR_SPEED_INDEX               (34)
#define MOTOR_STEPS_INDEX               (36)
#define MOTOR_CURRENT_INDEX             (39)
#define EJECT_SOLENOID_INDEX            (40)
#define ELECTROMAGNET_INDEX             (43)
#define PSTAT_OSS_INDEX                 (53)
#define PELTIER_PWM_START_INDEX         (48)
#define PELTIER_CURRENT_INDEX           (0)
#define PELTIER_TEMP_INDEX              (16)
#define PELTIER_CONTROL_INDEX           (50)
#define POWER_VERSION_INDEX             (46)
#define THERMAL_VERSION_INDEX           (52)

#define CMD_BUF_SIZE                    (8)
#define CMD_BUF_MASK                    (CMD_BUF_SIZE - 1)

#define THERMAL_CPLD_VERSION            (1)
#define POWER_CPLD_VERSION              (1)

typedef enum {
    eCPLD_STEP_IDLE,
    eCPLD_STEP_SET_CURRENT,
    eCPLD_STEP_MOTOR_CMD,
    eCPLD_STEP_CLEAR_CMD,
    eCPLD_STEP_MOTOR_STATUS,
    eCPLD_MOTOR_STOP
}eCpldState;

static uint8_t SpiRxBuffer[SPI_NUM_BYTES];
static uint8_t SpiTxBuffer[SPI_NUM_BYTES];
static uint16_t TickCount;


static tStepperParams StpCmdBuf[8];            // Circular buffer for stepper commands
static eCpldState State;
static tMotorMetrics DefaultSolMetrics[eLAST_SOL];
static tMotorMetrics TestSolMetrics[eLAST_SOL];
static tMotorMetrics DefaultStepperMetrics;
static tMotorMetrics TestStepperMetrics;
static uint8_t StpCmdPos;
static uint8_t StpCmdCount;
static uint8_t StpMonitor;
static bool * bStpAckVar[eLAST_STEPPER];
static bool bCheckComms;                      // Check for CPLD response

static void stepperCmd(void);
static uint8_t getPumpIndex(ePumpType pump);

//=================================================================================================
//! Init
/*! Init SPI1 for DMA transfer of 27 16 bits words
*/
void CpldInit(void)
{
    uint16_t i;
    GPIO_InitTypeDef GPIO_InitStructure;
    SPI_InitTypeDef    SPI_InitStructure;
    DMA_InitTypeDef    DMA_InitStructure;

    RCC_AHBPeriphClockCmd(RCC_AHBPeriph_DMA1, ENABLE);
    RCC_APB1PeriphClockCmd(RCC_APB1Periph_SPI2, ENABLE);
    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOB | RCC_APB2Periph_AFIO, ENABLE);

    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_Out_PP;
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_11 | GPIO_Pin_12;
    GPIO_Init(GPIOB, &GPIO_InitStructure);

    // Generate reset line for CPLD
    CPLD_CS_HIGH;
    CPLD_RESET_HIGH;
    for (i=0; i<0xFF; i++);
    CPLD_RESET_LOW;
    for (i=0; i<0xFF; i++);
    CPLD_RESET_HIGH;
    for (i=0; i< SPI_NUM_BYTES; i++)
    {
        SpiTxBuffer[i] = 0;
    }

    // Set OSS mode
    SpiTxBuffer[PSTAT_OSS_INDEX] = OSS_MODE <<4;

    // Configure SPI_MASTER pins: SCK, MISO and MOSI
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_13 | GPIO_Pin_14 | GPIO_Pin_15;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_AF_PP;
    GPIO_Init(GPIOB, &GPIO_InitStructure);

    // SPI_MASTER configuration ------------------------------------------------
    SPI_InitStructure.SPI_Direction = SPI_Direction_2Lines_FullDuplex;
    SPI_InitStructure.SPI_Mode = SPI_Mode_Master;
    SPI_InitStructure.SPI_DataSize = SPI_DataSize_8b;
    SPI_InitStructure.SPI_CPOL = SPI_CPOL_High;
    SPI_InitStructure.SPI_CPHA = SPI_CPHA_1Edge;
    SPI_InitStructure.SPI_NSS = SPI_NSS_Soft;
    SPI_InitStructure.SPI_BaudRatePrescaler = SPI_BaudRatePrescaler_4;
    SPI_InitStructure.SPI_FirstBit = SPI_FirstBit_MSB;
    SPI_InitStructure.SPI_CRCPolynomial = 7;
    SPI_Init(SPI2, &SPI_InitStructure);

    // DMA Channel 4 (Rx)
    DMA_DeInit(DMA1_Channel4);
    DMA_InitStructure.DMA_PeripheralBaseAddr = (uint32_t)(&(SPI2->DR));
#ifdef POWER_BOARD_TEST
    DMA_InitStructure.DMA_MemoryBaseAddr = (uint32_t)&SpiRxBuffer[32];
#else
    DMA_InitStructure.DMA_MemoryBaseAddr = (uint32_t)SpiRxBuffer;
#endif
    DMA_InitStructure.DMA_DIR = DMA_DIR_PeripheralSRC;
    DMA_InitStructure.DMA_BufferSize = SPI_TRANSACTION_SIZE;
    DMA_InitStructure.DMA_PeripheralInc = DMA_PeripheralInc_Disable;
    DMA_InitStructure.DMA_MemoryInc = DMA_MemoryInc_Enable;
    DMA_InitStructure.DMA_PeripheralDataSize = DMA_PeripheralDataSize_Byte;
    DMA_InitStructure.DMA_MemoryDataSize = DMA_MemoryDataSize_Byte;
    DMA_InitStructure.DMA_Mode = DMA_Mode_Normal;
    DMA_InitStructure.DMA_Priority = DMA_Priority_High;
    DMA_InitStructure.DMA_M2M = DMA_M2M_Disable;
    DMA_Init(DMA1_Channel4, &DMA_InitStructure);

    // DMA Channel 5 (Tx) 
    DMA_DeInit(DMA1_Channel5);
    DMA_InitStructure.DMA_PeripheralBaseAddr = (uint32_t)(&(SPI2->DR));
#ifdef POWER_BOARD_TEST
    DMA_InitStructure.DMA_MemoryBaseAddr = (uint32_t)&SpiTxBuffer[32];
#else
    DMA_InitStructure.DMA_MemoryBaseAddr = (uint32_t)SpiTxBuffer;
#endif
    DMA_InitStructure.DMA_DIR = DMA_DIR_PeripheralDST;
    DMA_InitStructure.DMA_Priority = DMA_Priority_Low;
    DMA_Init(DMA1_Channel5, &DMA_InitStructure);

    // Enable SPI_MASTER
    SPI_Cmd(SPI2, ENABLE);
    // Enable SPI_MASTER DMA Tx, Rx request
    SPI_I2S_DMACmd(SPI2, SPI_I2S_DMAReq_Tx | SPI_I2S_DMAReq_Rx , ENABLE);

    DMA_Cmd(DMA1_Channel4, DISABLE);
    DMA_Cmd(DMA1_Channel5, DISABLE);     
    TickCount = 0;   
    State = eCPLD_STEP_IDLE;
    StpCmdPos = 0;
    StpCmdCount = 0;
    StpMonitor = 0;
    bCheckComms = false;
#ifdef CPLD_STEPPER_TEST    
    static bool bTestFlag;
    tStepperParams params = {
       300, -500, &bTestFlag
    };
    CpldSetStepper(eL1_STEPPER, &params);
#endif    
}

//=================================================================================================
//! Checks CPLD versions are correct
/*! 
*/
void CpldCheckComms(void)
{
    bCheckComms = true;
}
                 
//=================================================================================================
//! Sets valve to open or close position
/*! 
*/
void CpldSetValve(eValveType type, eOnOffAction action)
{
    uint8_t index;
    uint8_t shift = 0;
    if (type <= eV8_VALVE)
    {
        index = V1_V8_INDEX;
        shift = (type - eV1_VALVE);
    }
    else if (type >= eV9_VALVE && type <= eV16_VALVE)
    {
        index = V9_V16_INDEX;
        shift = (type - eV9_VALVE);
    }
    else
    {
        index = V17_V20_INDEX;
        shift = (type - eV17_VALVE);
    }
    if (action == eON_ACTION)
    {
        SpiTxBuffer[index] |= (1<<shift);
    }
    else
    {
        SpiTxBuffer[index] &= ~(1<<shift);
    }    
}

//=================================================================================================
//! Returns status of valve
/*! 
*/
eOnOffAction CpldGetValve(eValveType type)
{
    uint8_t index;
    uint8_t shift = 0;
    if (type <= eV8_VALVE)
    {
        index = V1_V8_INDEX;
        shift = (type - eV1_VALVE);
    }
    else if (type >= eV9_VALVE && type <= eV16_VALVE)
    {
        index = V9_V16_INDEX;
        shift = (type - eV9_VALVE);
    }
    else
    {
        index = V17_V20_INDEX;
        shift = (type - eV17_VALVE);
    }
    if (SpiTxBuffer[index] & (1<<shift))
    {
        return eON_ACTION;
    }
    else
    {
        return eOFF_ACTION;
    }
}

//=================================================================================================
//! 5 ms tick, for getting and setting CPLD registers
/*! 
*/
void CpldTick5Ms(void)
{  
    if (TickCount++ == TICKS_LED_TOGGLE)
    {
        TickCount = 0;
        SpiTxBuffer[LED1_INDEX] ^= 0x80;
        SpiTxBuffer[LED2_INDEX] ^= 0x80; 
    }

    switch (State)
    {
    case eCPLD_STEP_IDLE:
        if (StpCmdCount > 0)
        {
            State = eCPLD_STEP_SET_CURRENT;
        }
        break;
    case eCPLD_STEP_SET_CURRENT:
        if (SpiTxBuffer[MOTOR_CURRENT_INDEX] == TestStepperMetrics.pwm)
        {
            stepperCmd();
            State = eCPLD_STEP_CLEAR_CMD;   
        }
        else
        {
            SpiTxBuffer[MOTOR_CURRENT_INDEX] = TestStepperMetrics.pwm;
            State = eCPLD_STEP_MOTOR_CMD;
        }
        break;
    case eCPLD_STEP_MOTOR_CMD:        
        stepperCmd();
        State = eCPLD_STEP_CLEAR_CMD;
        break;
    case eCPLD_STEP_CLEAR_CMD:
        SpiTxBuffer[MOTOR_CMD_INDEX] = 0;
        if (StpCmdCount > 0)
        {
            State = eCPLD_STEP_MOTOR_CMD;
        }
        else
        {
            State = eCPLD_STEP_MOTOR_STATUS;
        }
        break;
    case eCPLD_STEP_MOTOR_STATUS:
        if (StpMonitor == 0)
        {
            State = eCPLD_STEP_IDLE;
        }
        else 
        {
            eStepperType i;
            for (i=eL1_STEPPER; i<eLAST_STEPPER; i++)
            {
                if ((StpMonitor & (1<<i)) != 0)
                {
                    if ((SpiRxBuffer[MOTOR_CMD_INDEX] & (1<<i)) == 0)
                    {
                        if (bStpAckVar[i] != NULL)
                        {
                            *bStpAckVar[i] = true;
                        }
                        StpMonitor &= ~(1<<i);
                    }
                }
            }
        }
        break;
    case eCPLD_MOTOR_STOP:
        if (StpMonitor == 0)
        {
            State = eCPLD_STEP_IDLE;
        }
        else
        {
            eStepperType i;
            for (i=eL1_STEPPER; i<eLAST_STEPPER; i++)
            {
                if ((StpMonitor & (1<<i)) != 0)
                {
                    SpiTxBuffer[MOTOR_CMD_INDEX] = i + 1;
                    SpiTxBuffer[MOTOR_CMD_INDEX] += (3<<4);
                    StpMonitor &= ~(1<<i);
                    break;
                }
            }
            break;
        }
    }

    CPLD_CS_LOW;
    DMA1_Channel4->CNDTR = SPI_TRANSACTION_SIZE;
    DMA1_Channel5->CNDTR = SPI_TRANSACTION_SIZE;
    DMA_ClearFlag(DMA1_FLAG_TC5);
    DMA_ClearFlag(DMA1_FLAG_TC4);
    // Enable DMA channels
    DMA_Cmd(DMA1_Channel4, ENABLE);
    DMA_Cmd(DMA1_Channel5, ENABLE);

    // Transfer complete
    while(!DMA_GetFlagStatus(DMA1_FLAG_TC5));
    while(!DMA_GetFlagStatus(DMA1_FLAG_TC4));

    DMA_Cmd(DMA1_Channel4, DISABLE);
    DMA_Cmd(DMA1_Channel5, DISABLE);
    CPLD_CS_HIGH;    
    
    if (bCheckComms)
    {
#ifdef DEV_BOARD      
#else
        if (SpiRxBuffer[POWER_VERSION_INDEX] == 0xFF ||
            SpiRxBuffer[THERMAL_VERSION_INDEX] == 0xFF)
        {
            LoggerSend(eTHROW_TEXT, "30006, CPLD Comms error\r\n");
            bCheckComms = false;
        }
#endif
    }
}

//=================================================================================================
//! Controls the pump
/*! 
*/
void CpldSetPump(ePumpType pump, uint8_t pwm)
{
    SpiTxBuffer[getPumpIndex(pump)] = pwm;    
}

//=================================================================================================
//! Returns status of pump
/*! \returns 1 if pump is running
             0 pump is stopped
*/
uint8_t CpldGetPump(ePumpType pump)
{
    if (SpiTxBuffer[getPumpIndex(pump)] != 0)
    {
        return 1;
    }
    else
    {
        return 0;
    }
}

//=================================================================================================
//! Controls the stepper motors
/*! 
*/
bool CpldSetStepper(tStepperParams * pParams)
{
    if (StpCmdCount < CMD_BUF_SIZE)
    {
        tStepperParams * pStpCmd = &StpCmdBuf[StpCmdPos++];
        StpCmdPos &= CMD_BUF_MASK;
        memcpy(pStpCmd, pParams, sizeof(tStepperParams));
        StpCmdCount++;
        return true;
    }
    else
    {
        return false;
    }
}
//=================================================================================================
//! Stops the current stepper motor
/*! 
*/
void CpldStepperStop(void)
{
    if (State != eCPLD_STEP_IDLE)
    {
        State = eCPLD_MOTOR_STOP;
    }
}
//=================================================================================================
//! Returns Stepper motor status
/*! 
*/
uint8_t CpldGetStepper(eStepperType stepper)
{
    if (SpiRxBuffer[MOTOR_CMD_INDEX] & (1<<stepper))
    {
        return 1;
    }
    else
    {
        return 0;
    }
}

//=================================================================================================
//! Fills in the stepper command register
/*! 
*/
static void stepperCmd(void)
{
    tStepperParams * pStpCmd = &StpCmdBuf[(StpCmdPos-StpCmdCount) & CMD_BUF_MASK];
    uint16_t speed;    
    uint8_t direction;
    
    StpCmdCount--;

    speed = 10000/pStpCmd->speed;
    // Find out direction bit
    if (pStpCmd->steps < 0)
    {
        direction = 0;
        pStpCmd->steps = -pStpCmd->steps;
    }
    else
    {
        direction = 1;
    }
    SpiTxBuffer[MOTOR_CMD_INDEX] = pStpCmd->type + 1;
    SpiTxBuffer[MOTOR_CMD_INDEX] += ((direction+1)<<4);
    SpiTxBuffer[MOTOR_SPEED_INDEX] = (speed & 0xFF00)>>8;
    SpiTxBuffer[MOTOR_SPEED_INDEX+1] = (speed & 0xFF);
    SpiTxBuffer[MOTOR_STEPS_INDEX] = (pStpCmd->steps & 0xFF00)>>8;
    SpiTxBuffer[MOTOR_STEPS_INDEX+1] = (pStpCmd->steps & 0xFF);
    StpMonitor |= (1<<pStpCmd->type);
    bStpAckVar[pStpCmd->type] = pStpCmd->bAckVar;
}

//=================================================================================================
//! Fills in solenoid register
/*! 
*/
void CpldSolenoid(eSolenoidType type, eOnOffAction action)
{
    uint8_t index;
    uint8_t pwm;
    if (type == eDRAWER_SOL)
    {
        index = EJECT_SOLENOID_INDEX;
        LoggerSend(eDEVICE_STATE, "s1,%d\r\n", action);
        (action == eON_ACTION) ? (pwm = TestSolMetrics[eDRAWER_SOL].pwm) : (pwm = 0);    
    }
    else
    {
        index = ELECTROMAGNET_INDEX;
        LoggerSend(eDEVICE_STATE,"m1,%d\r\n", action);
        (action == eON_ACTION) ? (pwm = TestSolMetrics[eELECTROMAGNET_SOL].pwm) : (pwm = 0);
    }        
    SpiTxBuffer[index] = pwm;
}

//=================================================================================================
//! Gets solenoid status
/*! 
*/
uint8_t CpldGetSolenoid(eSolenoidType type)
{
    uint8_t index;

    (type == eDRAWER_SOL) ? (index = EJECT_SOLENOID_INDEX) : (index = ELECTROMAGNET_INDEX);
    if (SpiTxBuffer[index] != 0)
    {
        return 1;
    }
    else
    {
        return 0;
    }
}

//=================================================================================================
//! Sets the Peltier PWM
/*! 
*/
void CpldPeltierSetPWM(ePeltierType peltier, uint8_t dutyCycle)
{
    uint8_t offset = 0;

    if (peltier == ePCR_PELTIER)
    {
        offset = 1;
    }
    else if (peltier == eDETECT_PELTIER)
    {
        offset = 0;
    }
    else if (peltier == eLYSIS_PELTIER)
    {
        offset = 3;
    }
    SpiTxBuffer[PELTIER_PWM_START_INDEX + offset] = dutyCycle;
}

//=================================================================================================
//! Gets the peltier temperature and current
/*! 
*/
void CpldPeltierGetAdcValues(ePeltierType peltier, tPeltierParams * pPeltier)
{
    uint8_t index = PELTIER_CURRENT_INDEX;
    if (peltier == ePCR_PELTIER)
    {
        index += 10;
    }
    else if (peltier == eDETECT_PELTIER)
    {
        index += 6;
    }
    else if (peltier == eLYSIS_PELTIER)
    {
        index += 4;
    }
    // Get current adc values
    pPeltier->CurrentActual = ((SpiRxBuffer[index]<<8) | SpiRxBuffer[index+1]);
    pPeltier->CurrentActual &= 0xFFF;
    
    // Get top plate temperature adc values
    pPeltier->topPlateActual = CpldPeltierGetTopPlate(peltier);
    pPeltier->bottomPlateActual = CpldPeltierGetBottomPlate(peltier);
}

//=================================================================================================
//! Gets the peltier top plate temperature
/*! 
*/
uint16_t CpldPeltierGetTopPlate(ePeltierType peltier)
{
    uint16_t temperature;
    uint8_t index = PELTIER_TEMP_INDEX;
    uint8_t offset = 0;

    if (peltier == ePCR_PELTIER)
    {
        offset = 1;
    }
    else if (peltier == eDETECT_PELTIER)
    {
        offset = 2;
    }
    else
    {
        offset = 0;
    }
    index += offset*4;
    
    temperature =  ((SpiRxBuffer[index]<<8) | SpiRxBuffer[index+1]);
    temperature &= 0x0FFF;
    return temperature;
}
//=================================================================================================
//! Gets the peltier bottom plate temperature
/*! 
*/
uint16_t CpldPeltierGetBottomPlate(ePeltierType peltier)
{
    uint16_t temperature;
    uint8_t index = PELTIER_TEMP_INDEX;
    uint8_t offset = 0;
    
    if (peltier == ePCR_PELTIER)
    {
        offset = 1;
    }
    else if (peltier == eDETECT_PELTIER)
    {
        offset = 2;
    }
    else
    {
        offset = 0;
    }
    index += offset*4 + 2;
    
    temperature =  ((SpiRxBuffer[index]<<8) | SpiRxBuffer[index+1]);
    temperature &= 0xFFF;

    return temperature;

}

//=================================================================================================
//! Sets heating or cooling direction for the peltier
/*! CPLD handles inserting the dead time when switching between heating and cooling
*/
void CpldPeltierHeatCool(ePeltierType peltier, bool bIsHeating)
{
    if (bIsHeating)
    {
        SpiTxBuffer[PELTIER_CONTROL_INDEX] |= (1<< (4 + peltier));
    }
    else 
    {
        SpiTxBuffer[PELTIER_CONTROL_INDEX] &= ~(1<< (4 + peltier));
    }
}

//=================================================================================================
//! Turns fan on/off
/*! 
*/
void CpldPeltierFan(ePeltierType peltier, bool bIsOn)
{
    uint8_t bitOffset = 0;
    // Map fan peltiers to fan position
    if (peltier == ePCR_PELTIER)
    {
        bitOffset = 2;
    }
    else if (peltier == eDETECT_PELTIER)
    {
        bitOffset = 1;
    }
    else if (peltier == eLYSIS_PELTIER)
    {
        bitOffset = 0;
    }
    else 
    {
        bitOffset = 3;
    }
    if (bIsOn)
    {
        SpiTxBuffer[PELTIER_CONTROL_INDEX] |= (1<<bitOffset);
    }
    else 
    {
        SpiTxBuffer[PELTIER_CONTROL_INDEX] &= ~(1<<bitOffset);
    }
}
//=================================================================================================
//! Returns the array index for a pump type
/*! 
*/
static uint8_t getPumpIndex(ePumpType pump)
{
    uint8_t index;

    if (pump == eVACUUM_PUMP)
    {
        index = VACUUM_PUMP_INDEX;
    }
    else if (pump == ePRESSURE_PUMP)
    {
        index = PRESSURE_PUMP_INDEX;
    }
    return index;
}

//=================================================================================================
//! Set/Resets test metrics to default values
/*! 
*/
void CpldSetStepperDefaultMetrics(void)
{
    memcpy(&TestStepperMetrics, &DefaultStepperMetrics, sizeof(DefaultStepperMetrics));
}

//=================================================================================================
//! Returns pointer to solenoid metrics
/*! 
*/
tMotorMetrics * CpldGetStepperMetrics(eMetricType metric)
{
    if (metric == eDEFAULT_METRIC)
    {
        return &DefaultStepperMetrics;
    }
    else
    {
        return &TestStepperMetrics;
    }
}

//=================================================================================================
//! Set/Resets test metrics to default values
/*! 
*/
void CpldSetSolDefaultMetrics(void)
{
    memcpy(&TestSolMetrics, &DefaultSolMetrics, sizeof(DefaultSolMetrics));
}

//=================================================================================================
//! Returns pointer to solenoid metrics
/*! 
*/
tMotorMetrics * CpldGetSolMetrics(eMetricType metric, eSolenoidType sol)
{
    if (metric == eDEFAULT_METRIC)
    {
        return &DefaultSolMetrics[sol];
    }
    else
    {
        return &TestSolMetrics[sol];
    }
}

//=================================================================================================
//! Turns everything off in case of an error
/*! 
*/
void CpldAbortRun(void)
{
    CpldSetPump(eVACUUM_PUMP, 0);
    CpldSetPump(ePRESSURE_PUMP, 0);
    CpldPeltierSetPWM(ePCR_PELTIER, 0);
    CpldPeltierSetPWM(eDETECT_PELTIER, 0);
    CpldPeltierSetPWM(eLYSIS_PELTIER, 0);
    CpldStepperStop();
}
//=================================================================================================
//! Returns the thermal board cpld version
/*! 
*/
uint8_t CpldGetThermalVersion(void)
{
    return SpiRxBuffer[THERMAL_VERSION_INDEX];
}
//=================================================================================================
//! Returns the power board cpld version
/*! 
*/
uint8_t CpldGetPowerVersion(void)
{
    return SpiRxBuffer[POWER_VERSION_INDEX];
}
