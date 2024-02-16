/*
 * MainProcess.c
 *
 *  Created on: Jan 15, 2024
 *      Author: Acer
 */


#include "MainProcess.h"
#include "main.h"
#include "SensorList.h"
#include "StringUtils.h"
#include "stdlib.h"
#include "CommandConfig.h"
#include "string.h"
#include "LoRa.h"


typedef enum
{
	Initializing,
	Config,
	Running,
} State;

static State eCurrentState = Initializing;
static uint32_t current;

static void GetDataCallback(Sensor *sSensor);
static void RunningSection();

extern I2C_HandleTypeDef hi2c1;
extern UART_HandleTypeDef huart1;
extern char chStationId[];
extern uint32_t u32UpdateTime;


void MainProcess()
{
	switch(eCurrentState)
	{
	case Initializing:
		break;
	case Config:
		break;
	case Running:
		RunningSection();
		break;
	}
}

void RunningSection()
{
	uint32_t newCurrent = HAL_GetTick();

	if ((newCurrent - current > u32UpdateTime * 1000) || (newCurrent < current))
	{
		if (Sensors_Length() > 0)
		{
		  Sensors_ForEach(GetDataCallback);
		  current = newCurrent;
		  char chData[128];
		  char chSensorData[64];

		  uint16_t length = Sensors_GetData((uint8_t*)chSensorData);
		  FormatString(chData, "%s;", chStationId);

		  // sent packet has the format
		  // stationId;<byte-array>
		  for (int i = 0; i< length; i++)
		  {
			  FormatString(chData, "%s %02X", chData, chSensorData[i]);
		  }

		  FormatString(chData, "%s\n", chData);

		  LoRa_Send((uint8_t*)chData, strlen(chData));
//		  HAL_UART_Transmit(&huart1, (uint8_t*)chData, strlen(chData), 100);
		}
	}
}


void InitializeSuccess()
{
	eCurrentState = Running;
	current = HAL_GetTick();
}

void GoToConfig()
{
	eCurrentState = Config;
}

void GoToRun()
{
	eCurrentState = Running;
}

void GetCurrentState(char* chState)
{
	switch(eCurrentState)
	{
	case Initializing:
		FormatString(chState, "State: Initialize");
		break;
	case Config:
		FormatString(chState, "State: Config");
		break;
	case Running:
		FormatString(chState, "State: Running");
		break;
	default:
		FormatString(chState, "Error");
	}
}

void GetDataCallback(Sensor *sSensor)
{
	uint8_t u8Response[33];
	uint8_t u8Data[4];
	memset(u8Response, 0, 33);
	u8Data[0] = 4;
	u8Data[1] = READ_REGISTERS_FUNCTION;
	u8Data[2] = sSensor->u8StartRegAddr;
	u8Data[3] = sSensor->u8RegNum;
	HAL_I2C_Master_Transmit(
					&hi2c1, sSensor->u8Address << 1,
					u8Data, u8Data[0], 1000);
	while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);
	if(HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
	{
		HAL_Delay(300);
		HAL_I2C_Master_Receive(&hi2c1, sSensor->u8Address << 1,
										u8Response,
										1 + sSensor->u8RegNum,
										2000);
		while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);
		if (HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
		{
			memcpy(sSensor->u8Data, u8Response + 1, 32);
		}
	}
}