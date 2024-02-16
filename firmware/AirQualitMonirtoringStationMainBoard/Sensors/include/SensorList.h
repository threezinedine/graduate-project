#ifndef _NTT_C_SENSORS_LIB
#define _NTT_C_SENSORS_LIB

#include "main.h"

#define NAME_SIZE 12
#define DATA_SIZE 32

typedef struct
{
    char chName[NAME_SIZE];
    uint8_t u8Address;
    uint8_t u8StartRegAddr;
    uint8_t u8RegNum;
    uint8_t u8Data[DATA_SIZE];
} Sensor;

typedef void (*ForEach)(Sensor *);

void Sensors_Init();
void Sensors_Release();

void Sensors_AddSensors(char *chName);
uint8_t Sensors_Length();
void Sensors_ForEach(ForEach fCallback);
Sensor *Sensors_GetSensor(uint8_t u8Index);
void Sensors_Remove(uint8_t u8Index);
void Sensors_Serialize(uint8_t *u8Buffer);
void Sensors_DeSerialize(uint8_t *u8Buffer);
void Sensors_Clear();
void Sensors_ToString(uint8_t u8Index, char *chStr);
uint16_t Sensors_GetData(uint8_t *u8Data);

#endif
