/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : William Chung-How                                                                 */
/* Reviewers : Chris Dawber                                                                      */
/*************************************************************************************************/
#pragma once
#include "stdafx.h"
#include <limits.h>
#include "gtest/gtest.h"
#include "ModelTime.h"
#include "ScriptDiskTest.hpp"

// Class definition
//=================================================================================================
class ModelTimeTest : public testing::Test {
};

//=================================================================================================
TEST_F(ModelTimeTest, Ticks) {
    uint32_t i;
    for (i=0; i< 45678; i++)
    {
        ModelTimeTick();
    }
    EXPECT_EQ(45678, ModelTimeGetTime().Ticks);
}

