#ifndef _COMMAND_H
#define _COMMAND_H
#include <stdbool.h>

void CmdHandler(void);
void CmdTick(void);
bool ShutdownCmdReceived(void);

#endif

