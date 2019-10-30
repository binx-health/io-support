/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : William Chung-How                                                                 */
/* Reviewers : Chris Dawber                                                                      */
/*************************************************************************************************/
#pragma once
#include "stdafx.h"
#include "gtest/gtest.h"
#include "ScriptDisk.h"
#include "Logger.h"
#include "peltier.h"
#include "ScriptEngine.h"
#include "ScriptDiskTest.hpp"
#include "simIo.hpp"

extern "C" { void LoggerInit(void); }
// Class definition
//=================================================================================================
class ScriptEngineTest : public ScriptDiskTest {
    public:
        virtual void SetUp() { ScriptEngineInit(); LoggerInit();}
        virtual void TearDown() { ScriptEngineInit(); }
};

//=================================================================================================
TEST_F(ScriptEngineTest, FibreCreation) {
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");
    EXPECT_EQ(1, ScriptEngineNumFibres());
    TestScriptDiskLoadFile("dummy1.txt");
    ScriptEngineExecute("dummy1.txt");
    EXPECT_EQ(2, ScriptEngineNumFibres());
    TestScriptDiskLoadFile("dummy2.txt");
    ScriptEngineExecute("dummy2.txt");
    EXPECT_EQ(3, ScriptEngineNumFibres());
}

//=================================================================================================
TEST_F(ScriptEngineTest, FibreDestroy) {
    TestScriptDiskLoadFile("dummy3.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("dummy3.txt");
    EXPECT_EQ(1, ScriptEngineNumFibres());
    TestScriptDiskLoadFile("dummy4.txt");
    ScriptEngineExecute("dummy4.txt");
    EXPECT_EQ(2, ScriptEngineNumFibres());

    ScriptEngineHandler();   
    EXPECT_EQ(2, ScriptEngineNumFibres());                           
    ScriptEngineHandler();               
    ScriptEngineHandler();
    EXPECT_EQ(1, ScriptEngineNumFibres());                            
    ScriptEngineHandler();                          
    ScriptEngineHandler();                          
    ScriptEngineHandler();                          
    ScriptEngineHandler();                          
    ScriptEngineHandler();                          
    ScriptEngineHandler();                          
    EXPECT_EQ(0, ScriptEngineNumFibres());
}
//=================================================================================================
TEST_F(ScriptEngineTest, DeviceNameTable) {
    EXPECT_EQ(0,ScriptEngineDevNameLookup("v1").id);
    EXPECT_EQ(7,ScriptEngineDevNameLookup("v8").id);
    EXPECT_EQ(43,ScriptEngineDevNameLookup("opto3").id);
    EXPECT_FALSE(ScriptEngineMapNameLookup("NoEntry").isValid);
}

//=================================================================================================
TEST_F(ScriptEngineTest, MapNameTable) {
    ScriptEngineMapName("v8", "testMapv8");
    ScriptEngineMapName("l1", "testMapl1");
    ScriptEngineMapName("ps1", "testMapps1");
    ScriptEngineMapName("WrongName", "testWrongDeviceName");
    ScriptEngineMapName("ps1", "testMapNew");
    EXPECT_EQ(7,ScriptEngineMapNameLookup("testMapv8").id);
    EXPECT_EQ(25,ScriptEngineMapNameLookup("testMapl1").id);
    EXPECT_EQ(30,ScriptEngineMapNameLookup("testMapNew").id);
    EXPECT_FALSE(ScriptEngineMapNameLookup("testWrongDeviceName").isValid);
}

//=================================================================================================
TEST_F(ScriptEngineTest, DescNameTable) {
    ScriptEngineDescName("v8", "testDescv8");
    ScriptEngineDescName("l1", "testDescl1");
    ScriptEngineDescName("ps1", "testDescps1");
    ScriptEngineDescName("v1", "n/c");
    ScriptEngineDescName("v16", "-");
    EXPECT_EQ(7,ScriptEngineDescNameLookup("testDescv8").id);
    EXPECT_EQ(25,ScriptEngineDescNameLookup("testDescl1").id);
    EXPECT_EQ(30,ScriptEngineDescNameLookup("testDescps1").id);
    EXPECT_FALSE(ScriptEngineDescNameLookup("NoEntry").isValid);
    EXPECT_FALSE(ScriptEngineDescNameLookup("n/c").isValid);
    EXPECT_FALSE(ScriptEngineDescNameLookup("-").isValid);
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserMap) {
    char lineBuf[128];
    char * pMappedDevice;

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test exceeding maximum number of command parameters
    sprintf(lineBuf, "1 2 3 4 5 6 8 9 10 11");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "map  V1    testv1");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    pMappedDevice = ScriptEngineGetMapName("v1");
    EXPECT_STREQ("testv1", pMappedDevice);

    sprintf(lineBuf, "map v1 testv1 dummyParam");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "map v1 testv1MoreThanTheMaximumSizeForParameterStringTesting01234567890abcdef");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserPhase) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test exceeding maximum number of command parameters
    sprintf(lineBuf, "phase");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "phase startup");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "phase testv1MoreThanTheMaximumSizeForParameterStringTesting01234567890abcdef");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ParserValve) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test mapped name does not exist
    sprintf(lineBuf, "valve vacuum_enable off");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
        
    // Test valve command not referencing a valve
    sprintf(lineBuf, "map  p1    Vacuum_Enable");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "valve vacuum_enable off");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test invalid valve
    sprintf(lineBuf, "valve v21 off");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test first valve
    sprintf(lineBuf, "map  v1    v1test");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "valve v1test off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    // Test last valve
    sprintf(lineBuf, "valve v20 off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    // Test correct command with mixed case
    ScriptEngineInit();
    sprintf(lineBuf, "map  V12    Vacuum_Enable");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "valve vacuum_enable invalid");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "valve vacuum_enable on");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "valve vacuum_enable off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserSolenoid) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test mapped name does not exist
    sprintf(lineBuf, "solenoid drawer off");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
        
    // Test valve command not referencing a solenoid
    sprintf(lineBuf, "solenoid drawer off");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test correct command with mixed case
    ScriptEngineInit();
    sprintf(lineBuf, "map  m1    testm1");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "map  s1    tests1");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "solenoid testm1 invalidPrm");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "solenoid testm1 on");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "solenoid testm1 off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "solenoid tests1 on");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "solenoid tests1 off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserDelay) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "delay 1 2");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong parameter
    sprintf(lineBuf, "delay notNumber");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "delay 100");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserDpr) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "dpr dpr1");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong number of parameters for on
    sprintf(lineBuf, "dpr dpr1 on 0 0 0");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong number of parameters for on
    sprintf(lineBuf, "dpr dpr1 on");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong number of parameters for on
    sprintf(lineBuf, "dpr dpr1 on not_a_number");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong number of parameters for on
    sprintf(lineBuf, "dpr dpr1 on 3 not_number");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong parameter
    sprintf(lineBuf, "dpr dpr1 not_on_or_off");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong device
    sprintf(lineBuf, "dpr pr1 off");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "dpr dpr1 on 0 0");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "dpr dpr1 on 10 40");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));    

    sprintf(lineBuf, "dpr dpr1 off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0)); 
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserSetGetVariables) {
    char buf[128];
    bool bVariable;

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    //Test maximum string length exceeded - 64 characters
    sprintf(buf, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789ab");
    EXPECT_FALSE(ScriptEngineSetVariable(buf, false).isValid);

    //Test maximum number of variables exceeded
    for (int i=0; i<MAX_VARIABLES_NUM; i++)
    {
        sprintf(buf, "testVariable%d", i);
        EXPECT_TRUE(ScriptEngineSetVariable(buf, false).isValid);
    }

    EXPECT_TRUE(ScriptEngineSetVariable(buf, false).isValid);
    EXPECT_FALSE(ScriptEngineSetVariable("ShouldFail", false).isValid);

    // Test getting back variable
    EXPECT_TRUE(ScriptEngineGetVariable("testVariable3", &bVariable).isValid);
    EXPECT_FALSE(bVariable);
    ScriptEngineSetVariable("testVariable3", true);
    EXPECT_TRUE(ScriptEngineGetVariable("testVariable3", &bVariable).isValid);
    EXPECT_TRUE(bVariable);

    // Test getting unknown variable 
    EXPECT_FALSE(ScriptEngineGetVariable("NotDefinedVariable", &bVariable).isValid);
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserWait) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "wait var1 2 1");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong parameter
    sprintf(lineBuf, "wait var1 abc");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    EXPECT_TRUE(ScriptEngineSetVariable("var1", false).isValid);
    sprintf(lineBuf, "wait var1 10");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserRes) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong parameter
    sprintf(lineBuf, "res res2 invalid");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    // Test max pressure less or equal than set pressure
    sprintf(lineBuf, "res res2 hold 600 600 20");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "res res2 hold 600 700 20");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "res res2 hold 600 800 20 ackVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "res res1 dump");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserStepper) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "stepper l1 500 -1500 ackVar dummy");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    // Test wrong parameter
    sprintf(lineBuf, "stepper l1 500 invalid ackVar");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    // Test wrong device
    sprintf(lineBuf, "stepper res1 500 -1500 ackVar");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "stepper l2 500 -1500");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "stepper l3 500 -1500 ackVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserTherm) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "therm therm1");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "therm therm1 onsf");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "therm therm1 on 100");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "therm therm1 on invalid ackVar");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "therm therm1 on 100 ackVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "therm therm2 off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "therm therm3 on 100 ackVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}


