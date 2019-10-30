#include "stm32f10x.h"
#include <stdio.h>
#include "emagnet.h"
#include "logger.h"
#include "controller.h"

#define EMAGNET_OPTO_ID     3
#ifndef M1A_BUILD
#define DISK_PRESENT        1
#define DISK_NOT_PRESENT    0
#else
#define DISK_PRESENT        0
#define DISK_NOT_PRESENT    1
#endif

static uint8_t ElecMagnetStatus;
static bool bDiskReleased;

static uint8_t getStatus(void);

//=================================================================================================
//! Init
/*! 
*/
void EMagnetInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;

    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_2;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
    GPIO_Init(GPIOC, &GPIO_InitStructure);
    ElecMagnetStatus = DISK_NOT_PRESENT;
    bDiskReleased = false;
}

//=================================================================================================
//! 10ms tick to monitor electro magnet status
/*! 
*/
void EMagnetTick10Ms(void)
{
    // Monitor electromangnet opto status and send notification if changed
    if (ElecMagnetStatus != getStatus())
    {
        ElecMagnetStatus = getStatus();
        LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", EMAGNET_OPTO_ID, ElecMagnetStatus);
        if (ElecMagnetStatus == DISK_PRESENT)
        {
            bDiskReleased = false; 
        }
        else
        {
            if (!bDiskReleased && CpldGetStepper(eL1_STEPPER) != 1)
            {
                LoggerSend(eTHROW_TEXT,"30003, Isolation Disc error\r\n");
                CtrlAbortExecute();
            }
        }          
    }
}

//=================================================================================================
//! Returns opto status
/*! 
*/
static uint8_t getStatus(void)
{
    return GPIO_ReadInputDataBit(GPIOC, GPIO_Pin_2);
}

//=================================================================================================
//! Returns opto status
/*! 
*/
void EMagnetDiskReleased(void)
{
    bDiskReleased = true;
}
