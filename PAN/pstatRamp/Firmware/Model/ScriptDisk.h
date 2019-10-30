/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#ifndef _SCRIPT_DISK_H
#define _SCRIPT_DISK_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdint.h>

#define MAX_FILENAME_SIZE     (64)
#ifdef DEV_BOARD
#define SCRIPT_BUFFER_SIZE          (12*1024)       // Allocate 10k for script buffer
#else
#define SCRIPT_BUFFER_SIZE          (56*1024)       // Allocate 56k for script buffer
#endif  
#define MAX_NUM_SCRIPT              (100)

typedef struct {
    int8_t id;
    bool isValid;
}tIndexId;

void ScriptDiskInit(void);
void ScriptDiskStoreData(uint8_t dataByte);
uint16_t ScriptDiskGetFreeSpace(char * pName);
bool ScriptDiskIsDuplicate(char * pName);
uint16_t ScriptDiskGetSize(int8_t scriptId);
void ScriptDiskWriteFileTable(char * pFilename);
void ScriptDiskResetState(void);
void ScriptDiskSetFileLength(void);
tIndexId ScriptDiskGetId(char * scriptName);
uint8_t * ScriptDiskGetLine(uint16_t offset);
uint16_t ScriptDiskGetOffset(uint8_t scriptId);
char * ScriptDiskGetName(int8_t scriptId);
bool ScriptDiskHasEmptySlots(void);

#ifdef __cplusplus
};
#endif


#endif
