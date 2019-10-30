/*************************************************************************
*
*    Used with ICCARM and AARM.
*
*    (c) Copyright IAR Systems 2006
*
*    File name   : cd_class.c
*    Description : Communication device class module
*
*    History :
*    1. Date        : June 28, 2006
*       Author      : Stanimir Bonev
*       Description : Create
*
*    $Revision: 34964 $
**************************************************************************/
#define CD_CLASS_GLOBAL
#include <stdio.h>
#include "cd_class.h"

#pragma data_alignment=4
static CDC_LineCoding_t CDC_LineCoding;

#pragma data_alignment=4
static UsbSetupPacket_t CdcReqPacket;

static volatile Boolean CDC_Configure;
static volatile Boolean TxDone;

#pragma data_alignment=4
static Int8U UsbRxBuffer[CommOutEpMaxSize];
static Int8U UsbTxBuffer[CommOutEpMaxSize];

#define RX_BUFFER_SIZE      (4096)
#ifdef DEV_BOARD      
#define TX_BUFFER_SIZE      (2048)
#else
#define TX_BUFFER_SIZE      (4096)
#endif

#define RX_BUFFER_MASK      (RX_BUFFER_SIZE-1)
#define TX_BUFFER_MASK      (TX_BUFFER_SIZE-1)

#define LINE_FEED       10
#define CR              13

#if ( RX_BUFFER_SIZE & RX_BUFFER_MASK )
    #error RX buffer size is not a power of 2
#endif
#if ( TX_BUFFER_SIZE & TX_BUFFER_MASK )
    #error TX buffer size is not a power of 2
#endif

#define USB_ENABLE_INT      (USB_CNTR |= bmCTRM)
#define USB_DISABLE_INT     (USB_CNTR &= ~bmCTRM)

static volatile uint16_t TxPos;
static volatile uint16_t TxCount;
static volatile uint16_t RxPos;
static volatile uint16_t RxCount;
static volatile uint16_t DataBytes;
static volatile uint16_t RxLineCount;

static bool bIsDataMode;

static uint8_t RxBuffer[RX_BUFFER_SIZE];
static uint8_t TxBuffer[TX_BUFFER_SIZE];

static void storeRxChar(uint8_t rxChar);
static void storeToRxBuffer(uint8_t rxChar);

/*************************************************************************
* Function Name: UsbCdcInit
* Parameters: none
*
* Return: none
*
* Description: USB communication device class init
*
*************************************************************************/
void UsbCdcInit (void)
{
    // Init CD Class variables
    CDC_Configure               = FALSE;
    UsbCdcResetBuffer();
    CDC_LineCoding.dwDTERate    = CDC_DATA_RATE;
    CDC_LineCoding.bDataBits    = CDC_DATA_BITS;
    CDC_LineCoding.bParityType  = CDC_PARITY;
    CDC_LineCoding.bCharFormat  = CDC_STOP_BITS;

    UsbCdcConfigure(NULL);
    UsbCoreInit();
    bIsDataMode = false;
}

/*************************************************************************
* Function Name: UsbCdcConfigure
* Parameters:  pUsbDevCtrl_t pDev
*
* Return: none
*
* Description: USB communication device class configure
*
*************************************************************************/
void UsbCdcConfigure (pUsbDevCtrl_t pDev)
{
    if(pDev != NULL && pDev->Configuration)
    {
        CDC_Configure = TRUE;      
        USB_IO_Data(CommOutEp,UsbRxBuffer,sizeof(UsbRxBuffer),(void*)UsbCdcReadHandler);
    }
    else
    {
        CDC_Configure = FALSE;
    }
    TxDone = true;
}

/*************************************************************************
* Function Name: IsUsbCdcConfigure
* Parameters:  none
*
* Return: Boolean
*
* Description: Return configuration state
*
*************************************************************************/
Boolean IsUsbCdcConfigure (void)
{
    return(CDC_Configure);
}

