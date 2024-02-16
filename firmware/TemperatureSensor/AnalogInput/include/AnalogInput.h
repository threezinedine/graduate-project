#ifndef _INC_ANALOG_INPUT
#define _INC_ANALOG_INPUT

#include "main.h"
#include "DataType.h"

#define AI_INT8 0
#define AI_UINT8 1
#define AI_INT16 2
#define AI_UINT16 3
#define AI_INT32 4
#define AI_UINT32 5
#define AI_FLOAT32 6

#define AI_ON 1
#define AI_OFF 0

typedef union
{
    uint8_t u8Value;
    int8_t i8Value;
    uint16_t u16Value;
    int16_t i16Value;
    uint32_t u32Value;
    int32_t i32Value;
    float f32Value;
} AnalogInput_Value;

struct AnalogInput_Instance;

typedef float (*AnalogInput_Handler)(struct AnalogInput_Instance *);

struct AnalogInput_Instance
{
    uint8_t u8IsActive;
    uint8_t u8Type;

    float inputMax;

    float outputMin;
    float outputMax;

    AnalogInput_Handler handler;
};

typedef struct AnalogInput_Instance AnalogInput_Instance;

void AnalogInput_Init(uint8_t numAnalogInput);
void AnalogInput_Release();
uint8_t AnalogInput_GetDataLength();

void AnalogInput_SetType(uint8_t index, uint8_t type);
void AnalogInput_TurnOff(uint8_t index);
void AnalogInput_TurnOn(uint8_t index);
void AnalogInput_SetMinAndMax(uint8_t index, float min, float max);
void AnalogInput_SetInputMax(uint8_t index, float value);
void AnalogInput_SetHandler(uint8_t index, AnalogInput_Handler handler);

void AnalogInput_Update();
void AnalogInput_GetData(uint8_t *data);

void AnalogInput_GetStatusString(char *response, uint16_t size);
uint8_t AnalogInput_TypeFromString(char *chType);

#endif
