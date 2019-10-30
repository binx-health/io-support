/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#ifndef _LOGGER_H
#define _LOGGER_H

#ifdef __cplusplus
extern "C" {
#endif

#include <stdint.h>

typedef enum {
    eLOGGING_TEXT,
    eTHROW_TEXT,
    eDEVICE_STATE,
    ePEAK_TEXT,
    eTICK_COUNT,
    eUART_DEBUG
}eLogType;

void LoggerSend(eLogType, char * pString, ...);

#ifdef __cplusplus
};

#endif

#endif
