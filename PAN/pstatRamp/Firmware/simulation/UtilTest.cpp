/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : William Chung-How                                                                 */
/* Reviewers : Chris Dawber                                                                      */
/*************************************************************************************************/
#pragma once
#include "stdafx.h"
#include "gtest/gtest.h"
#include "util.h"
#include "ScriptDiskTest.hpp"

extern "C" { void LoggerInit(void); }

// Class definition
//=================================================================================================
class UtilTest : public testing::Test {
};

//=================================================================================================
TEST_F(UtilTest, StrcmpI) {
    EXPECT_EQ(UtilStrcmpI("test", "TEST"), 0);
    EXPECT_EQ(UtilStrcmpI("testing", "testing"), 0);
    EXPECT_NE(UtilStrcmpI("testing1", "testing"), 0);
}

//=================================================================================================
TEST_F(UtilTest, UtilStrncmpI) {
    EXPECT_EQ(UtilStrncmpI("test", "TEST", 4), 0);    
    EXPECT_EQ(UtilStrncmpI("testing", "testing", 7), 0);
    EXPECT_NE(UtilStrncmpI("testing1", "testing", 8), 0);
}
