#pragma once
#include "stdafx.h"
#include "gtest/gtest.h"
#include "peltier.h"


// Class definition
//=================================================================================================
class PeltierTest : public testing::Test {
    public:
        virtual void SetUp() { PeltierInit(); }
        virtual void TeadDown() {  }
};

//=================================================================================================
TEST_F (PeltierTest, DefaultMetrics) {
    tPeltierParams params;
    tPeltierMetrics * pMetrics = PeltierGetMetrics(eDEFAULT_METRIC, ePCR_PELTIER);
    memset(&params, 0, sizeof(tPeltierParams));
    
    pMetrics->Kp = 100;
    pMetrics->Kd = 0;
    pMetrics->Ki = 1;
    pMetrics->IMax = 13;
    pMetrics->MinTemp = 15;
    pMetrics->MaxTemp = 100;
    params.mode = ePELTIER_ON;
    params.topPlateTarget = 950;
    params.topPlateActual = 250;

    PeltierSetParams(ePCR_PELTIER, &params);

    for (int i=0; i< 80; i++)
    {
        PeltierTick5Ms();
    }

}
