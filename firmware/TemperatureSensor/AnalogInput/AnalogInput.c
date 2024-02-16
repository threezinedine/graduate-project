#include <AnalogInput.h>
#include <LinkList.h>
#include "main.h"
#include <string.h>
#include "stdlib.h"
#include "StringUtils.h"


#define DEFAULT_ANALOG_INPUT_DATA_SIZE 4

static LinkList *m_sAnalogInputs;
static uint8_t *m_u8Buffer;

static void NodeFreeElement(LinkList_Node *sNode);
static uint8_t GetTypeSize(uint8_t type);
static void GetTypeString(uint8_t type, char *buffer, uint16_t size);
static float AnalogInput_DefaultHandler(AnalogInput_Instance *sIns);
static float GetDefaultOutputMin(uint8_t type);
static float GetDefaultOutputMax(uint8_t type);

void AnalogInput_Init(uint8_t numAnalogInput)
{
    m_sAnalogInputs = LinkList_Init();
    LinkList_SetFreeElement(m_sAnalogInputs, NodeFreeElement);

    m_u8Buffer = (uint8_t *)malloc(sizeof(uint8_t) * GetTypeSize(AI_FLOAT32) * numAnalogInput);

    for (int i = 0; i < numAnalogInput; i++)
    {
        AnalogInput_Instance *sIns = (AnalogInput_Instance *)malloc(sizeof(AnalogInput_Instance));
        sIns->u8IsActive = AI_ON;
        sIns->u8Type = AI_FLOAT32;
        sIns->inputMax = 3.3;
        sIns->handler = AnalogInput_DefaultHandler;
        sIns->outputMin = GetDefaultOutputMin(AI_FLOAT32);
        sIns->outputMax = GetDefaultOutputMax(AI_FLOAT32);

        LinkList_Append(m_sAnalogInputs, sIns);
    }
}

void AnalogInput_Release()
{
    free(m_u8Buffer);

    LinkList_Release(m_sAnalogInputs);
}

uint8_t AnalogInput_GetDataLength()
{
    uint8_t length = 0;

    for (int i = 0; i < LinkList_GetSize(m_sAnalogInputs); i++)
    {
        AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, i);
        if (sIns->u8IsActive == AI_ON)
        {
            length += GetTypeSize(sIns->u8Type);
        }
    }
    return length;
}

void AnalogInput_SetType(uint8_t index, uint8_t type)
{
    AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, index);
    sIns->outputMin = GetDefaultOutputMin(type);
    sIns->outputMax = GetDefaultOutputMax(type);
    sIns->u8Type = type;
}

void AnalogInput_TurnOff(uint8_t index)
{
    AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, index);
    sIns->u8IsActive = AI_OFF;
}

void AnalogInput_TurnOn(uint8_t index)
{
	AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, index);
	sIns->u8IsActive = AI_ON;
}

void AnalogInput_SetMinAndMax(uint8_t index, float min, float max)
{
    AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, index);
    sIns->outputMin = min;
    sIns->outputMax = max;
}

void AnalogInput_SetInputMax(uint8_t index, float value)
{
    AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, index);
    sIns->inputMax = value;
}

void AnalogInput_SetHandler(uint8_t index, AnalogInput_Handler handler)
{
    AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, index);
    sIns->handler = handler;
}

void AnalogInput_Update()
{
    memset(m_u8Buffer, 0, AnalogInput_GetDataLength());
    uint8_t bufferIndex = 0;

    for (int i = 0; i < LinkList_GetSize(m_sAnalogInputs); i++)
    {
        AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, i);

        if (sIns->u8IsActive == AI_OFF)
        {
            continue;
        }

        uint8_t outputSize = GetTypeSize(sIns->u8Type);
        float input = sIns->handler(sIns);
        float f32Result = (input / sIns->inputMax) * (sIns->outputMax - sIns->outputMin) + sIns->outputMin;

#define CONVERT(type_const, uni, type)                                           \
    case type_const:                                                             \
        uni value_##type_const;                                                  \
        value_##type_const.value = (type)f32Result;                              \
        memcpy(m_u8Buffer + bufferIndex, value_##type_const.buffer, outputSize); \
        break;

        switch (sIns->u8Type)
        {
            CONVERT(AI_UINT16, U16, uint16_t)
            CONVERT(AI_INT16, I16, int16_t)
            CONVERT(AI_UINT32, U32, uint32_t)
            CONVERT(AI_INT32, I32, int32_t)
            CONVERT(AI_FLOAT32, F32, float)
        }

        bufferIndex += outputSize;
    }
}

