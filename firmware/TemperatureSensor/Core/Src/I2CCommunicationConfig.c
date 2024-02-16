/*
 * I2CCommunicationConfig.c
 *
 *  Created on: Dec 4, 2023
 *      Author: Acer
 */


#include "I2CCommunicationConfig.h"
#include "main.h"
#include "string.h"
#include "Queue.h"
#include "DataType.h"
#include "stdlib.h"
#include "AnalogInput.h"

extern Queue *m_sTransmitQueue;

extern I2C_HandleTypeDef hi2c1;
static uint8_t u8Buffers[16];
static uint16_t u8BufferSize;

void I2C_TransmitFunction(uint8_t *u8Data, uint8_t u8Length)
{
	I2C_TransmitFrame *sFrame = (I2C_TransmitFrame *)malloc(sizeof(I2C_TransmitFrame));
	memcpy(sFrame->u8Data, u8Data, u8Length);
	sFrame->u8Length = u8Length;
	Queue_EnQueue(m_sTransmitQueue, (void *)sFrame);
}

void I2C_HandlerFunction(Communication_Instance *sInstance,
							uint8_t *u8Data, uint8_t u8Length)
{
	u8BufferSize = AnalogInput_GetDataLength();
	AnalogInput_GetData(u8Buffers);

	uint8_t u8Response[DATA_FRAME_SIZE];
	memset(u8Response, 0, DATA_FRAME_SIZE);
	switch (u8Data[0]) {
		case READ_REGISTERS_FUNCTION:
			if (u8Data[1] + u8Data[2] <= u8BufferSize)
			{
				u8Response[0] = I2C_OK;
				for (int i=0; i<u8Data[2]; i++)
				{
					u8Response[i + 1] = u8Buffers[u8Data[1] + i];
				}
			}
			else
			{
				u8Response[0] = I2C_REGISTER_ADDR_ERROR;
			}
			sInstance->fTransmitFunction(u8Response, u8Data[2] + 1);
			break;
		case WRITE_REGISTERS_FUNCTION:
			if (u8Data[1] + u8Data[2] <= u8BufferSize)
			{
				u8Response[0] = I2C_OK;
				for (int i=0; i<u8Data[2]; i++)
				{
					u8Buffers[u8Data[1] + i] = u8Data[3 + i];
				}
			}
			else
			{
				u8Response[0] = I2C_REGISTER_ADDR_ERROR;
			}
			sInstance->fTransmitFunction(u8Response, 1);
			break;
		case READ_REGISTER_FUNCTION:
			if (u8Data[1] < u8BufferSize)
			{
				u8Response[0] = I2C_OK;
				u8Response[1] = u8Buffers[u8Data[1]];
			}
			else
			{
				u8Response[0] = I2C_REGISTER_ADDR_ERROR;
			}
			sInstance->fTransmitFunction(u8Response, 2);
			break;
		case WRITE_REGISTER_FUNCTION:
			if (u8Data[1] < u8BufferSize)
			{
				u8Response[0] = I2C_OK;
				u8Buffers[u8Data[1]] = u8Data[2];
			}
			else
			{
				u8Response[0] = I2C_REGISTER_ADDR_ERROR;
			}
			sInstance->fTransmitFunction(u8Response, 1);
			break;
		default:
			u8Response[0] = I2C_FUNCTION_NOT_REGISTERED;
			sInstance->fTransmitFunction(u8Response, 1);
	}
}
