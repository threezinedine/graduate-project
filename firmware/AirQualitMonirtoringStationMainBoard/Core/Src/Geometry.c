/*
 * Geometry.c
 *
 *  Created on: Dec 7, 2023
 *      Author: Acer
 */

#include "main.h"
#include "Communication.h"
#include "Geometry.h"

extern Communication_Instance *sGeometry;
extern uint8_t u8RxData;
//extern UART_HandleTypeDef huart2;

uint8_t u8RxBuffer[128];
uint8_t u8RxIndex = 0;
GPS_t GPS;

//extern void HAL_UART_RxCpltCallback(UART_HandleTypeDef *huart)
//{
//	if (u8RxData != '\n')
//	{
//		u8RxBuffer[u8RxIndex++] = u8RxData;
//	}
//	else
//	{
//		Communication_ReceiveData(sGeometry, u8RxBuffer, u8RxIndex);
//		u8RxIndex = 0;
//	}
//	HAL_UART_Receive_IT(&huart2, &u8RxData, 1);
//}
//
//
//void GeometryHandler(Communication_Instance *sInstance,
//						uint8_t *u8Data, uint8_t u8Length)
//{
//
//}
