/*! \file
\brief Command handler module, handles commands received over the serial port

*/
#include <stdio.h>
#include <stdint.h>

#include "cd_class.h"
#include "version.h"
#include "logger.h"
#include "ScriptEngine.h"
#include "Controller.h"
#include "drawer.h"
#include "command.h"

//-----------------------------------------------------------------------------
// Defines
//-----------------------------------------------------------------------------
#define DATA_TIMEOUT            10000       // Timeout when waiting for script data
#define SCRIPT_CHUNK_SIZE       (4096)      // Script chunk size in bytes

typedef enum {
    eCMD_DEFAULT_METRICS,
    eCMD_TEST_METRICS,
    eNO_METRICS
}eCommandMode;
static bool bShutdownCmdReceived = false;
static char StringBuf[MAX_FILENAME_SIZE];
static uint16_t CmdTimer = 0;
static uint32_t ExpectedBytes = 0;
static bool FileTableWritten = false;
static eCommandMode CommandMode = eNO_METRICS;
//-----------------------------------------------------------------------------
// Local function prototypes
//-----------------------------------------------------------------------------
static uint8_t getChar(void);
static uint8_t getString(void);
static uint32_t getDecimal32(void);
static void hardwareStatus(void);

//=================================================================================================
//! Processes command from the serial port
/*! Typically command is in the form *P=xxx\n
*/
void CmdHandler(void)
{
    uint8_t c;
    uint32_t val;

    if (UsbCdcIsDataMode())
    {        
        if (CommandMode == eCMD_TEST_METRICS || CommandMode == eCMD_DEFAULT_METRICS)
        {
            if (UsbCdcRxCount() >= ExpectedBytes)
            {
                if (CommandMode == eCMD_TEST_METRICS)
                {
                    CtrlInitTestMetrics();
                }
                while(UsbCdcRxCount() > 0)
                {
                    if(getString() != 0)
                    { 
                        val = getDecimal32();                    
                        if (CommandMode == eCMD_TEST_METRICS)
                        {
                            CtrlSetMetric(eTEST_METRIC, StringBuf, val);
                        }
                        else
                        {
                            CtrlSetMetric(eDEFAULT_METRIC, StringBuf, val);                           
                        }
                    }
                }
                if (CommandMode != eCMD_TEST_METRICS)
                {
                    // Set test metrics to default vaules
                    CtrlInitTestMetrics(); 
                }
                UsbCdcSetDataMode(false);
                FileTableWritten = false;
                CommandMode = eNO_METRICS;
                printf("*A\r\n");
            }
        }
        // Check for the expected bytes
        else if (UsbCdcRxCount() >= ExpectedBytes)
        {
            if (FileTableWritten == false)
            {
                ScriptDiskWriteFileTable(StringBuf);
                FileTableWritten = true;
            }
            
            for (uint32_t i=0; i<ExpectedBytes; i++)
            {
                ScriptDiskStoreData(UsbCdcGetChar());
            }
            
            ScriptDiskSetFileLength();
            UsbCdcSetDataMode(false);
            printf("*A\r\n");
        }
        // Check for a script chunk
        else if (UsbCdcRxCount() == SCRIPT_CHUNK_SIZE)
        {
            if (FileTableWritten == false)
            {
                ScriptDiskWriteFileTable(StringBuf);
                FileTableWritten = true;
            }
            
            for (uint32_t i=0; i<SCRIPT_CHUNK_SIZE; i++)
            {
                ScriptDiskStoreData(UsbCdcGetChar());
            }

            ExpectedBytes -= SCRIPT_CHUNK_SIZE;

            printf("*A\r\n");
        }
    }
    else
    {
        if (UsbCdcRxLineCount() != 0)
        {
            c = getChar();
            // if we receive just a carriage return 
            if (c != '*')
            {
                return;
            }

            c = getChar();

            switch (c)
            { 
            case 'b':
            case 'B':
                printf("*B=%d\r\n", CpldGetThermalVersion());
                break;
            case 'c':
            case 'C':
                printf("*C=%d\r\n", CpldGetPowerVersion());
                break;
            case 'd':
            case 'D':
                ScriptEngineInit();
                printf("*A\r\n");
                break;
            case 'e':
            case 'E':
                if (getChar() == '=')
                {
                    if (getString())
                    {
                        if (CtrlGetState() == eEXE_SCRIPT_CTRL)
                        {
                            printf("*Z\r\n");
                        }
                        else
                        {
                            if (CtrlScriptExecute(StringBuf))
                            {                                
                                printf("*A\r\n");
                            }
                            else
                            {
                                printf("*N\r\n");
                            }
                        }
                    }
                }
                break;
            case 'H':
            case 'h':
                // Acknowledge command
                printf("*A\r\n");
                // Return status of all hardware devices
                hardwareStatus();
                // Start checking CPLD Spi comms
                CpldCheckComms();
                break;
            case 'M':
            case 'm':
                if (getChar() == '=')
                {
                    val = getDecimal32();
                    CommandMode = eCMD_TEST_METRICS;
                    ExpectedBytes = val;
                    FileTableWritten = false;
                    CmdTimer = DATA_TIMEOUT;
                    UsbCdcSetDataMode(true);
                    printf("*A\r\n");
                }
                break;
            case 'Q':
            case 'q':
                if (getChar() == '=')
                {
                    val = getDecimal32();
                    CommandMode = eCMD_DEFAULT_METRICS;
                    ExpectedBytes = val;
                    FileTableWritten = false;
                    CmdTimer = DATA_TIMEOUT;
                    UsbCdcSetDataMode(true);
                    printf("*A\r\n");
                }
                break;
            case 's':
            case 'S':
                if (getChar() == '=')
                {
                    if (getString())
                    {
                        val = getDecimal32();
                        if (CtrlGetState() == eEXE_SCRIPT_CTRL)
                        {
                            printf("*Z\r\n");
                        }
                        else
                        {
                            if (!ScriptDiskIsDuplicate(StringBuf) && ScriptDiskHasEmptySlots())
                            {
                                if (val < ScriptDiskGetFreeSpace(StringBuf))
                                {
                                    ExpectedBytes = val;
                                    FileTableWritten = false;
                                    CmdTimer = DATA_TIMEOUT;
                                    CommandMode = eNO_METRICS;
                                    UsbCdcSetDataMode(true);
                                    // Reset state machine to ignore white space
                                    ScriptDiskResetState();
                                    printf("*A\r\n");
                                }
                                else
                                {
                                    printf("*N\r\n");
                                }
                            }
                            else
                            {
                                printf("*N\r\n");
                            }
                        }
                    }
                }
                break;
            case 'v':
            case 'V':
                printf("*V=%s\r\n", SVN_BUILD_VERSION);
                break;
            case 'x':
            case 'X':
                CtrlAbortExecute();                
                printf("*A\r\n");
                break;
            case 'y':
            case 'Y':
                bShutdownCmdReceived = true;
                printf("*A\r\n");
                break;
            default:
                printf("*N\r\n");
                break;
            }// End switch
        }
    }
}
//=================================================================================================
//! Returns status of bCmdReceived flag
/*! 
*/
bool ShutdownCmdReceived(void)
{
    return bShutdownCmdReceived;
}       
//=================================================================================================
//! Checks for timeout while waiting for script data
/*! 
*/
void CmdTick(void)
{
    if ((--CmdTimer == 0) && UsbCdcIsDataMode())
    {
        CmdTimer = DATA_TIMEOUT;
        UsbCdcResetBuffer();
        printf("*N\r\n");
        LoggerSend(eTHROW_TEXT, "Expected: %d Rcvd: %d Bytes", ExpectedBytes, UsbCdcRxCount());
    }
}
//=================================================================================================
//! Get number from serial port
/*! Reads characters from the serial port, converting them to a 32bit unsigned
decimal number. Halts when a character other than '0' or '9' is received
\return rval uint32_t number
*/
static uint32_t getDecimal32(void)
{
    uint32_t rval;
    int8_t c;

    rval = 0;
    while (TRUE)
    {
        c = UsbCdcGetChar();
        if (c == ' ' && rval == 0)              // Ignore leading spaces
        {
            continue;
        }
        if ((c < '0') || (c > '9'))
        {
            break;
        }
        rval = (rval * 10) + (c - '0');
    }
    return rval;
}
//=================================================================================================
//! Get the character from the serial port
/*! Gets the next non white space character from serial port, converted to uppercase
\return c 
*/
static uint8_t getChar(void) {
    uint8_t c;
    do
    {
        c = UsbCdcGetChar();
    } while ((c == ' ') || (c == '\t'));
    return c;
}
//=================================================================================================
//! Get a string from serial port
/*! Halts when a space, comma or carriage return is seen    
*/
static uint8_t getString(void)
{
    int8_t c;
    uint8_t index = 0;

    memset(StringBuf, 0, sizeof(StringBuf));
    while (true)
    {
        c = UsbCdcGetChar();
        if ((c == '\n') || (c == ',') || (c == ' '))
        {
            break;
        }
        else
        {
            if (index < MAX_FILENAME_SIZE-1)
            {
                StringBuf[index++] = c;
            }
        }        
    }
    return index;
}
//=================================================================================================
//! Returns the hardware status 
/*! 
*/
static void hardwareStatus(void)
{
    uint8_t i;

    for (i=0; i< eLAST_VALVE; i++)
    {
        LoggerSend(eDEVICE_STATE,"v%d,%d\r\n", i+1, CpldGetValve((eValveType)i));
    }
    LoggerSend(eDEVICE_STATE,"p1,%d\r\n", CpldGetPump(eVACUUM_PUMP));
    LoggerSend(eDEVICE_STATE,"p2,%d\r\n", CpldGetPump(ePRESSURE_PUMP));
    LoggerSend(eDEVICE_STATE,"s1,%d\r\n", CpldGetSolenoid(eDRAWER_SOL));
    LoggerSend(eDEVICE_STATE,"m1,%d\r\n", CpldGetSolenoid(eELECTROMAGNET_SOL));
    LoggerSend(eDEVICE_STATE,"dpr1,%d\r\n", ResGetDpr1());
    for (i=0; i<eLAST_STEPPER; i++)
    {
        LoggerSend(eDEVICE_STATE,"l%d,%d\r\n", i+1, CpldGetStepper((eStepperType)i));
    }
    for (i=0; i<eLAST_RES; i++)
    {
        LoggerSend(eDEVICE_STATE,"ps%d,%d\r\n", i+1, ResReadPressure((eReservoirType)i));
    }
    for (i=0; i<eLAST_PELTIER; i++)
    {
        tPeltierParams * pParams;
        pParams = PeltierGetParams((ePeltierType)i);
        LoggerSend(eDEVICE_STATE,"therm%d,%d\r\n", i+1, pParams->mode);
        LoggerSend(eDEVICE_STATE,"therm%d.top,%d\r\n", i+1, CpldPeltierGetTopPlate((ePeltierType)i));
        LoggerSend(eDEVICE_STATE,"therm%d.bottom,%d\r\n", i+1, CpldPeltierGetBottomPlate((ePeltierType)i));
    }
    for (i=0; i<ePSTAT_LAST_CHANNEL; i++)
    {
        LoggerSend(eDEVICE_STATE,"pstat%d,%d\r\n", i+1, ioPstatFluidDetect((ePstatAdcChannel)i));
    }        
    for (i=0; i<eOPTO_LAST; i++)
    {
        LoggerSend(eDEVICE_STATE,"opto%d,%d\r\n", i+1, DrawerGetOptoStatus((eOptoType)i));
    }
}
