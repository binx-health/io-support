/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#ifndef _PELTIER_H
#define _PELTIER_H

#ifdef __cplusplus
extern "C" {
#endif
#include "cpld.h"
#include <stdint.h>

typedef enum {
    eHEATING_CYCLE,
    eCOOLING_CYCLE,
    eLAST_CYCLE
}eCycleType;

typedef struct {
    uint16_t min;
    uint16_t max;
}tPeltierTemperatureRange;

void PeltierInit(void);
void PeltierStop(void);
void PeltierSetDefaultMetrics(void);
void PeltierSetParams(ePeltierType type, tPeltierParams * pParams);
void PeltierTick5Ms(void);
tPeltierMetrics * PeltierGetMetrics(eMetricType metric, ePeltierType peltier);
tPeltierParams * PeltierGetParams(ePeltierType peltier);
void PeltierSetFanTimeout(uint32_t timeout);
void PeltierTest(bool start, bool heat);
uint32_t PeltierGetPcrCycles(void);
void PeltierResetPcrCycles(void);

#ifdef __cplusplus
};
#endif


#endif
