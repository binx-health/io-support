/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : William Chung-How                                                                 */
/* Reviewers : Chris Dawber                                                                      */
/*************************************************************************************************/
#pragma once
#include "stdafx.h"
#include "gtest/gtest.h"
#include "Logger.h"
#include "ScriptDiskTest.hpp"

extern "C" { void LoggerInit(void); }

// Class definition
//=================================================================================================
class LoggerTest : public ScriptDiskTest {
    public:
        virtual void SetUp() { LoggerInit();}
        virtual void TeadDown() {  }
};

//=================================================================================================
TEST_F(LoggerTest, LoggerSend) {
    LoggerSend(eLOGGING_TEXT, "Testing 1\r\n");
    LoggerSend(eTHROW_TEXT, "Testing %d %s\r\n", 2, "string");
    LoggerSend(eDEVICE_STATE, "Testing 3\r\n");
    LoggerSend(ePEAK_TEXT, "Testing 4\r\n");

    EXPECT_TRUE(CompareFile("output.txt", "output_logger.txt"));
}
