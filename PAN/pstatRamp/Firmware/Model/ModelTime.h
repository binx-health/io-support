/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#ifndef _MODELTIME_H
#define _MODELTIME_H
#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

#include "tick.h"

typedef struct {
    uint64_t Ticks;
}tModelTime;

void ModelTimeTick();
tModelTime ModelTimeGetTime(void);
void ModelTimeSpin(uint32_t delay);

#ifdef __cplusplus
};
#endif

#endif
