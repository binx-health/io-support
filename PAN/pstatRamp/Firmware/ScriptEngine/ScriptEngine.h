/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#ifndef _SCRIPT_ENGINE_H
#define _SCRIPT_ENGINE_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdint.h>
#include <stdbool.h>
#include "ScriptDisk.h"

#define MAX_NUM_FIBRES      (10)
#define MAX_STACK_FRAME     (10)
#define MAX_VARIABLES_NUM   (50)

void ScriptEngineInit(void);
void ScriptEngineHandler(void);
bool ScriptEngineExecute(char * scriptName);
uint8_t ScriptEngineNumFibres(void);
bool ScriptEngineParseCommand(char * pCmd, uint8_t fibreId);
tIndexId ScriptEngineDevNameLookup(const char * pName);
tIndexId ScriptEngineMapNameLookup(const char * pName);
tIndexId ScriptEngineDescNameLookup(const char * pName);
void ScriptEngineMapName(char * pDevice, char * mappedName);
void ScriptEngineDescName(char * pDevice, char * descName);
char * ScriptEngineGetMapName(char * hdwDevice);
char * ScriptEngineGetDescName(char * hdwDevice);
tIndexId ScriptEngineSetVariable(char * pName, bool setReset);
tIndexId ScriptEngineGetVariable(char * pName, bool * value);
bool * ScriptEngineGetVarAddr(char * pName);
void ScriptEngineTick(void);
uint16_t ScriptEngineGetTickCount(void);
char * ScriptEngineExeName(void);
bool ScriptEngineCheckName(char * scriptName);

#ifdef __cplusplus
};
#endif

#endif
