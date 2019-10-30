#include "stm32f10x.h"
#include <stdio.h>
#include "drawer.h"
#include "barcode.h"
#include "logger.h"
#include "cpld.h"
#include "controller.h"

#define OPTO_DRAWER_FRONT   eOPTO_1
#define OPTO_DRAWER_REAR    eOPTO_2

#define DEBOUNCE_TIMEOUT    100                 // 1 second debounce for closed drawer

//#define M1A_BUILD

#ifdef M1A_BUILD
#define OPTO_DARK       0
#define OPTO_LIGHT      1
#else
#define OPTO_DARK       1
#define OPTO_LIGHT      0
#endif

#ifdef DEV_BOARD
    #define FORCE_DRAWER_CLOSED
#endif

static eDrawerState State;
static bool bCheckInvalid;
static uint8_t DebounceCount;

//=================================================================================================
//! Init
/*! 
*/
void DrawerInit(void)
{
    GPIO_InitTypeDef GPIO_InitStructure;

    RCC_APB2PeriphClockCmd(RCC_APB2Periph_GPIOC, ENABLE);
    GPIO_InitStructure.GPIO_Pin = GPIO_Pin_0 | GPIO_Pin_1;
    GPIO_InitStructure.GPIO_Speed = GPIO_Speed_50MHz;
    GPIO_InitStructure.GPIO_Mode = GPIO_Mode_IN_FLOATING;
    GPIO_Init(GPIOC, &GPIO_InitStructure);

    State = eUNKNOWN_POS_DRAWER;
    // Check what state drawer is at power up
    if ((DrawerGetOptoStatus(OPTO_DRAWER_FRONT) == OPTO_DARK) && 
        (DrawerGetOptoStatus(OPTO_DRAWER_REAR) == OPTO_LIGHT))
    {
        State = eCLOSED_DRAWER;
    }
    else if ((DrawerGetOptoStatus(OPTO_DRAWER_FRONT) == OPTO_LIGHT) && 
             (DrawerGetOptoStatus(OPTO_DRAWER_REAR) == OPTO_DARK))
    {
        State = eOPENED_DRAWER;
    }
    bCheckInvalid = true;
    DebounceCount = 0;
}

//=================================================================================================
//! 10ms tick to monitor drawer open/closed status
/*! 
*/
void DrawerTick10Ms(void)
{
    switch(State)
    {
    case eUNKNOWN_POS_DRAWER:
        if ((DrawerGetOptoStatus(OPTO_DRAWER_FRONT) == OPTO_LIGHT)  &&
            (DrawerGetOptoStatus(OPTO_DRAWER_REAR) == OPTO_DARK))
        {
            if (DebounceCount++ > DEBOUNCE_TIMEOUT) 
            {
                // Check barcode data and notify PC
                if (BarcodeHasData())
                {
                    LoggerSend(eTHROW_TEXT, "10001, %s\r\n", BarcodeGetString());
                    BarcodeClearStatus();
                }
                else
                {
                    LoggerSend(eTHROW_TEXT, "20001\r\n");
                }
                LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", OPTO_DRAWER_FRONT+1, DrawerGetOptoStatus(OPTO_DRAWER_FRONT));
                LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", OPTO_DRAWER_REAR+1, DrawerGetOptoStatus(OPTO_DRAWER_REAR));
                BarcodeTriggerOff();
                State = eCLOSED_DRAWER;
            }
        }
        else if ((DrawerGetOptoStatus(OPTO_DRAWER_FRONT) == OPTO_DARK) &&
            (DrawerGetOptoStatus(OPTO_DRAWER_REAR) == OPTO_LIGHT))
        {
            BarcodeTriggerOn();
            LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", OPTO_DRAWER_FRONT+1, DrawerGetOptoStatus(OPTO_DRAWER_FRONT));
            LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", OPTO_DRAWER_REAR+1, DrawerGetOptoStatus(OPTO_DRAWER_REAR));
            State = eOPENED_DRAWER;
            DebounceCount = 0;
        }
        else
        {
            DebounceCount = 0;
        }
        if (bCheckInvalid)
        {
            bCheckInvalid = false;
            if ((DrawerGetOptoStatus(OPTO_DRAWER_FRONT) == OPTO_LIGHT) &&
                (DrawerGetOptoStatus(OPTO_DRAWER_REAR) == OPTO_LIGHT))
            {
                BarcodeTriggerOff();
                // Drawer not present or optos failed
                LoggerSend(eTHROW_TEXT, "30005, Drawer Error\r\n");                
            }
        }
        break;
    case eOPENED_DRAWER:
        if (DrawerGetOptoStatus(OPTO_DRAWER_REAR) == OPTO_DARK ||
            DrawerGetOptoStatus(OPTO_DRAWER_FRONT) == OPTO_LIGHT )
        {
            LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", OPTO_DRAWER_FRONT+1, DrawerGetOptoStatus(OPTO_DRAWER_FRONT));
            LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", OPTO_DRAWER_REAR+1, DrawerGetOptoStatus(OPTO_DRAWER_REAR));
            bCheckInvalid = true;
            State = eUNKNOWN_POS_DRAWER;
        }
        break;
    case eCLOSED_DRAWER:
        if (DrawerGetOptoStatus(OPTO_DRAWER_FRONT) == OPTO_DARK ||
            DrawerGetOptoStatus(OPTO_DRAWER_REAR) == OPTO_LIGHT)
        {
            eStepperType i;
            LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", OPTO_DRAWER_FRONT+1, DrawerGetOptoStatus(OPTO_DRAWER_FRONT));
            LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", OPTO_DRAWER_REAR+1, DrawerGetOptoStatus(OPTO_DRAWER_REAR));
            bCheckInvalid = true;
            State = eUNKNOWN_POS_DRAWER;
            // Check steppers are not moving when drawer is not closed anymore                       
            for (i=eL1_STEPPER; i<eLAST_STEPPER; i++)
            {
                if (CpldGetStepper(i) == 1)
                {
                    LoggerSend(eTHROW_TEXT,"30002, Drawer not closed\r\n");
                    CtrlAbortExecute();  
                    break;
                }
            }            
        }
        break;
    }
}

//=================================================================================================
//! Returns opto status
/*! 
*/
uint8_t DrawerGetOptoStatus(eOptoType type)
{
    uint16_t pin = 1<<(uint16_t)type;
#ifdef FORCE_DRAWER_CLOSED
    if (type == OPTO_DRAWER_FRONT)
    {
        return OPTO_LIGHT;
    }
    else if (OPTO_DRAWER_REAR == type)
    {
        return OPTO_DARK;
    }
#endif
    return GPIO_ReadInputDataBit(GPIOC, pin);
}

//=================================================================================================
//! Returns drawer state
/*! 
*/
eDrawerState DrawerGetState(void)
{
    return State;
}
