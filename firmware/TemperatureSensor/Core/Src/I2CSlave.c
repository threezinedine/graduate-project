/*
 * I2CSlave.c
 *
 *  Created on: Dec 4, 2023
 *      Author: Acer
 */


#include "I2CSlave.h"
#include "stm32f1xx_hal_i2c.h"
#include "Communication.h"
#include "I2CCommunicationConfig.h"
#include "stdlib.h"

#define RX_BUFFER_SIZE	11

uint8_t u8RxBuffer[RX_BUFFER_SIZE];

extern I2C_HandleTypeDef hi2c1;
extern Communication_Instance *sI2CCommunicationIns;
extern Queue *m_sTransmitQueue;

uint8_t m_u8Index;

extern void HAL_I2C_ListenCpltCallback(I2C_HandleTypeDef *hi2c)
{
	HAL_I2C_EnableListen_IT(hi2c);
}

void HAL_I2C_AddrCallback(I2C_HandleTypeDef *hi2c, uint8_t TransferDirection, uint16_t AddrMatchCode)
{
	if (TransferDirection == I2C_DIRECTION_TRANSMIT)  // if the master wants to transmit the data
	{
		m_u8Index = 0;
		HAL_I2C_Slave_Seq_Receive_IT(hi2c, u8RxBuffer, 1, I2C_FIRST_FRAME);
	}
	else
	{
		if (!Queue_IsEmpty(m_sTransmitQueue))
		{
			I2C_TransmitFrame *sFrame = Queue_DeQueue(m_sTransmitQueue);
			HAL_I2C_Slave_Seq_Transmit_IT(hi2c, sFrame->u8Data,
											sFrame->u8Length,
											I2C_FIRST_AND_LAST_FRAME);
			free(sFrame);
		}
	}
}

void HAL_I2C_SlaveRxCpltCallback(I2C_HandleTypeDef *hi2c)
{
	m_u8Index ++;

	if (m_u8Index < u8RxBuffer[0])
	{
		if (m_u8Index < u8RxBuffer[0] - 1)
		{
			HAL_I2C_Slave_Seq_Receive_IT(hi2c, u8RxBuffer + m_u8Index, 1, I2C_NEXT_FRAME);
		}
		else
		{
			HAL_I2C_Slave_Seq_Receive_IT(hi2c, u8RxBuffer + m_u8Index, 1, I2C_LAST_FRAME);
		}
	}

	if (m_u8Index == u8RxBuffer[0])
	{
		Communication_ReceiveData(sI2CCommunicationIns, u8RxBuffer + 1, m_u8Index);
	}
}

void HAL_I2C_ErrorCallback(I2C_HandleTypeDef *hi2c)
{
	uint32_t errorcode = HAL_I2C_GetError(hi2c);
	if (errorcode == 0x04)
	{
		Communication_ReceiveData(sI2CCommunicationIns, u8RxBuffer + 1, m_u8Index);
	}

	HAL_I2C_EnableListen_IT(hi2c);
}
