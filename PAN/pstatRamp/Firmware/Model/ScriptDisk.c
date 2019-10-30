/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/
/**
\file ScriptDisk.c
\brief Script Disk module for storing and retrieving script contents
*/
#include <string.h>
#include <stdbool.h>
#include "ScriptDisk.h"
#include "util.h"

typedef struct {
    char filename[MAX_FILENAME_SIZE];        // Filename
    uint16_t length;                            // size of file
    uint16_t offset;                            // Start offset in array
}tFileTable;

typedef enum {
    eIGNORE_WHITESPACE,
    eSEARCH_COMMENT,
    eIGNORE_DATA,
    eSTORE_DATA,
}eDiskState;

static tFileTable FileTable[MAX_NUM_SCRIPT];
static uint8_t ScriptBuffer[SCRIPT_BUFFER_SIZE];

static eDiskState State;
static uint16_t EndOffset;
static uint8_t FileIndex;
static uint16_t FileLength;

//=================================================================================================
//! Init 
/*! 
*/
void ScriptDiskInit(void)
{
    EndOffset = 0;              // Start at beginning of buffer
    FileIndex = 0;
    State = eIGNORE_WHITESPACE;
    memset(FileTable, 0, sizeof(FileTable));
}

//=================================================================================================
//! Stores data byte in buffer 
/*! 
*/
void ScriptDiskStoreData(uint8_t dataByte)
{    
    if (State == eIGNORE_WHITESPACE)
    {
        if (dataByte != ' ' && dataByte != '\t')
        {
            State = eSEARCH_COMMENT;
        }
    }
    if (State == eSEARCH_COMMENT)
    {
        if (dataByte != '#')
        {
            State = eSTORE_DATA;
        }
        else
        {
            State = eIGNORE_DATA;
        }
    }
    if (State == eIGNORE_DATA)
    {
        if (dataByte == '\n')
        {
            State = eSTORE_DATA;        // Set to store data to save \n
        }
    }
    if (State == eSTORE_DATA)
    {
        if (dataByte == '#')			// Check for inline comment
        {
            State = eIGNORE_DATA;
        }
        else
        {
            // Check for tab and convert to space
            if (dataByte == '\t')
            {
                dataByte = 0x20;
            }
            if (dataByte != '\r')       // Ignore Carriage Return
            {
                ScriptBuffer[EndOffset++] = dataByte;
                FileLength++;
            }
            if (dataByte == '\n')
            {
                State = eIGNORE_WHITESPACE;
            }
        }
    }
}
//=================================================================================================
//! resets script disk state to ignore whitespace
/*! 
*/
void ScriptDiskResetState(void)
{
    State = eIGNORE_WHITESPACE;
}

//=================================================================================================
//! writes to the file allocation table, called before storing file data
/*! NOTE: This Function HAS to be called at the beginning of file data for 
    setting correct offset, length and State variable
*/
void ScriptDiskWriteFileTable(char * pFilename)
{
    strcpy(FileTable[FileIndex].filename, (const char *)pFilename);
    FileTable[FileIndex].offset = EndOffset;
    FileLength = 0;
}
//=================================================================================================
//! Writes the file length to the allocation table, after comments have been removed
/*! Called at end of file
    NOTE: This function HAS to be called at the end of the file to set the correct length
*/
void ScriptDiskSetFileLength(void)
{
    ScriptBuffer[EndOffset++] = '\n';
    FileLength++;
    FileTable[FileIndex++].length = FileLength;       
    
}

//=================================================================================================
//! Returns number of free bytes in script buffer 
/*! 
*/
uint16_t ScriptDiskGetFreeSpace(char * pName)
{
    int32_t space = SCRIPT_BUFFER_SIZE - EndOffset;

    if (space > 0)
    {
        return space;
    }
    else
    {
        return 0;
    }
}
//=================================================================================================
//! Checks if scripts already exists
/*! returns true if script name already exists
*/
bool ScriptDiskIsDuplicate(char * pName)
{
    return ScriptDiskGetId(pName).isValid;
}
//=================================================================================================
//! Checks for empty slots in FileTable
/*! returns true if script name already exists
*/
bool ScriptDiskHasEmptySlots(void)
{
    if (FileIndex >= MAX_NUM_SCRIPT)
    {
        return false;
    }
    else
    {
        return true;
    }
}

//=================================================================================================
//! Returns the absolute base offset for a given script
/*! 
*/
uint16_t ScriptDiskGetOffset(uint8_t scriptId)
{
    return FileTable[scriptId].offset;
}

//=================================================================================================
//! Returns a pointer to the contents at offset
/*! 
*/
uint8_t * ScriptDiskGetLine(uint16_t offset)
{
    return &ScriptBuffer[offset];
}

//=================================================================================================
//! Returns the script id, which is the file index in File Table for a given script
/*! 
*/
tIndexId ScriptDiskGetId(char * scriptName)
{
    tIndexId index;
    uint8_t i;

    index.isValid = false;
    for (i=0; i<MAX_NUM_SCRIPT; i++)
    {
        if (UtilStrcmpI(scriptName, FileTable[i].filename) == 0)
        {
            index.id = i;
            index.isValid = true;
            break;
        }
    }
    return index;
}

//=================================================================================================
//! Returns the size of a script
/*!
*/
uint16_t ScriptDiskGetSize(int8_t scriptId)
{
    return FileTable[scriptId].length;
}

//=================================================================================================
//! Returns pointer to the name of a script
/*!
*/
char * ScriptDiskGetName(int8_t scriptId)
{
    return FileTable[scriptId].filename;
}