//=================================================================================================
TEST_F(ScriptEngineTest, ParserVoltammetry) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "voltammetry squarewave 1 2 3 4 5 6 7 8");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "voltammetry differential 1 2 3 4 5 not_number ackVar");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "voltammetry differential 1 2 3 4 5");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "voltammetry not_valid 1 2 3 4 5 ackVar");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "voltammetry squarewave 1 2 3 4 5 ackVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "voltammetry differential 1 2 3 4 5 6 ackVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "voltammetry squarewave 1 2 3 4 5");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "voltammetry differential 1 2 3 4 5 6");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "voltammetry differential -1 2 3 4 5 6");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserFluid) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "fluid pstat1 ackVar dummy");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    // wrong device
    sprintf(lineBuf, "fluid ps1 ackVar");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "fluid pstat1 ackVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "fluid pstat4 ackVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserIf) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "if flagVar dummy");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    // flag does not exist
    sprintf(lineBuf, "if flagNotExist");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    EXPECT_TRUE(ScriptEngineSetVariable("flagVar", false).isValid);
    sprintf(lineBuf, "if flagVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    EXPECT_TRUE(ScriptEngineSetVariable("flagVar", true).isValid);
    sprintf(lineBuf, "if flagVar");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserCall) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_call.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_call.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "call dummy1.txt invalidParam");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    
    // File does not exist
    sprintf(lineBuf, "call dummy1.txt");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    
    TestScriptDiskLoadFile("dummy1.txt");
    sprintf(lineBuf, "call dummy1.txt");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserBeginEnd) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_call.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_call.txt");

    // Test wrong number of parameters
    sprintf(lineBuf, "begin invalid");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    
    sprintf(lineBuf, "begin nonatomic 10 ackBegin");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));    

    sprintf(lineBuf, "end");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));    
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserThrow) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    sprintf(lineBuf, "throw \"this is a test\"");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserPeak) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    sprintf(lineBuf, "peak \"this is a test\"");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ParserBuzzer) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    sprintf(lineBuf, "buzzer 1 2 3 4");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "buzzer a b c");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "buzzer 1 2 3");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ParserRegValve) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    sprintf(lineBuf, "regValve 100 1");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "regValve on");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "regValve 1500");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ParserSet) {
    char lineBuf[128];
    tPeltierMetrics * pHeatMetrics = PeltierGetMetrics(eTEST_METRIC, ePCR_PELTIER);
    tPeltierMetrics * pCoolMetrics = PeltierGetMetrics(eTEST_METRIC, eLYSIS_PELTIER);
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    sprintf(lineBuf, "set dummyMetric 12");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    // simulate typo error Pletier 
    sprintf(lineBuf, "set Pletier.Pcr.Ki 0");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    // More than 10 parameters
    sprintf(lineBuf, "set Peltier.Pcr.Ki.one.two.three.four.five.six.seven.eight.nine.ten 0");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "set Peltier.Pcr.Ki 55");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    EXPECT_EQ(55, pHeatMetrics->Ki);
    
    sprintf(lineBuf, "set Peltier.Lysis.Ki 10");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    EXPECT_EQ(10, pCoolMetrics->Ki);
}
//=================================================================================================
TEST_F(ScriptEngineTest, ParserPreport) {
    char lineBuf[128];
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    sprintf(lineBuf, "preport");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "preport 1");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "preport on");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "preport off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ParserPstat) {
    char lineBuf[128];
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    sprintf(lineBuf, "pstat on");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "pstat 1500 0");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "pstat 1500 internal");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "pstat 5000 external");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "pstat");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "pstat 5100 external");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ParserPeltier) {
    char lineBuf[128];
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("initialise_run.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("initialise_run.txt");

    sprintf(lineBuf, "peltier");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "peltier 1");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));

    sprintf(lineBuf, "peltier heat");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "peltier cool");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    sprintf(lineBuf, "peltier off");
    EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteIf) {

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_if.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_if.txt");
    EXPECT_EQ(1, ScriptEngineNumFibres());

    ScriptEngineHandler();                          // res res1 hold 500 20 initvswID
    ScriptEngineHandler();                          // res res2 hold 100 10 flag2
    ScriptEngineSetVariable("initvswID", true);     
    ScriptEngineHandler();                          // if initvswID
    ScriptEngineHandler();                          // 	voltammetry differential 1 2 3 4 5 6 ackVar
    ScriptEngineHandler();                          // else
    ScriptEngineHandler();                          // 	voltammetry differential 6 5 4 3 2 1 ackVar
    ScriptEngineHandler();                          // endif
    ScriptEngineHandler();                          // res res3 dump

    ScriptEngineHandler();                          // Fibre destroyed
    EXPECT_TRUE(CompareFile("output.txt", "output_ExecuteIf.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteInLineCommentIf) {

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_CommentIf.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_CommentIf.txt");
    EXPECT_EQ(1, ScriptEngineNumFibres());

    ScriptEngineHandler();                          // res res1 hold 500 20 initvswID
    ScriptEngineHandler();                          // Comment line
    ScriptEngineHandler();                          // res res2 hold 100 10 flag2
    ScriptEngineSetVariable("initvswID", true);     
    ScriptEngineHandler();                          // if initvswID
    ScriptEngineHandler();                          // 	voltammetry differential 1 2 3 4 5 6 ackVar
    ScriptEngineHandler();                          // else
    ScriptEngineHandler();                          // 	voltammetry differential 6 5 4 3 2 1 ackVar
    ScriptEngineHandler();                          // endif
    ScriptEngineHandler();                          // res res3 dump

    ScriptEngineHandler();                          // Fibre destroyed
    EXPECT_TRUE(CompareFile("output.txt", "output_ExecuteIf.txt"));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteElse) {

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_if.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_if.txt");
    EXPECT_EQ(1, ScriptEngineNumFibres());

    ScriptEngineHandler();                          // res res1 hold 500 20 initvswID
    ScriptEngineHandler();                          // res res2 hold 100 10 flag2    
    ScriptEngineHandler();                          // if initvswID
    ScriptEngineHandler();                          // 	voltammetry differential 1 2 3 4 5 6 ackVar
    ScriptEngineHandler();                          // else
    ScriptEngineHandler();                          // 	voltammetry differential 6 5 4 3 2 1 ackVar
    ScriptEngineHandler();                          // endif
    ScriptEngineHandler();                          // res res3 dump

    ScriptEngineHandler();                          // Fibre destroyed
    EXPECT_TRUE(CompareFile("output.txt", "output_ExecuteElse.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteStackOverflow) {
    char lineBuf[128];

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_if.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_if.txt");
    EXPECT_EQ(1, ScriptEngineNumFibres());


    EXPECT_TRUE(ScriptEngineSetVariable("testVar", false).isValid);

    sprintf(lineBuf, "if testVar");
    
    EXPECT_TRUE(ScriptEngineSetVariable("testVar", true).isValid);
    
    //Test maximum number of stack frames exceeded
    for (int i=0; i<MAX_STACK_FRAME-1; i++)
    {
        sprintf(lineBuf, "if testVar");
        EXPECT_TRUE(ScriptEngineParseCommand(lineBuf, 0));
    }
    sprintf(lineBuf, "if testVar");
    EXPECT_FALSE(ScriptEngineParseCommand(lineBuf, 0));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteScriptError) {

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_error.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_error.txt");
    EXPECT_EQ(1, ScriptEngineNumFibres());


    for (int i=0; i<100; i++)
    {
        ScriptEngineHandler();
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_scriptError.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteNestedAtomicBegin) {

    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_NestedAtomic.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_NestedAtomic.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_nestedAtomic.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteNestedNonAtomic) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_NestedNonAtomic.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_NestedNonAtomic.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_nestedNonAtomic.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteNestedCall) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_nestedCall.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    TestScriptDiskLoadFile("testCall1.txt");
    TestScriptDiskLoadFile("testCall2.txt");
    TestScriptDiskLoadFile("testCall3.txt");
    TestScriptDiskLoadFile("testCall4.txt");
    TestScriptDiskLoadFile("testCall5.txt");
    TestScriptDiskLoadFile("testCall6.txt");
    TestScriptDiskLoadFile("testCall7.txt");
    TestScriptDiskLoadFile("testCall8.txt");
    TestScriptDiskLoadFile("testCall9.txt");
    TestScriptDiskLoadFile("testCall10.txt");

    ScriptEngineExecute("test_nestedCall.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_nestedCall.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteNestedIf) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_nestedif.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_nestedif.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_nestedif.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteTestBeginAck) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_BeginAck.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_BeginAck.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
        // Simulate tick every 100 round the loop
        if (i % 10 == 0)
        {
            ScriptEngineTick();
        }  
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_BeginAck.txt"));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteTestBeginEndFile) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_BeginEndFile.txt");
    TestScriptDiskLoadFile("test_CallBegin.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_CallBegin.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
        // Simulate tick every 100 round the loop
        if (i % 10 == 0)
        {
            ScriptEngineTick();
        }  
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_BeginEndFile.txt"));
}
//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteTestThrowError) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_ThrowError.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_ThrowError.txt");

    // Start executing script
    for (int i=0; i<1000; i++)
    {
        ScriptEngineHandler();
        // Simulate tick every 100 round the loop
        if (i % 10 == 0)
        {
            ScriptEngineTick();
        }  
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_testThrowError.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteTestTab) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_tab.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_tab.txt");

    // Start executing script
    for (int i=0; i<10000; i++)
    {
        ScriptEngineHandler();
        // Simulate tick every 100 round the loop
        if (i % 10 == 0)
        {
            ScriptEngineTick();
        }  
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_testTab.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteTestMap) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_map.txt");
    TestScriptDiskLoadFile("test_remap.txt");
    ScriptEngineExecute("test_map.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
        // Simulate tick every 100 round the loop
        if (i % 10 == 0)
        {
            ScriptEngineTick();
        }  
    }

    EXPECT_EQ(0, ScriptEngineMapNameLookup("bellows").id);
    EXPECT_EQ(1,ScriptEngineMapNameLookup("v2_new_name").id);
    EXPECT_FALSE(ScriptEngineMapNameLookup("v2_name").isValid);
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecutePhase) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_phase.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_phase.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
        // Simulate tick every 100 round the loop
        if (i % 10 == 0)
        {
            ScriptEngineTick();
        }  
    }
    EXPECT_TRUE(CompareFile("output.txt", "output_phase.txt"));
}

//=================================================================================================
TEST_F(ScriptEngineTest, ExecuteNonAtomicAck) {
    // Load and call execute so fibreId and stackId gets initialised properly
    TestScriptDiskLoadFile("test_NonAtomicAck.txt");
    EXPECT_EQ(0, ScriptEngineNumFibres());
    ScriptEngineExecute("test_NonAtomicAck.txt");

    // Start executing script
    for (int i=0; i<100000; i++)
    {
        ScriptEngineHandler();
        ScriptEngineTick();
    }
    //EXPECT_TRUE(CompareFile("output.txt", "output_NonAtomicAck.txt"));
}