/*************************************************************************
* Function Name: UsbCdcRequest
* Parameters:  pUsbSetupPacket_t pSetup
*
* Return: UsbCommStatus_t
*
* Description: The class requests processing
*
*************************************************************************/
UsbCommStatus_t UsbCdcRequest (pUsbSetupPacket_t pSetup)
{
    CdcReqPacket = *pSetup;
    // Validate Request
    if (CdcReqPacket.mRequestType.Recipient == UsbRecipientInterface)
    {
        switch (CdcReqPacket.bRequest)
        {
        case SET_LINE_CODING:
            if ((CdcReqPacket.wValue.Word == 0) &&
                (CdcReqPacket.wIndex.Word == 0))
            {
                USB_IO_Data(CTRL_ENP_OUT,
                    (pInt8U)&CDC_LineCoding,
                    USB_T9_Size(sizeof(CDC_LineCoding_t),CdcReqPacket.wLength.Word),
                    (void *)UsbCdcData);
                return(UsbPass);
            }
            break;
        case SET_CONTROL_LINE_STATE:
          if ((CdcReqPacket.wLength.Word == 0) &&
              (CdcReqPacket.wIndex.Word == 0))
          {
            //CDC_LineState.DTR_State = ((CdcReqPacket.wValue.Lo & 1) != 0);
            //CDC_LineState.RTS_State = ((CdcReqPacket.wValue.Lo & 2) != 0);
            // Send AKN
            USB_StatusHandler(CTRL_ENP_IN);
            //LineStateDelta = TRUE;
            return(UsbPass);
          }
          break;
        case GET_LINE_CODING:
            if ((CdcReqPacket.wValue.Word == 0) &&
                (CdcReqPacket.wIndex.Word == 0))
            {
                USB_IO_Data(CTRL_ENP_IN,
                    (pInt8U)&CDC_LineCoding,
                    USB_T9_Size(sizeof(CDC_LineCoding_t),CdcReqPacket.wLength.Word),
                    (void *)USB_StatusHandler);
                return(UsbPass);
            }
            break;
        }
    }
    return(UsbFault);
}

/*************************************************************************
* Function Name: UsbCdcData
* Parameters:  USB_Endpoint_t EP
*
* Return: none
*
* Description: USB Communication Device Class Data receive
*
*************************************************************************/
void UsbCdcData (USB_Endpoint_t EP)
{
    if ((CdcReqPacket.bRequest == SET_LINE_CODING) &&
        (CdcReqPacket.wLength.Word != 0))
    {
        USB_StatusHandler(CTRL_ENP_IN);
    }
    USB_T9_ERROR_REQUEST();
}

/*************************************************************************
* Function Name: UsbCdcReadHandler
* Parameters: USB_Endpoint_t EP
*
* Return: none
*
* Description: Called when receive buffer is filled or error appear
*
*************************************************************************/
static void UsbCdcReadHandler (USB_Endpoint_t EP)
{
    assert(EP == CommOutEp);
    for (uint32_t i=0; i<EpCnfg[EP].Size; i++)
    {
        storeRxChar(UsbRxBuffer[i]);
    }
    USB_IO_Data(CommOutEp,UsbRxBuffer,sizeof(UsbRxBuffer),(void*)UsbCdcReadHandler);
}

/*************************************************************************
* Function Name: UsbCdcWriteHandler
* Parameters: USB_Endpoint_t EP
*
* Return: none
*
* Description: Called when transmitting buffer is empty or error appear
*
*************************************************************************/
static void UsbCdcWriteHandler (USB_Endpoint_t EP)
{
    assert(EP == CommInEp);
    TxDone = TRUE;
}
//=================================================================================================
//! Returns the Rx and Tx buffers
/*! 
*/
void UsbCdcResetBuffer(void)
{
    RxPos = RxCount = 0;
    TxPos = TxCount = 0;
    RxLineCount = 0;
    DataBytes = 0;
    bIsDataMode = false;
}
//=================================================================================================
//! Stores a character in the received circular buffer
/*! 
*/
static void storeRxChar(uint8_t rxChar)
{
    if (RxCount < RX_BUFFER_SIZE)
    {
        if (bIsDataMode)
        {
            storeToRxBuffer(rxChar);
        }
        else
        {            
            if (rxChar != CR )
            {
                storeToRxBuffer(rxChar);
            }
            if (rxChar == LINE_FEED)
            {
                RxLineCount++;                
            }
        }
    }
}