void AnalogInput_GetData(uint8_t *data)
{
    memcpy(data, m_u8Buffer, AnalogInput_GetDataLength());
}

void AnalogInput_GetStatusString(char *response, uint16_t size)
{
    memset(response, 0, size);
    uint8_t numAIs = LinkList_GetSize(m_sAnalogInputs);
    for (int i = 0; i < numAIs; i++)
    {
        char tempStr[32];
        char typeString[8];
        AnalogInput_Instance *sIns = (AnalogInput_Instance *)LinkList_Get(m_sAnalogInputs, i);
        GetTypeString(sIns->u8Type, typeString, 8);
        FormatString(tempStr, "%d: %s | %s | %d | %d | %d", i,
        			 typeString,
                     sIns->u8IsActive == AI_ON ? "on" : "off",
                     (int)(sIns->inputMax * 10),
                     (int)(sIns->outputMin * 10),
                     (int)(sIns->outputMax * 10));

        if (i == 0)
        {
            FormatString(response, "%s", tempStr);
        }
        else if (i == numAIs - 1)
        {
        	FormatString(response, "%s;%s;", response, tempStr);
        }
        else
        {
            FormatString(response, "%s;%s", response, tempStr);
        }
    }
}

uint8_t AnalogInput_TypeFromString(char *chType)
{
#define CONVERT_STRING_TO_TYPE(type, strType) \
    if (strcmp(chType, strType) == 0)         \
        return type;

    CONVERT_STRING_TO_TYPE(AI_FLOAT32, "float32");
    CONVERT_STRING_TO_TYPE(AI_INT32, "int32");
    CONVERT_STRING_TO_TYPE(AI_UINT32, "uint32");
    CONVERT_STRING_TO_TYPE(AI_INT16, "int16");
    CONVERT_STRING_TO_TYPE(AI_UINT16, "uint16");
    CONVERT_STRING_TO_TYPE(AI_INT8, "int8");
    CONVERT_STRING_TO_TYPE(AI_UINT8, "uint8");

    return -1;
}

void NodeFreeElement(LinkList_Node *sNode)
{
    free(((AnalogInput_Instance *)sNode->pData));
}

uint8_t GetTypeSize(uint8_t type)
{
    switch (type)
    {
    case AI_INT8:
    case AI_UINT8:
        return 1;
    case AI_INT16:
    case AI_UINT16:
        return 2;
    case AI_INT32:
    case AI_UINT32:
    case AI_FLOAT32:
        return 4;
    default:
        return 4;
    }
}

float AnalogInput_DefaultHandler(AnalogInput_Instance *sIns)
{
    return 0.0f;
}

float GetDefaultOutputMin(uint8_t type)
{
    return 0.0f;
}

float GetDefaultOutputMax(uint8_t type)
{
    switch (type)
    {
    case AI_UINT8:
    case AI_INT8:
        return 100.0f;
    case AI_UINT16:
    case AI_INT16:
    case AI_UINT32:
    case AI_INT32:
    case AI_FLOAT32:
        return 1000.0f;
    default:
        return 0.0f;
    }
}

void GetTypeString(uint8_t type, char *buffer, uint16_t size)
{
    memset(buffer, 0, size);

    switch (type)
    {
    case AI_UINT8:
        memcpy(buffer, "uint8", 6);
        break;
    case AI_INT8:
        memcpy(buffer, "int8", 5);
        break;
    case AI_INT16:
        memcpy(buffer, "int16", 6);
        break;
    case AI_UINT16:
        memcpy(buffer, "uint16", 7);
        break;
    case AI_INT32:
        memcpy(buffer, "int32", 6);
        break;
    case AI_UINT32:
        memcpy(buffer, "uint32", 7);
        break;
    case AI_FLOAT32:
        memcpy(buffer, "float32", 8);
        break;

    default:
        break;
    }
}