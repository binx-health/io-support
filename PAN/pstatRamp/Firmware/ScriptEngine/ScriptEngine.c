/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/
/**
\file ScriptEngine.c
\brief Script Engine module
*/
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "Controller.h"
#include "ScriptEngine.h"
#include "Logger.h"
#include "util.h"


#define DEV_STRING_LEN_MAX  (64)        // Maximum string length, set t0 64 which should be enough
#define MAX_PARAMS_NUM      (10)        // Maximum number of parameters, 10 should be enough


typedef enum {
    eNO_COMMAND,                            // Nothing to do
    eWAIT_FLAG_SET,                         // Wait on a flag
    eWAIT_FLAG_TIMEOUT,                     // Wait on flag with timeout
    eWAIT_TIMER,                            // Wait for elapsed time
    ePARSING_IF,                            // Executing if block, until else or endif is found
    eSEARCH_ELSE_ENDIF,                     // Ignore if block, until else or endif is found
    ePARSING_ELSE,                          // Executing else block, until endif is found
    eSEARCH_ENDIF,                          // Ignore block until endif is found
}eAction;

typedef struct {
    uint16_t lineNumber;                    // Current line number
    uint16_t memOffset;                     // Memory offset
}tLineInfo;

typedef struct {
    uint32_t ticksWait;                     // Number of 1ms tick to wait
    bool * pAckVariable;                    // pointer to Ack variable used in wait command
    bool * pBeginVariable;                  // pointer to ack variable used in begin command
    bool bAtomicThead;                      // boolean if already in fibre atomic thread
    tLineInfo startLine;                    // start line in file for stack frame
    tLineInfo currentLine;                  // current line in file
    uint16_t size;                          // length of file
    int32_t repeatCount;                    // number of repeat for begin command
    eAction action;                         // type of action being executed
    int8_t scriptId;                        // script id for stack frame
    uint8_t beginCount;                     // Count for nested begin
    uint8_t ifCount;                        // Count for nested if
}tStackFrame;

typedef struct {
    tStackFrame stack[MAX_STACK_FRAME];     // array of stack frame for fibre
    int8_t stackId;                         // active stack id for fibre
}tFibreInfo;

typedef struct {
    char * name;                            // name of device as referenced from scripts
    uint16_t deviceId;                      // device id
}tDeviceDesc;

//=================================================================================================
// Hardware device names and id
static const tDeviceDesc DeviceIdTable[] = {
    {"v1", eV1_VALVE}, {"v2" , eV2_VALVE}, {"v3", eV3_VALVE}, {"v4", eV4_VALVE},
    {"v5", eV5_VALVE}, {"v6" , eV6_VALVE}, {"v7", eV7_VALVE},         // Cartridge valves
    {"v8", eV8_VALVE}, {"v9", eV9_VALVE}, {"v10" , eV10_VALVE},       // Cartridge blow line valves
    {"v11", eV11_VALVE},                            // Vacuum dump valve
    {"v12", eV12_VALVE},                            // Vacuum enable valve
    {"v13", eV13_VALVE},                            // Low-high pressure select valve
    {"v14" , eV14_VALVE},                           // Low pressure dump valve
    {"v15", eV15_VALVE},                            // High pressure dump valve
    {"v16", eV16_VALVE},                            // Low pressure enable valve
    {"v17", eV17_VALVE},                            // Spare valve
    {"v18", eV18_VALVE},                            // Spare valve
    {"v19", eV19_VALVE},                            // Spare valve
    {"v20", eV20_VALVE},                            // Spare valve
    {"p1", ePRESSURE_PUMP},                         // Pressure pump
    {"p2", eVACUUM_PUMP},                           // Vacuum pump
    {"s1", eDRAWER_SOL},                            // Drawer catch solenoid
    {"m1", eELECTROMAGNET_SOL},                     // Electro magnet
    {"dpr1", eDIG_PRESSREG_VALVE},                  // Digital pressure regulator valve
    {"l1", eL1_STEPPER},                            // Cartridge clamp stepper motors
    {"l2", eL2_STEPPER},                            // Elute blister stepper motor
    {"l3", eL4_STEPPER},                            // Lyse blister stepper motor
    {"l4", eL3_STEPPER},                            // Wash blister stepper motor
    {"l5", eL5_STEPPER},                            // Mechanical valve stepper motor
    {"ps1", eP1_SENSOR},                            // Vacuum pressure sensor (relative)
    {"ps2", eP2_SENSOR},                            // Low pressure sensor (relative)
    {"ps3", eP3_SENSOR},                            // High pressure sensor (relative)
    {"ps4", eP4_SENSOR},                            // Atmospheric pressure sensor (absolute)
    {"therm1", eLYSIS_PELTIER},                     // Lysis peltier
    {"therm2", ePCR_PELTIER},                       // PCR peltier
    {"therm3", eDETECT_PELTIER},                    // Detection peltier
    {"pstat1", ePSTAT1}, {"pstat2", ePSTAT2}, {"pstat3", ePSTAT3}, {"pstat4", ePSTAT4},  
    {"opto1", eOPTO_1},                              // Drawer front opto
    {"opto2", eOPTO_2},                              // Drawer back opto
    {"opto3", eOPTO_3},                              // Isolation valve primed opto
    {"res1", eVACUUM_RES},                          // vacuum reservoir
    {"res2", eLOW_PRESSURE_RES},                    // low pressure reservoir
    {"res3", eHIGH_PRESSURE_RES},                   // high pressure reservoir
};

