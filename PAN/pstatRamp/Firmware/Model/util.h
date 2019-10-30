/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/

#ifndef UTIL_H
#define UTIL_H

#ifdef __cplusplus
extern "C" {
#endif
#include <stdint.h>

uint8_t UtilStrcmpI(char * string1, char * string2);
uint8_t UtilStrncmpI(char * string1, char * string2, uint8_t len);

#ifdef __cplusplus
};
#endif

#endif
