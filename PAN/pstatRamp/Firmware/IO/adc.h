#ifndef ADC_H
#define ADC_H

typedef enum {
    
    eLOW_PRESSURE_ADC,
    eHIGH_PRESSURE_ADC,
    eVARIABLE_PRESSURE_ADC,
    eVACUUM_PRESSURE_ADC,
    eLAST_ADC
}eAdcChannel;

void AdcInit(void);
uint16_t AdcGetVoltage(eAdcChannel adcChannel);
uint16_t AdcGetValue(eAdcChannel channel);
#endif
