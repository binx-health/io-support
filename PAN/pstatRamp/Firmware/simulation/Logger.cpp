#include "stdafx.h"
#include "Logger.h"
#include <stdio.h>
#include <stdarg.h>

extern "C" { void LoggerInit(void); }

static void sendString(char * string, ...);

#define OUTPUT_FILE         "./Scripts/output.txt"

//============================================================================
//! Init
/*! 
*/
void LoggerInit(void)
{
    // Create new output file
    FILE * fp;

    fp = fopen(OUTPUT_FILE, "w");
    if (fp) {
        fclose(fp);
    } 
}

//============================================================================
//! send log back to PC (*L command)
/*! 
*/
void LoggerSend(eLogType logType, char * pString, ...)
{
    FILE * fp;
    va_list args;
    va_start(args, pString);
    char buf[256];
    if (logType == eLOGGING_TEXT)
    {
        sprintf(buf, "*L=");
    }
    else if (logType == eTHROW_TEXT)
    {
        sprintf(buf, "*T=");
    }
    else if (logType == eDEVICE_STATE)
    {
        sprintf(buf, "*H=*");
    }
    else if (logType == ePEAK_TEXT)
    {
        sprintf(buf, "*P=");
    }
    vsprintf(&buf[3], pString, args);
    va_end(args);
    printf(buf);
    fp = fopen(OUTPUT_FILE, "a");
    if (fp) {
        fprintf(fp, "%s", buf);
        fclose(fp);
    }
}