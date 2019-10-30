#ifndef _BARCODE_H
#define _BARCODE_H

#ifdef __cplusplus
extern "C" {
#endif
#include <stdint.h>
#include <stdbool.h>
  
void BarcodeInit(void);
void BarcodeTriggerOn(void);
void BarcodeTriggerOff(void);
char * BarcodeGetString(void);
bool BarcodeHasData(void);
void BarcodeClearStatus(void);


#ifdef __cplusplus
};
#endif


#endif