#define HDW_NUM_DEVICES     (sizeof(DeviceIdTable)/sizeof(tDeviceDesc))


typedef struct {
    char mappedName[DEV_STRING_LEN_MAX];
    char friendlyName[DEV_STRING_LEN_MAX];
}tVirtualDevice;

typedef struct {
    char name[DEV_STRING_LEN_MAX];
    bool bIsSet;
    bool bInUse;
}tVariable;

static char * MappedNameTable[HDW_NUM_DEVICES];
static char * DescNameTable[HDW_NUM_DEVICES];            
static char * VariableTable[MAX_VARIABLES_NUM];         

static tVirtualDevice VirtualDevice[HDW_NUM_DEVICES];   
static tFibreInfo Fibres[MAX_NUM_FIBRES];
static tVariable Variables[MAX_VARIABLES_NUM];
static char PhaseString[DEV_STRING_LEN_MAX];
static char CmdBuf[256];                        // Buffer for storing script line command
static bool bTickFlag;                          // Tick flag set from 1 ms interrupt
static uint8_t NumActiveFibres;

static tIndexId createNewFibre(void);
static int8_t getStackFrameIndex(int8_t fibreId);
static tIndexId lookupTable(const char * pName, char ** pTable, uint8_t numElements);
static bool executeCommands(char ** pCmds, uint8_t numCmds, uint8_t fibreId);
static tIndexId getDeviceIndex(char * pName);
static bool createNewStack(int8_t fibreId, eAction action);
static void destroyStack(tFibreInfo * pStack);
static void setStackInfo(int8_t scriptId, tStackFrame * pStack);


//=================================================================================================
//! Initialise script engine
/*! 
*/
void ScriptEngineInit(void)
{
    uint8_t i, j;

    ScriptDiskInit();

    // Set all stackId and scriptId to -1
    for (i=0; i<MAX_NUM_FIBRES; i++)
    {
        Fibres[i].stackId = -1;
        for (j=0; j<MAX_STACK_FRAME; j++)
        {
            Fibres[i].stack[j].scriptId = -1;
        }
    }

    // Initialise mapped and descriptive pointer table
    for (i=0; i<HDW_NUM_DEVICES; i++) 
    {
        MappedNameTable[i] = &VirtualDevice[i].mappedName[0];
        DescNameTable[i] = &VirtualDevice[i].friendlyName[0];
    }
    for (i=0; i<MAX_VARIABLES_NUM; i++)
    {
        VariableTable[i] = &Variables[i].name[0];
    }

    // Reset all the names
    memset(VirtualDevice, 0, sizeof(VirtualDevice));
    // Reset all variables
    memset(Variables, 0, sizeof(Variables));
    memset(PhaseString, 0, sizeof(PhaseString));
    bTickFlag = false;
    NumActiveFibres = 0;
}

//=================================================================================================
//! Returns number of fibres
/*! 
*/
uint8_t ScriptEngineNumFibres(void)
{
    return NumActiveFibres;
}
//=================================================================================================
//! Check for a valid script name
/*! 
*/
bool ScriptEngineCheckName(char * scriptName)
{
    tIndexId index =  ScriptDiskGetId(scriptName);
    return index.isValid;    
}

//=================================================================================================
//! Start execution of a script
/*! 
*/
bool ScriptEngineExecute(char * scriptName)
{
    int8_t retVal = false;
    tIndexId index = ScriptDiskGetId(scriptName);
    // Look for script ID
    if (index.isValid)
    {
        tIndexId fibreId = createNewFibre();
        if (fibreId.isValid)
        {
            int8_t stackId;
            if (createNewStack(fibreId.id, eNO_COMMAND))
            {
                stackId = Fibres[fibreId.id].stackId;
                setStackInfo(index.id, &Fibres[fibreId.id].stack[stackId]);
                retVal = true;
            }
        }
    }
    return retVal;
}

