/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#include "Logger.h"
#include <stdio.h>
#include <stdarg.h>
#include "uart1.h"

static char LogBuf[64];

//=================================================================================================
//! send log back to PC 
/*! 
*/
void LoggerSend(eLogType logType, char * pString, ...)
{
    va_list args;
    va_start(args, pString);
    if (logType == eUART_DEBUG)
    {
        vsprintf(LogBuf, pString, args);
        Uart1TxString(LogBuf);
    }
    else
    {
        if (logType == eLOGGING_TEXT)
        {
            printf("*L=");
        }
        else if (logType == eTHROW_TEXT)
        {
            printf("*T=");
        }
        else if (logType == eDEVICE_STATE)
        {
            printf("*H=");
        }
        else if (logType == ePEAK_TEXT)
        {
            printf("*P=");
        }
        else if (logType == eTICK_COUNT)
        {
            printf("*I=");
        }
        vprintf(pString, args);
    }
    va_end(args);
}
