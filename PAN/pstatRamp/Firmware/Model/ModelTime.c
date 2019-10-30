/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#include <stdbool.h>
#include "ModelTime.h"


static tModelTime Time;

//=================================================================================================
//! 1ms Tick 
/*! 
*/
void ModelTimeTick()
{
    Time.Ticks++;
}

//=================================================================================================
//! Returns the number of ticks elapsed
/*! 
*/
tModelTime ModelTimeGetTime(void)
{
    return Time;    
}

//=================================================================================================
//! Spins for specified time
/*! 
*/
void ModelTimeSpin(uint32_t delay)
{
    TickDelayMs(delay);
}
