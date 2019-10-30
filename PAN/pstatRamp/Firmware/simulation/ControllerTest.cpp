/******************************************************************************/
/*   Project : IO                                                             */
/*   Authors : William Chung-How                                              */
/* Reviewers : Chris Dawber                                                   */
/******************************************************************************/
#pragma once
#include "stdafx.h"
#include "gtest/gtest.h"
#include "ScriptDiskTest.hpp"
#include "controller.h"
#include "ScriptEngine.h"

// Class definition
//=================================================================================================
class ControllerTest : public ScriptDiskTest {
    public:
        virtual void SetUp() { CtrlInit(); ScriptEngineInit();}
        virtual void TeadDown() {  }
    private:
};
//=================================================================================================
TEST_F (ControllerTest, PeltierDefaultMetric) {
    char buf[64];
    PeltierInit();

    tPeltierMetrics * pMetrics = PeltierGetMetrics(eDEFAULT_METRIC, ePCR_PELTIER);
    
    sprintf(buf, "peltier.pcr.kp");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 55);
    EXPECT_EQ(55, pMetrics->Kp);

    sprintf(buf, "peltier.pcr.imax");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 23);
    EXPECT_EQ(23, pMetrics->IMax);

    sprintf(buf, "peltier.pcr.detection.mintemp");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 15);
    EXPECT_NE(150, pMetrics->MinTemp);

    sprintf(buf, "peltier.pcr.mintemp");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 15);
    EXPECT_NE(150, pMetrics->MaxTemp);

    sprintf(buf, "peltier.pcr.mintemp");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 15);
    EXPECT_EQ(150, pMetrics->MinTemp);
        
    sprintf(buf, "peltier.pcr.pwmmax");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 50);
    EXPECT_EQ(50, pMetrics->PwmMax);

}

//=================================================================================================
TEST_F (ControllerTest, PeltierTestMetric) {
    char buf[64];
    
    tPeltierMetrics * pMetrics = PeltierGetMetrics(eTEST_METRIC, eDETECT_PELTIER);

    sprintf(buf, "peltier.detection.kp");
    CtrlSetMetric(eTEST_METRIC, buf, 55);
    EXPECT_EQ(55, pMetrics->Kp);

    sprintf(buf, "peltier.detection.imax");
    CtrlSetMetric(eTEST_METRIC, buf, 23);
    EXPECT_EQ(23, pMetrics->IMax);

    sprintf(buf, "peltier.detection.mintemp");
    CtrlSetMetric(eTEST_METRIC, buf, 4);
    EXPECT_EQ(40, pMetrics->MinTemp);

    sprintf(buf, "peltier.pcr.mintemp");
    CtrlSetMetric(eTEST_METRIC, buf, 24);
    EXPECT_NE(240, pMetrics->MinTemp);

    sprintf(buf, "peltier.detection.pwmmax");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 50);
    EXPECT_NE(50, pMetrics->PwmMax);

    sprintf(buf, "peltier.detection.pwmmax");
    CtrlSetMetric(eTEST_METRIC, buf, 50);
    EXPECT_EQ(50, pMetrics->PwmMax);
}

//=================================================================================================
TEST_F (ControllerTest, ReportingMetric) {
    char buf[64];
    
    tReportMetrics * pDefault = CtrlGetReportMetrics(eDEFAULT_METRIC);
    tReportMetrics * pTest= CtrlGetReportMetrics(eTEST_METRIC);

    sprintf(buf, "general.log.fastreporting");
    CtrlSetMetric(eTEST_METRIC, buf, 100);
    EXPECT_EQ(100, pTest->FastPeriod);

    sprintf(buf, "general.log.slowreporting");
    CtrlSetMetric(eTEST_METRIC, buf, 2345);
    EXPECT_EQ(2345, pTest->SlowPeriod);

    sprintf(buf, "general.log.fastreporting");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 123);
    EXPECT_EQ(123, pDefault->FastPeriod);
    EXPECT_EQ(100, pTest->FastPeriod);

    sprintf(buf, "general.log.slowreporting");
    CtrlSetMetric(eDEFAULT_METRIC, buf, 5678);
    EXPECT_EQ(5678, pDefault->SlowPeriod);
    EXPECT_EQ(2345, pTest->SlowPeriod);

}

