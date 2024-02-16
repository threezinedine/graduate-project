#include <SensorList.h>
#include <stdlib.h>
#include <LinkList.h>
#include <string.h>
#include <StringUtils.h>

#include "main.h"

static LinkList *m_sSensors;

static void NodeFreeElement(LinkList_Node *sNode)
{
    free(((Sensor *)sNode->pData));
}

void Sensors_Init()
{
    m_sSensors = LinkList_Init();
    LinkList_SetFreeElement(m_sSensors, NodeFreeElement);
}

void Sensors_Release()
{
    LinkList_Release(m_sSensors);
}

void Sensors_AddSensors(char *chName)
{
    Sensor *sSensor = (Sensor *)malloc(sizeof(Sensor));
    memset(sSensor->chName, 0, sizeof(sSensor->chName));
    memcpy(sSensor->chName, chName, strlen(chName));

    LinkList_Append(m_sSensors, sSensor);
}

void Sensors_Remove(uint8_t u8Index)
{
    LinkList_Delete(m_sSensors, u8Index);
}

uint8_t Sensors_Length()
{
    return LinkList_GetSize(m_sSensors);
}

void Sensors_ForEach(ForEach fCallback)
{
    for (int i = 0; i < LinkList_GetSize(m_sSensors); i++)
    {
        Sensor *sSensor = (Sensor *)LinkList_Get(m_sSensors, i);
        fCallback(sSensor);
    }
}

void Sensors_Serialize(uint8_t *u8Buffer)
{
//    uint16_t u16BufferSize = LinkList_GetSize(m_sSensors) * sizeof(Sensor);
    uint16_t u16Index = 1;

    u8Buffer[0] = LinkList_GetSize(m_sSensors);

    for (int i = 0; i < LinkList_GetSize(m_sSensors); i++)
    {
        Sensor *sSensor = (Sensor *)LinkList_Get(m_sSensors, i);
        memcpy(u8Buffer + u16Index, sSensor, sizeof(Sensor));
        u16Index += sizeof(Sensor);
    }
}

void Sensors_Clear()
{
    LinkList_Release(m_sSensors);
    m_sSensors = LinkList_Init();
}

void Sensors_DeSerialize(uint8_t *u8Buffer)
{
    uint8_t u8SensorCount = u8Buffer[0];
    uint16_t u16Index = 1;

    for (int i = 0; i < u8SensorCount; i++)
    {
        Sensor *sSensor = (Sensor *)malloc(sizeof(sSensor));
        memcpy(sSensor, u8Buffer + u16Index, sizeof(Sensor));
        LinkList_Append(m_sSensors, (void *)sSensor);
        u16Index += sizeof(Sensor);
    }
}

Sensor *Sensors_GetSensor(uint8_t u8Index)
{
    return (Sensor *)LinkList_Get(m_sSensors, u8Index);
}

void Sensors_ToString(uint8_t u8Index, char *chStr)
{
    Sensor *sSensor = Sensors_GetSensor(u8Index);
	FormatString(chStr, (char *)"%s - %d - %d - %d", sSensor->chName, sSensor->u8Address, sSensor->u8StartRegAddr, sSensor->u8RegNum);
}

uint16_t Sensors_GetData(uint8_t *u8Data)
{
	uint16_t u16Index = 0;
	for (int i = 0; i < LinkList_GetSize(m_sSensors); i++)
	{
		Sensor *sSensor = Sensors_GetSensor(i);
		memcpy(u8Data + u16Index, sSensor->u8Data, DATA_SIZE);
		u16Index += sSensor->u8RegNum;
	}

	u8Data[u16Index] = '\0';
	return u16Index;
}