//=================================================================================================
//! Called from the main loop to execute fibres/ scripts
/*! 
*/
void ScriptEngineHandler(void)
{
    uint16_t j;
    uint8_t i;

    for (i=0; i<MAX_NUM_FIBRES; i++)
    {
        int8_t stackId = Fibres[i].stackId;
        if (stackId != -1)
        {
            uint16_t currentOffset;
            uint8_t len = 0;
            currentOffset = Fibres[i].stack[stackId].currentLine.memOffset - 
                Fibres[i].stack[stackId].startLine.memOffset;
            
            if (Fibres[i].stack[stackId].action == eWAIT_TIMER || 
                Fibres[i].stack[stackId].action == eWAIT_FLAG_TIMEOUT)
            {
                if (bTickFlag)
                {
                    if(Fibres[i].stack[stackId].ticksWait-- == 0)
                    {
                        Fibres[i].stack[stackId].action = eNO_COMMAND;
                    }
                }
                if (Fibres[i].stack[stackId].pAckVariable != NULL && 
                    Fibres[i].stack[stackId].action != eWAIT_TIMER)
                {
                    if (*Fibres[i].stack[stackId].pAckVariable)
                    {
                        Fibres[i].stack[stackId].action = eNO_COMMAND;
                    }
                }
            }
            else
            {
                // Check for end of file and destroy stack if necessary
                if (currentOffset > Fibres[i].stack[stackId].size)
                {               
                    destroyStack(&Fibres[i]);
                }
                else
                {
                    uint8_t * pBuf = ScriptDiskGetLine(Fibres[i].stack[stackId].currentLine.memOffset);
                    for (j=0; j<sizeof(CmdBuf); j++)
                    {
                        CmdBuf[j] = pBuf[j];
                        len++;
                        currentOffset++;

                        if (pBuf[j] == '\n' || currentOffset > Fibres[i].stack[stackId].size)
                        {
                            Fibres[i].stack[stackId].currentLine.memOffset +=len;
                            Fibres[i].stack[stackId].currentLine.lineNumber++;
                            // Null terminate string
                            CmdBuf[j] = '\0';
                            // Check if there are some text to parse first
                            if (len >= 3)
                            {
                                // parse and execute command
                                // if script error - reinitialise everything
                                if (!ScriptEngineParseCommand(CmdBuf, i))
                                {                            
                                    LoggerSend(eTHROW_TEXT, "30001,%d,%s\r\n",                                         
                                        Fibres[i].stack[stackId].currentLine.lineNumber-1,
                                        ScriptDiskGetName(Fibres[i].stack[Fibres[i].stackId].scriptId));
                                    CtrlAbortExecute();
                                    ScriptEngineInit();
                                } 
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
    // Check if tick flag needs to be cleared
    if (bTickFlag)
    {
        bTickFlag = false;
    }
}

//=================================================================================================
//! Returns a free fibre, stackId is -1 for a free fibre
/*! 
*/
static tIndexId createNewFibre(void)
{
    tIndexId retVal = { 0, false};
    uint8_t i;
    for (i=0; i<MAX_NUM_FIBRES; i++)
    {
        if (Fibres[i].stackId == -1)
        {
            retVal.id = i;
            retVal.isValid = true;
            break;
        }  
    }
    if (!retVal.isValid)
    {
        LoggerSend(eTHROW_TEXT, "Error: Max Fibre exceeded\r\n");
    }
    else
    {
        NumActiveFibres++;
    }
    return retVal;
}

//=================================================================================================
//! Returns a free stack frame, -1 if no free fibre stack
/*! 
*/
static int8_t getStackFrameIndex(int8_t fibreId)
{
    int8_t retVal = -1;
    uint8_t i;
    for (i=0; i<MAX_STACK_FRAME; i++)
    {
        if (Fibres[fibreId].stack[i].scriptId == -1)
        {
            retVal = i;
            break;
        }
    }
    return retVal;
}

//=================================================================================================
//! Parses the command, returns false if command could not be parsed
/*! 
*/
bool ScriptEngineParseCommand(char * pLine, uint8_t fibreId)
{
    bool retVal = true;
    uint8_t index = 1;
    char * pCmd[MAX_PARAMS_NUM];

    // Check if it is a throw command
    if (UtilStrncmpI(pLine, "throw ", 6) == 0)
    {
        uint16_t errVal;
        LoggerSend(eTHROW_TEXT, "%s\r\n", &pLine[6]);
        // Check if throw message is an error
        pCmd[0] = strtok(pLine, " ");
        if (pCmd[0] != NULL)
        {
            pCmd[1] = strtok(NULL, " ");
            errVal = atoi(pCmd[1]);
            if (errVal >= 30000)
            {
                CtrlAbortExecute();
            }
        }
        return true;
    }
    // Check for peak command
    if (UtilStrncmpI(pLine, "peak ", 5) == 0)
    {
        LoggerSend(ePEAK_TEXT, "%s\r\n", &pLine[5]);
        return true;
    }        
    pCmd[0] = strtok(pLine, " ");
    // split line into tokens
    if (pCmd[0] != NULL)
    {
        while (1) 
        {
            pCmd[index] = strtok(NULL, " ");
            if (pCmd[index] == NULL)
            {
                break;
            }
            else
            {
                index++;
                if (index >= MAX_PARAMS_NUM)
                {
                    retVal = false;
                    break;
                }
            }
        }
    }
    // Look up commands from table
    if (retVal)
    {
        retVal = executeCommands(pCmd, index, fibreId);
    }

    return retVal;
}

//=================================================================================================
//! Lookup DeviceName table and return matching name index
/*! tIndexId.isValid == false if no matching name found
*/
tIndexId ScriptEngineDevNameLookup(const char * pName)
{
    tIndexId index;
    uint8_t i;
    index.isValid = false;

    for (i=0; i<HDW_NUM_DEVICES; i++)
    {
        if (UtilStrcmpI(DeviceIdTable[i].name, (char *)pName) == 0)
        {
            index.id = i;
            index.isValid = true;
            break;
        }
    }
    return index;
}

//=================================================================================================
//! Lookup MappedName table and return matching name index
/*! 
*/
tIndexId ScriptEngineMapNameLookup(const char * pName)
{
    return (lookupTable(pName, MappedNameTable, HDW_NUM_DEVICES));
}

//=================================================================================================
//! Lookup DescriptiveName table and return matching name index
/*! 
*/
tIndexId ScriptEngineDescNameLookup(const char * pName)
{
    tIndexId index = lookupTable(pName, DescNameTable, HDW_NUM_DEVICES);
    // n/c and - are used for no entry in descriptive name
    // Check for those and return -1 if n/c or - found
    if (index.isValid)
    {
        if (UtilStrcmpI(DescNameTable[index.id], "n/c") == 0 || 
            strcmp(DescNameTable[index.id], "-") == 0 )
        {
            index.isValid = false;
        }
    }
    return (index);
}

//=================================================================================================
//! Lookup  table and return matching name
/*! 
*/
static tIndexId lookupTable(const char * pName, char ** pTable, uint8_t numElements)
{
    tIndexId index;
    uint8_t i;
    index.isValid = false;
    for (i=0; i<numElements; i++)
    {
        if (UtilStrcmpI(pTable[i], (char *)pName) == 0)
        {
            index.id = i;
            index.isValid = true;
            break;
        }
    }
    return index;
}


//=================================================================================================
//! Inserts an entry in the mapped name virtual device table
/*! 
*/
void ScriptEngineMapName(char * pDevice, char * mappedName)
{
    char * pTable = VirtualDevice[ScriptEngineDevNameLookup(pDevice).id].mappedName;    
    strcpy(pTable, mappedName);
}

//=================================================================================================
//! Inserts an entry in the descriptive name virtual device table
/*! 
*/
void ScriptEngineDescName(char * pDevice, char * descName)
{
    char * pTable = VirtualDevice[ScriptEngineDevNameLookup(pDevice).id].friendlyName;    
    strcpy(pTable, descName);
}

//=================================================================================================
//! Returns the mapped device for a hardware device
/*! 
*/
char * ScriptEngineGetMapName(char * hdwDevice)
{
    tIndexId index = ScriptEngineDevNameLookup(hdwDevice);
    return (index.isValid) ? VirtualDevice[index.id].mappedName : NULL;
}

//=================================================================================================
//! Returns the mapped device for a hardware device
/*! 
*/
char * ScriptEngineGetDescName(char * hdwDevice)
{
    tIndexId index = ScriptEngineDevNameLookup(hdwDevice);
    return (index.isValid) ? VirtualDevice[index.id].friendlyName : NULL;
}

//=================================================================================================
//! Sets the variable
/*! 
*/
tIndexId ScriptEngineSetVariable(char * pName, bool setReset)
{
    tIndexId retVal = { 0, false};
    tIndexId index = lookupTable(pName, VariableTable, MAX_VARIABLES_NUM);
    uint8_t i;
    if (strlen(pName) < DEV_STRING_LEN_MAX)
    {
        if (index.isValid)
        {
            Variables[index.id].bIsSet = setReset;
            strcpy(&Variables[index.id].name[0], pName);
        }
        else
        {
            // Find empty entry
            for (i=0; i<MAX_VARIABLES_NUM; i++)
            {
                if (!Variables[i].bInUse)
                {
                    Variables[i].bInUse = true;
                    Variables[i].bIsSet = setReset;
                    strcpy(&Variables[i].name[0], pName);
                    index.id = i;
                    index.isValid = true;
                    break;
                }
            }
        }
        retVal = index;
    }
    return retVal;
}

//=================================================================================================
//! returns the address where the variable boolean is 
/*! returns NULL if no matching variable found, used mainly for unit testing            
*/
bool * ScriptEngineGetVarAddr(char * pName)
{
    tIndexId index = lookupTable(pName, VariableTable, MAX_VARIABLES_NUM);
    if (index.isValid)
    {
        return &Variables[index.id].bIsSet;
    }
    else
    {
        return NULL;    
    }
}

//=================================================================================================
//! returns the index of a script variable
/*!
*/
tIndexId ScriptEngineGetVariable(char * pName, bool * value)
{
    tIndexId index = lookupTable(pName, VariableTable, MAX_VARIABLES_NUM);
    if (index.isValid)
    {
        *value = Variables[index.id].bIsSet;
    }
    return index;
}

//=================================================================================================
//! Executes the command
/*! Returns false if there was an error in the command syntax
*/
static bool executeCommands(char ** pCmd, uint8_t numCmds, uint8_t fibreId)
{
    bool retVal = false;
    tIndexId index;
      
    tStackFrame * pCurrentStack = &Fibres[fibreId].stack[Fibres[fibreId].stackId];
    int8_t curStackId = Fibres[fibreId].stackId;

    if (pCurrentStack->action == eSEARCH_ELSE_ENDIF || 
        pCurrentStack->action == eSEARCH_ENDIF)
    {
        if (UtilStrcmpI(pCmd[0], "if") == 0)
        {
            pCurrentStack->ifCount++;
        }
        if (UtilStrcmpI(pCmd[0], "else") != 0 && UtilStrcmpI(pCmd[0], "endif") != 0)
        {
            return true;
        }
    }
    if (pCurrentStack->bAtomicThead)
    {
        if (UtilStrcmpI(pCmd[0], "begin") == 0)
        {
            pCurrentStack->beginCount++;
        }
        if (UtilStrcmpI(pCmd[0], "end") != 0)
        {
            return true;
        }
    }
    if (UtilStrcmpI(pCmd[0], "map") == 0)
    {
        if (numCmds == 3)
        {
            if (ScriptEngineDevNameLookup(pCmd[1]).isValid)
            {
                if (strlen(pCmd[2]) < DEV_STRING_LEN_MAX) 
                {
                    ScriptEngineMapName(pCmd[1], pCmd[2]);
                    retVal = true;
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "name") == 0)
    {
        if (numCmds == 3)
        {
            if (ScriptEngineDevNameLookup(pCmd[1]).isValid)
            {
                if (strlen(pCmd[2]) < DEV_STRING_LEN_MAX) 
                {
                    ScriptEngineDescName(pCmd[1], pCmd[2]);
                    retVal = true;
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "res") == 0)
    {
        if (numCmds >= 3)
        {
            eReservoirType type;
            tReservoirParams params;
            tIndexId devId = getDeviceIndex(pCmd[1]);

            if (devId.isValid && devId.id >=44 && devId.id <=46)
            {
                type = (eReservoirType)DeviceIdTable[devId.id].deviceId;
                if (UtilStrcmpI(pCmd[2], "hold") == 0 && numCmds >= 6)
                {
                    params.action = eHOLD_RES;
                    params.pressure = atoi(pCmd[3]);
                    params.maxPressure = atoi(pCmd[4]);
                    params.minTime = atoi(pCmd[5]);
                    params.pAckVariable = NULL;
                    if (params.maxPressure > params.pressure)
                    {
                        if (params.minTime > 0 && params.pressure > 0)
                        {
                            if (numCmds == 7)
                            {
                                index = ScriptEngineSetVariable(pCmd[6], false);
                                if (index.isValid)
                                {
                                    params.pAckVariable = &Variables[index.id].bIsSet;
                                }
                            }
                            params.minTime *= 10;
                            CtrlReservoir(type, &params);
                            retVal = true;
                        }
                    }
                }
                else if (UtilStrcmpI(pCmd[2], "dump") == 0)
                {
                    params.action = eDUMP_RES;
                    CtrlReservoir(type, &params);
                    retVal = true;
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "valve") == 0)
    {
        if (numCmds == 3)
        {
            index = getDeviceIndex(pCmd[1]);
            if (index.isValid && (index.id >= 0 && index.id < eLAST_VALVE))
            {
                if (UtilStrcmpI(pCmd[2], "on") == 0)
                {
                    CtrlValve((eValveType)DeviceIdTable[index.id].deviceId, eON_ACTION);
                    retVal = true;
                }
                else if (UtilStrcmpI(pCmd[2], "off") == 0)
                {
                    CtrlValve((eValveType)DeviceIdTable[index.id].deviceId, eOFF_ACTION);
                    retVal = true;
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "delay") == 0)
    {
        if (numCmds == 2)
        {
            uint32_t val = atoi(pCmd[1]);
            if (val > 0)
            {
                pCurrentStack->ticksWait = val * 10;            
                pCurrentStack->action = eWAIT_TIMER;
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "wait") == 0)
    {
        if (numCmds == 3)
        {
        bool bFlag;
        tIndexId varIndex = ScriptEngineGetVariable(pCmd[1], &bFlag);
        if (varIndex.isValid)
        {
            uint32_t val = atoi(pCmd[2]);
            if (val > 0)
            {
                pCurrentStack->ticksWait = val * 1000;
                pCurrentStack->action = eWAIT_FLAG_TIMEOUT;
                pCurrentStack->pAckVariable = &Variables[varIndex.id].bIsSet;
                retVal = true;
            }
        }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "call") == 0)
    {                    
        if (numCmds == 2)
        {
            tIndexId scriptIndex = ScriptDiskGetId(pCmd[1]);            
            if (createNewStack(fibreId, eNO_COMMAND) && scriptIndex.isValid)
            {
                LoggerSend(eLOGGING_TEXT, "Calling %s\r\n", pCmd[1]);
                curStackId = Fibres[fibreId].stackId;
                // Set up new stack info for new script
                setStackInfo(scriptIndex.id, &Fibres[fibreId].stack[curStackId]);
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "begin") == 0)
    {
        if (numCmds >= 3)
        {
            uint16_t repeat = atoi(pCmd[2]);
            tIndexId tmpFibreId = { 0, false };
            tIndexId newFibreId = { 0 , false };

            if (repeat > 0)
            {
                if (UtilStrcmpI(pCmd[1], "atomic") == 0)
                {
                    newFibreId = createNewFibre();
                    if (newFibreId.isValid)
                    {
                        if (createNewStack(newFibreId.id, eNO_COMMAND))
                        {
                            tStackFrame * newStack = &Fibres[newFibreId.id].stack[Fibres[newFibreId.id].stackId];                        
                            memcpy(newStack, pCurrentStack, sizeof(tStackFrame));
                            memcpy(&newStack->startLine, &pCurrentStack->currentLine, sizeof(tLineInfo));
                            pCurrentStack->bAtomicThead = true;
                            newStack->action = eNO_COMMAND;
                            tmpFibreId = newFibreId;
                            retVal = true;
                        }
                    }
                }
                else if (UtilStrcmpI(pCmd[1], "nonatomic") == 0)
                {
                    retVal = createNewStack(fibreId, eNO_COMMAND);
                    tmpFibreId.id = fibreId;
                }
                if (retVal)
                {
                    if (numCmds == 4)
                    {
                        index = ScriptEngineSetVariable(pCmd[3], false);
                        if (!index.isValid)
                        {
                            retVal = false;
                        }
                        else
                        {
                            Fibres[tmpFibreId.id].stack[Fibres[tmpFibreId.id].stackId].pBeginVariable =
                                &Variables[index.id].bIsSet;
                        }
                    }
                    curStackId = Fibres[fibreId].stackId;
                    Fibres[tmpFibreId.id].stack[Fibres[tmpFibreId.id].stackId].repeatCount = repeat-1;
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "end") == 0)
    {
        if (numCmds == 1)
        {
            if (pCurrentStack->bAtomicThead)
            {
                if (pCurrentStack->beginCount == 0)
                {
                    pCurrentStack->bAtomicThead = false;
                }
                else
                {
                    pCurrentStack->beginCount--;
                }
                retVal = true;
            }
            else if (pCurrentStack->repeatCount-- > 0)
            {
                memcpy(&pCurrentStack->currentLine, 
                    &pCurrentStack->startLine, sizeof(tLineInfo)); 
                retVal = true;
            }
            else
            {
                if (curStackId != 0)
                {
                    // Copy line info to parent stack if not at parent level
                    memcpy(&Fibres[fibreId].stack[curStackId-1].currentLine,
                        &pCurrentStack->currentLine, sizeof(tLineInfo));
                }
                if (pCurrentStack->pBeginVariable != NULL)
                {
                    *pCurrentStack->pBeginVariable = true;
                }
                destroyStack(&Fibres[fibreId]);
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "stepper") == 0)
    {
        if ((numCmds == 4 || numCmds == 5))
        {
            index = getDeviceIndex(pCmd[1]);
            if (index.id >= 25 && index.id <=29 && index.isValid)
            {
                tStepperParams params;
                params.speed = atoi(pCmd[2]);
                params.steps = atoi(pCmd[3]);
                params.type = (eStepperType)DeviceIdTable[index.id].deviceId;
                params.bAckVar = NULL;
                if (params.speed != 0 && params.steps != 0)
                {
                    if (numCmds == 5)
                    {
                        tIndexId varIndex = ScriptEngineSetVariable(pCmd[4], false);
                        if (varIndex.isValid)
                        {
                            params.bAckVar = &Variables[varIndex.id].bIsSet;
                        }
                    }
                    retVal = CtrlStepper(&params);
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "solenoid") == 0)
    {
        if (numCmds == 3)
        {
            index = getDeviceIndex(pCmd[1]);
            if ((index.id == 22 || index.id == 23) && index.isValid )
            {
                eSolenoidType type = (eSolenoidType)DeviceIdTable[index.id].deviceId;
                eOnOffAction action;
                if (UtilStrcmpI(pCmd[2], "on") == 0)            
                {
                    action = eON_ACTION;
                    retVal = true;
                }
                else if (UtilStrcmpI(pCmd[2], "off") == 0)
                {
                    action = eOFF_ACTION;
                    retVal = true;
                }
                if (retVal)
                {
                    CtrlSolenoid(type, action);
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "dpr") == 0)
    {
        if (numCmds >= 3 && numCmds < 6)
        {
            index = getDeviceIndex(pCmd[1]);
            if (index.id == 24 && index.isValid)
            {
                tDigPressReg params;
                params.rampPeriod = 0;
                if (UtilStrcmpI(pCmd[2], "on") == 0)
                {
                    params.action = eON_ACTION;
                    if (numCmds >= 4)
                    {
                        params.percent = atoi(pCmd[3]);
                        if (params.percent > 0 || *pCmd[3] == '0')
                        {
                            retVal = true;
                            if (numCmds == 5)
                            {
                                params.rampPeriod = atoi(pCmd[4]) * 10;
                                if (params.rampPeriod == 0 && *pCmd[4] != '0')
                                {
                                    retVal = false;
                                }
                            }
                        }
                    }
                }
                else if (UtilStrcmpI(pCmd[2], "off") == 0)
                {
                    params.action = eOFF_ACTION;
                    retVal = true;
                }
                if (retVal)
                {
                    CtrlDigitalPressReg(&params);
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "if") == 0)
    {
        if (numCmds == 2)
        {
            bool bFlag;
            tIndexId varIndex = ScriptEngineGetVariable(pCmd[1], &bFlag);
            if (varIndex.isValid && createNewStack(fibreId, eNO_COMMAND))
            {
                tStackFrame * pNewStack = &Fibres[fibreId].stack[Fibres[fibreId].stackId];
                if (bFlag)
                {                             
                    pNewStack->action = ePARSING_IF;
                }
                else
                {
                    pNewStack->action = eSEARCH_ELSE_ENDIF;
                }
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "else") == 0)
    {
        if (pCurrentStack->ifCount == 0)
        {
            if (pCurrentStack->action == eSEARCH_ELSE_ENDIF)
            {
                pCurrentStack->action = ePARSING_ELSE;
                retVal = true;
            }
            else if (pCurrentStack->action == ePARSING_IF)
            {
                pCurrentStack->action = eSEARCH_ENDIF;
                retVal = true;
            }
        }
        else
        {
            retVal = true;
        }
    }
    else if (UtilStrcmpI(pCmd[0], "endif") == 0)
    {
        if (pCurrentStack->ifCount == 0)
        {
            if (curStackId > 0)
            {
                // Copy needed information before destroying current stack frame
                memcpy(&Fibres[fibreId].stack[curStackId-1].currentLine, 
                    &pCurrentStack->currentLine, sizeof(tLineInfo));
                destroyStack(&Fibres[fibreId]);
            }
            if (pCurrentStack->action == eSEARCH_ELSE_ENDIF)
            {
                pCurrentStack->action = eNO_COMMAND;
            }
        }
        else
        {
            pCurrentStack->ifCount--;
        }
        retVal = true;
    }
    else if (UtilStrcmpI(pCmd[0], "phase") == 0)
    {
        if (numCmds == 2)
        {
            if (strlen(pCmd[1]) < DEV_STRING_LEN_MAX)
            {
                if (strcmp(PhaseString, pCmd[1]) != 0)
                {
                    strcpy(PhaseString, pCmd[1]);
                    LoggerSend(eTHROW_TEXT, "10000, %s\r\n", pCmd[1]);
                }
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "set") == 0)
    {
        if (numCmds == 3)
        {
            uint32_t val = atoi(pCmd[2]);
            if (val != 0 || *pCmd[2] == '0')
            {
                retVal = CtrlSetMetric(eTEST_METRIC, pCmd[1], val);
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "therm") == 0)
    {
        if ((numCmds == 3 || numCmds >= 4))
        {
            index = getDeviceIndex(pCmd[1]);
            if (index.id >=34 && index.id <=36 && index.isValid)
            {
                ePeltierType type = (ePeltierType)DeviceIdTable[index.id].deviceId;
                tPeltierParams params;
                memset(&params, 0, sizeof(tPeltierParams));
                if (UtilStrcmpI(pCmd[2], "on") == 0)
                {
                    params.mode = ePELTIER_ON;
                    if (numCmds >= 4)
                    {
                        params.topPlateTarget = atoi(pCmd[3]);
                        if (params.topPlateTarget > 0)
                        {
                            retVal = true;
                            // Check for ACK variable
                            if (numCmds == 5)
                            {
                                tIndexId varIndex = ScriptEngineSetVariable(pCmd[4], false);
                                if (varIndex.isValid)
                                {
                                    params.bAckVar = &Variables[varIndex.id].bIsSet;
                                }
                                else
                                {
                                    retVal = false;
                                }
                            }
                        }
                    }
                }
                else if (UtilStrcmpI(pCmd[2], "off") == 0)
                {
                    params.mode = ePELTIER_OFF;
                    params.topPlateTarget = 0;
                    retVal = true;
                }
                if (retVal)
                {
                    CtrlThermal(type, &params, DeviceIdTable[index.id].name);
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "fluid") == 0)
    {
        if (numCmds == 3)
        {
            index = getDeviceIndex(pCmd[1]);
            if (index.id >= 37 && index.id <= 40 && index.isValid)
            {
                ePstatType type = (ePstatType)DeviceIdTable[index.id].deviceId;
                tIndexId varIndex = ScriptEngineSetVariable(pCmd[2], false);
                if (varIndex.isValid)
                {
                    CtrlFluidDetect(type, &Variables[varIndex.id].bIsSet);
                    retVal = true;
                }
            }
        }                
    }
    else if (UtilStrcmpI(pCmd[0], "voltammetry") == 0)
    {
        if (numCmds >= 7)
        {
            tPstatParams params = { 0 };
            eVoltammetryType type;
            uint8_t minNumCmds = 7;
            bool bCmdValid = false;

            tIndexId varIndex = { 0 };
            varIndex.isValid = true;
            params.V1 = atoi(pCmd[2]);
            params.V2 = atoi(pCmd[3]);
            params.V3 = atoi(pCmd[4]);
            params.V4 = atoi(pCmd[5]);
            params.T1 = atoi(pCmd[6]);

            if (UtilStrcmpI(pCmd[1], "squarewave") == 0)
            {
                minNumCmds = 7;
                params.T2 = params.T1;
                type = eSQUARE_WAVE;
                bCmdValid = true;
            }
            else if (UtilStrcmpI(pCmd[1], "differential") == 0 && numCmds >= 8)
            {
                params.T2 = atoi(pCmd[7]);
                minNumCmds = 8;
                type = eDIFF_PULSE;
                bCmdValid = true;
            }
            if (bCmdValid)
            {
                // Check for ACK variable
                if (numCmds == (minNumCmds + 1))
                {
                    varIndex = ScriptEngineSetVariable(pCmd[minNumCmds], false);
                    if (varIndex.isValid)
                    {
                        params.bAckVar = &Variables[varIndex.id].bIsSet;
                    }
                }

                if (varIndex.isValid && params.V2 > 0 && params.V3 > 0 &&
                    params.V4 >= 0 && params.T1 > 0 && params.T2 > 0)
                {
                    CtrlPstatParams(type, &params);
                    retVal = true;
                }
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "buzzer") == 0)
    {
        if (numCmds == 4)
        {
            uint8_t period;
            uint8_t dutyCycle;
            uint8_t runTime;

            period = atoi(pCmd[1]);
            dutyCycle = atoi(pCmd[2]);
            runTime = atoi(pCmd[3]);
            if (period != 0 && dutyCycle != 0 && runTime != 0)
            {
                CtrlBuzzer(period, dutyCycle, runTime);
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "regvalve") == 0) 
    {
        if (numCmds == 2)
        {
            uint16_t dacValue = atoi(pCmd[1]);
            if (dacValue != 0 || *pCmd[1] == '0')
            {
                CtrlSetRegValve(dacValue);
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "preport") == 0) 
    {
        if (numCmds == 2)
        {
            if (UtilStrcmpI(pCmd[1], "on") == 0)
            {
                CtrlPreport(true);
                retVal = true;
            }
            else if (UtilStrcmpI(pCmd[1], "off") == 0)
            {
                CtrlPreport(false);
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "pstat") == 0) 
    {
        if (numCmds == 1)
        {
            CtrlPstatTest();
            retVal = true;
        }
        if (numCmds == 3)
        {
            uint16_t milliVolts = atoi(pCmd[1]);
            bool bInternalLoad = false;
            if ((milliVolts != 0 && milliVolts <= 5000) || *pCmd[1] == '0')
            {
                if (UtilStrcmpI(pCmd[2], "internal") == 0)
                {
                    bInternalLoad = true;
                    retVal = true;
                }
                else if (UtilStrcmpI(pCmd[2], "external") == 0)
                {
                    bInternalLoad = false;
                    retVal = true;
                }            
            }
            if (retVal)
            {
                CtrlPstatControl(milliVolts, bInternalLoad);
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "peltier") == 0) 
    {
        if (numCmds == 2)
        {
            if (UtilStrcmpI(pCmd[1], "heat") == 0)
            {
                CtrlPeltiers(true, true);
                retVal = true;
            }
            else if (UtilStrcmpI(pCmd[1], "cool") == 0)
            {
                CtrlPeltiers(true, false);
                retVal = true;          
            }
            else if (UtilStrcmpI(pCmd[1], "off") == 0)
            {
                CtrlPeltiers(false, false);
                retVal = true;
            }
        }
    }
    else if (UtilStrcmpI(pCmd[0], "psensor") == 0)
    {
        if (numCmds == 1)
        {
            CtrlPsensorTest();
            retVal = true;
        }
    }
    else
    {
        retVal = false;
    }
    return retVal;
}


//=================================================================================================
//! returns the device name index
/*! returns not valid if no matching name from map name or device name is found
*/
static tIndexId getDeviceIndex(char * pName)
{
    tIndexId retVal = {0, false};
    retVal = ScriptEngineMapNameLookup(pName);
    if (!retVal.isValid)
    {
        retVal = ScriptEngineDevNameLookup(pName);
    }
    return retVal;
}

//=================================================================================================
//! Creates a new stack frame for given fibre
/*! returns false if stack frame could not be created
*/
static bool createNewStack(int8_t fibreId, eAction action)
{
    int8_t newStackId = getStackFrameIndex(fibreId);
    int8_t currStackId = Fibres[fibreId].stackId;

    bool retVal = false;

    if (newStackId != -1)
    {
        tStackFrame * pNewStack = &Fibres[fibreId].stack[newStackId];
        tStackFrame * pCurrentStack = &Fibres[fibreId].stack[currStackId];
        if (currStackId != -1)
        {
            // Initialise new stack frame setting correct line number
            memcpy(pNewStack, pCurrentStack, sizeof(tStackFrame));
            memcpy(&pNewStack->startLine, &pCurrentStack->currentLine, sizeof(tLineInfo));
        }
        pNewStack->action = action;
        pNewStack->repeatCount = 0;
        pNewStack->ticksWait = 0;
        pNewStack->pAckVariable = NULL;
        pNewStack->pBeginVariable = NULL;
        pNewStack->bAtomicThead = false;
        pNewStack->beginCount = 0;
        pNewStack->ifCount = 0;

        Fibres[fibreId].stackId = newStackId;
        retVal = true;
    }
    else
    {
        LoggerSend(eTHROW_TEXT, "30002, Cannot create new stack\r\n");
    }
    return retVal;
}

//=================================================================================================
//! Destroys the current stack or fibre if currently at top level
/*! 
*/
static void destroyStack(tFibreInfo * pFibre)
{    
    pFibre->stack[pFibre->stackId].scriptId = -1;
    if (--pFibre->stackId == -1)
    {
        NumActiveFibres--;
    }
}

//=================================================================================================
//! Populates a stack frame from scriptId
/*! 
*/
static void setStackInfo(int8_t scriptId, tStackFrame * pStack)
{
    pStack->scriptId = scriptId;
    pStack->size = ScriptDiskGetSize(scriptId);
    pStack->startLine.lineNumber = 1;
    pStack->startLine.memOffset = ScriptDiskGetOffset(scriptId);
    pStack->currentLine.lineNumber = 1;
    pStack->currentLine.memOffset = ScriptDiskGetOffset(scriptId);
}
//=================================================================================================
//! Set tick flag every 10 ms - called from main loop
/*! 
*/
void ScriptEngineTick(void)
{
    bTickFlag = true;
}
