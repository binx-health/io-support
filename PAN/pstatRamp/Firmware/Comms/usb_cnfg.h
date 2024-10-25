/*************************************************************************
 *
 *    Used with ICCARM and AARM.
 *
 *    (c) Copyright IAR Systems 2007
 *
 *    File name   : usb_cnfg.h
 *    Description : USB config file
 *
 *    History :
 *    1. Date        : June 16, 2007
 *       Author      : Stanimir Bonev
 *       Description : Create
 *
 *    $Revision: 34964 $
 **************************************************************************/

#include <includes.h>

#ifndef __USB_CNFG_H
#define __USB_CNFG_H

/* The Board */
#define IAR_STM32_SK

/* USB High Speed support*/
#define USB_HIGH_SPEED                  0

/* USB interrupt priority */
#define USB_INTR_HIGH_PRIORITY          1
#define USB_INTR_LOW_PRIORITY           2

/* USB Events */
#define USB_SOF_EVENT                   0
#define USB_ERROR_EVENT                 0   // for debug
#define USB_HIGH_PRIORITY_EVENT         1   // ISO and Double buffered bulk
#define USB_PMAOVR_EVENT                0   // for debug

/* USB Clock settings */
#define USB_DIVIDER                     RCC_USBCLKSource_PLLCLK_1Div5 // when PLL clk 72MHz

/* Device power atrb  */
#define USB_SELF_POWERED                0
#define USB_REMOTE_WAKEUP               0

/* Max Interfaces number*/
#define USB_MAX_INTERFACE               1

/* Endpoint definitions */
#define MaxIndOfRealizeEp               ENP3_IN   // be careful this is very important const
#define Ep0MaxSize                      8

#define ReportEp                        ENP1_IN
#define ReportEpMaxSize                 8

#define CommOutEp                       ENP2_OUT
#define CommOutEpMaxSize                64

#define CommInEp                        ENP3_IN
#define CommInEpMaxSize                 64

/* Class defenitions*/
#define CDC_DEVICE_SUPPORT_LINE_CODING  0
#define CDC_DEVICE_SUPPORT_LINE_STATE   0

#define CDC_DEVICE_SUPPORT_BREAK        0
#define CDC_BRK_TIMER_INTR_PRI          3

#define CDC_DATA_RATE                   CBR_9600
#define CDC_DATA_BITS                   8
#define CDC_PARITY                      NOPARITY
#define CDC_STOP_BITS                   ONESTOPBIT

#define CDC_LINE_DTR                    1
#define CDC_LINE_RTS                    1

/* Other defenitions */

#endif //__USB_CNFG_H
