#ifndef DRAWER_H
#define DRAWER_H

#ifdef __cplusplus
extern "C" {
#endif

typedef enum {
    eOPTO_1,
    eOPTO_2,
    eOPTO_3,
    eOPTO_LAST
}eOptoType;

typedef enum {
    eUNKNOWN_POS_DRAWER,
    eOPENED_DRAWER,
    eCLOSED_DRAWER
}eDrawerState;


void DrawerInit(void);
void DrawerTick10Ms(void);
uint8_t DrawerGetOptoStatus(eOptoType type);
eDrawerState DrawerGetState(void);

#ifdef __cplusplus
};
#endif


#endif
