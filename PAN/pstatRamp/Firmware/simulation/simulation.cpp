// simulation.cpp : Defines the entry point for the console application.
//
#include "stdafx.h"
#include "windows.h"
#include "gtest/gtest.h"
#include "ScriptDisk.h"
#include "ScriptEngine.h"

using namespace std;

static int TickCount;
static int LoopCount;

static void TickSimulate(void);
int GetLoopCount(void);


int _tmain(int argc, _TCHAR* argv[])
{	
    TickCount = 0;
    LoopCount = 0;
    
    testing::InitGoogleTest(&argc, argv);
    RUN_ALL_TESTS();

    ScriptEngineInit();
    while(1) {
        if (LoopCount++ == 10000)
        {
            TickSimulate();
            LoopCount=0;
        }
        ScriptEngineHandler();
    }
    return 0;
}

// Simulate a tick count of some sort
void TickSimulate(void)
{
    TickCount++;
}

int GetLoopCount(void)
{
    return LoopCount;
}