//=================================================================================================
//! Returns number of CR in buffer
/*! 
*/
uint16_t UsbCdcRxLineCount(void)
{
    return RxLineCount;

}

//=================================================================================================
//! Returns number of bytes in buffer
/*! 
*/
uint16_t UsbCdcRxCount(void)
{
    return DataBytes;  
}

//=================================================================================================
//! Returns number of bytes in TxBuffer
/*! 
*/
uint16_t UsbCdcTxCount(void)
{
    return TxCount;
}
//=================================================================================================
//! Returns the next character from the command buffer
/*! 
*/
uint8_t UsbCdcGetChar(void)
{    
    uint16_t tmpCount = RxCount;
    uint8_t b;
    
    if (RxCount ==0 )
    {
        b = 0;
    }
    else
    {
        ENTR_CRT_SECTION();
        b = RxBuffer[(RxPos - tmpCount) & RX_BUFFER_MASK];
        RxCount--;
        if (!bIsDataMode)
        {
            if (b == LINE_FEED && RxLineCount > 0)
            {
                RxLineCount--;
            }
        }
        else
        {
            DataBytes--;
        }
        EXT_CRT_SECTION();
    }
    return b;
}
//=================================================================================================
//! Stores byte into transmit buffer
/*! 
*/
void UsbCdcTxChar(uint8_t dataByte)
{
    if (TxCount != TX_BUFFER_SIZE)
    {
        ENTR_CRT_SECTION();
        TxBuffer[TxPos++] = dataByte;
        TxPos &= TX_BUFFER_MASK;
        TxCount++;
        EXT_CRT_SECTION();
    }
}
//=================================================================================================
//! Returns true if in data mode
/*! 
*/
bool UsbCdcIsDataMode(void)
{
    return bIsDataMode;
}
//=================================================================================================
//! Sets the data mode flag
/*! 
*/
void UsbCdcSetDataMode(bool dataMode)
{
    uint16_t tmpRxPos = RxPos;
    uint16_t tmpRxCount = RxCount; 
    bIsDataMode = dataMode;
    if (dataMode)
    {
        DataBytes = 0;  
    }
    else
    {
        // Resync line count in case extra carriage return was inserted after data
        ENTR_CRT_SECTION();
        RxCount = 0;
        RxPos = 0;
        RxLineCount = 0;
        EXT_CRT_SECTION();
    }
}
//=================================================================================================
//! Sends data in Tx buffer on USB IN Endpoint. Called from the main loop
/*! 
*/
void UsbCdcTxBufferSend(void)
{
    uint32_t size = MIN(TxCount, CommOutEpMaxSize);  
    uint16_t tmpPos = TxPos;  

    if (size == 64)
    {
        size--;
    }
    if (TxCount > 0 && TxDone)
    {
        TxDone = false;        
        ENTR_CRT_SECTION();
        for (uint8_t i=0; i<size; i++)
        {
            UsbTxBuffer[i] = TxBuffer[(tmpPos-TxCount) & TX_BUFFER_MASK];
            TxCount--;
        }
        USB_IO_Data(CommInEp,UsbTxBuffer, size, (void*)UsbCdcWriteHandler);
        EXT_CRT_SECTION();
    }    

}

//=================================================================================================
//! Retargets the C library printf function to the UART
/*!
*/
int putchar(int ch)
{
    UsbCdcTxChar(ch);
    return ch;
}

//=================================================================================================
//! Stores a byte in the Rx circular buffer
/*!
*/
static void storeToRxBuffer(uint8_t rxChar)
{
    RxBuffer[RxPos++] = rxChar;
    RxPos &= RX_BUFFER_MASK;
    RxCount++;
    DataBytes++;
}
