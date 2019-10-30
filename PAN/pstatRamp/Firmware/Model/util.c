/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/
/**
\file util.c
\brief Utilities
*/
#include "util.h"
#include <string.h>
#define DEV_STRING_LEN_MAX      (64)


static char lowerCase(char c);

//=================================================================================================
//! string compare without case sensitive 
/*! returns 0 if strings are identical
*/
uint8_t UtilStrcmpI(char * string1, char * string2)
{
    uint8_t len1 = strlen(string1);
    uint8_t len2 = strlen(string2);
    uint8_t i;

    if (len1 != len2)
    {
        return -1;
    }
    
    if (len1 >= DEV_STRING_LEN_MAX || len2 >= DEV_STRING_LEN_MAX)
    {
        return -1;
    }
    for (i=0; i<len1; i++)
    {
        if (lowerCase(*string1++) != lowerCase(*string2++))
        {
            return -1;
        }
    }
    // Strings all match, return 0
    return 0;
}

//=================================================================================================
//! string compare for number of characters without case sensitive 
/*! 
*/
uint8_t UtilStrncmpI(char * string1, char * string2, uint8_t len)
{
    uint8_t i;

    if (len >= DEV_STRING_LEN_MAX)
    {
        return -1;
    }
    for (i=0; i<len; i++)
    {
        if (lowerCase(*string1++) != lowerCase(*string2++))
        {
            return -1;
        }
    }
    return 0;
}

//=================================================================================================
//! converts to lower case
/*! 
*/
static char lowerCase(char c)
{
    char retVal;
    if ((c >= 'A') && (c <= 'Z'))
    {
        c += ('a' - 'A');
    }
    retVal = c;
    return retVal;
}

