/*
 * CommandConfig.c
 *
 *  Created on: Dec 4, 2023
 *      Author: Acer
 */

#include "CommandConfig.h"
#include "Command.h"
#include "string.h"
#include "StringUtils.h"
#include "LCD.h"
#include "SensorList.h"
#include "stdlib.h"
#include "LoRa.h"
#include "sx1278.h"
#include "MainProcess.h"

typedef union
{
	float value;
	uint8_t buffer[4];
} F32;

extern I2C_HandleTypeDef hi2c1;
extern uint16_t u16LoRa_status;
extern SPI_HandleTypeDef hspi1;
extern char chStationId[];
extern uint32_t u32UpdateTime;


void Command_DefaultResponse(uint8_t argc, char **argv)
{
	Command_ResponseConstChar("Unknown command");
}

void Command_LCDCommandResponse(uint8_t argc, char **argv)
{
	switch (argc)
	{
	case 0:
		Command_ResponseConstChar("Choose 1 action: clear, test, reset");
		break;
	case 1:
		if (strcmp(argv[0], "clear") == 0)
		{
			LCD_Clear();
			Command_ResponseConstChar("Clear the LCD command is sent, check the result");
		}
		else if (strcmp(argv[0], "test") == 0)
		{
			LCD_SendString("LCD Testing");
			Command_ResponseConstChar("A string is sent to the LCD, check the result");
		}
		else if (strcmp(argv[0], "reset") == 0)
		{
			LCD_Init();
			Command_ResponseConstChar("Reset the LCD command is sent, check the result");
		}
		else
		{
			Command_ResponseConstChar("Your option must be in [clear, test, reset]");
		}
		break;
	case 2:
		if (strcmp(argv[0], "data") == 0)
		{
			uint8_t u8Data = ConvertHexStringToHex(argv[1]);
			LCD_SendData(u8Data);
			char chResponse[64];
			FormatString(chResponse, "Send data %s to the LCD, check result", argv[1]);
			Command_ResponseConstChar(chResponse);
		}
		else if (strcmp(argv[0], "command") == 0)
		{
			uint8_t u8Data = ConvertHexStringToHex(argv[1]);
			LCD_SendCommand(u8Data);
			char chResponse[64];
			FormatString(chResponse, "Send command %s to the LCD, check result", argv[1]);
			Command_ResponseConstChar(chResponse);
		}
		else
		{
			Command_ResponseConstChar("Your option must be in [data, command]");
		}
		break;
	default:
		Command_ResponseConstChar("Invalid Command");
	}
}

void Command_I2CCommandResponse(uint8_t argc, char **argv)
{
	uint8_t u8Address;
	char chLCDString[28];
	uint8_t u8Response[2];
	char commandRes[64];
	uint8_t u8Data[4];
	char hexStr[4];
	memset(u8Response, 0, 2);

	GoToConfig();

	switch(argc)
	{
	case 0:
	case 1:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Specify the address and data data to send");
		break;
	case 2:
		if (strcmp(argv[0], "get-mul") == 0)
		{
			LCD_Clear();
			u8Address = ConvertHexStringToHex(argv[1]) << 1;
			u8Data[0] = 4;
			u8Data[1] = READ_REGISTERS_FUNCTION;
			u8Data[2] = 0x00;
			u8Data[3] = 0x03;
			HAL_I2C_Master_Transmit(
							&hi2c1, u8Address,
							u8Data, u8Data[0], 300);
			while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);
			if (HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
			{
				HAL_Delay(50);
				HAL_I2C_Master_Receive(&hi2c1, u8Address, u8Response, u8Data[3] + 1, 100);
				while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);
				if (HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
				{
					LCD_SetCursor(0, 0);
					FormatString(chLCDString, "Status: %d", u8Response[0]);
					LCD_SendString(chLCDString);
					LCD_SetCursor(1, 0);
					FormatString(chLCDString, "%d, %d, %d", u8Response[1], u8Response[2], u8Response[3]);
					LCD_SendString(chLCDString);
					Command_ResponseConstChar("Frame is sent.");
				}
				else
				{
					LCD_SetCursor(0, 0);
					LCD_SendString("Error occurs");
					Command_ResponseConstChar("Error occurs");
				}

			}
			else
			{
				LCD_SetCursor(0, 0);
				LCD_SendString("Error occurs");
				Command_ResponseConstChar("Error occurs");
			}
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 3:
		LCD_Clear();
		u8Address = ConvertHexStringToHex(argv[1]) << 1;

		if (strcmp(argv[0], "read") == 0)
		{
			u8Data[0] = 3;
			u8Data[1] = READ_REGISTER_FUNCTION;
			u8Data[2] = ConvertHexStringToHex(argv[2]);
			HAL_I2C_Master_Transmit(
							&hi2c1, u8Address,
							u8Data, u8Data[0], 300);
			HAL_Delay(50);
			while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);

			if (HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
			{
				hi2c1.ErrorCode = HAL_I2C_ERROR_NONE;
				HAL_Delay(500);
				HAL_I2C_Master_Receive(&hi2c1, u8Address, u8Response, 2, 2000);
				while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);

				if (HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
				{
					LCD_SetCursor(0, 0);
					FormatString(chLCDString, "Status: %d", u8Response[0]);
					LCD_SendString(chLCDString);

					LCD_SetCursor(1, 0);
					ConvertHexToString(hexStr, u8Response[1]);
					FormatString(chLCDString, "Value: %s", hexStr);
					LCD_SendString(chLCDString);

					FormatString(commandRes, "Status: %d - Value: %s", u8Response[0], hexStr);

					Command_ResponseConstChar(commandRes);
				}
				else
				{
					LCD_SetCursor(0, 0);
					LCD_SendString("Error occurs");
					Command_ResponseConstChar("Error occurs");

				}
			}
			else {
				LCD_SetCursor(0, 0);
				LCD_SendString("Error occurs");
				Command_ResponseConstChar("Error occurs");
			}
//
//			HAL_I2C_DeInit(&hi2c1);
//			HAL_I2C_Init(&hi2c1);

			hi2c1.ErrorCode = HAL_I2C_ERROR_NONE;
		}
		else if (strcmp(argv[0], "write-mul") == 0)
		{
			u8Data[1] = WRITE_REGISTERS_FUNCTION;
			u8Data[2] = 0x00;
			u8Data[3] = 0x03;
			u8Data[0] = 4 + u8Data[3];
			uint8_t u8NewValue = ConvertHexStringToHex(argv[2]);
			for (int i=0; i<u8Data[3]; i++)
			{
				u8Data[4 + i] = u8NewValue;
			}
			HAL_I2C_Master_Transmit(
							&hi2c1, u8Address,
							u8Data, u8Data[0], 300);
			while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);
			if (HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
			{
				HAL_Delay(50);
				HAL_I2C_Master_Receive(&hi2c1, u8Address, u8Response, 1, 100);
				while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);
				while(HAL_I2C_GetError(&hi2c1) == HAL_I2C_ERROR_AF);
				LCD_SetCursor(0, 0);
				FormatString(chLCDString, "Status: %d", u8Response[0]);
				LCD_SendString(chLCDString);
				Command_ResponseConstChar("Frame is sent successfully.");
			}
			else
			{
				LCD_SetCursor(0, 0);
				LCD_SendString("Error occurs");
				Command_ResponseConstChar("Error occurs");
			}
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command - read");
		}
		break;
	case 4:
		u8Address = ConvertHexStringToHex(argv[1]) << 1;
		u8Data[0] = 4;
		u8Data[1] = WRITE_REGISTER_FUNCTION;
		u8Data[2] = ConvertHexStringToHex(argv[2]);
		u8Data[3] = ConvertHexStringToHex(argv[3]);
		LCD_Clear();

		if (strcmp(argv[0], "write") == 0)
		{
			HAL_I2C_Master_Transmit(
							&hi2c1, u8Address,
							u8Data, u8Data[0], 300);
			while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);
			if (HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
			{
				HAL_Delay(50);
				HAL_I2C_Master_Receive(&hi2c1, u8Address, u8Response, 1, 100);

				while (HAL_I2C_GetState(&hi2c1) != HAL_I2C_STATE_READY);
				if (HAL_I2C_GetError(&hi2c1) != HAL_I2C_ERROR_AF)
				{
					LCD_SetCursor(0, 0);
					FormatString(chLCDString, "Status: %d", u8Response[0]);
					LCD_SendString(chLCDString);
					Command_ResponseConstChar("Frame is sent.");
				}
				else
				{
					LCD_SetCursor(0, 0);
					LCD_SendString("Error occurs");
					Command_ResponseConstChar("Error occurs");
				}
			}
			else
			{
				LCD_SetCursor(0, 0);
				LCD_SendString("Error occurs");
				Command_ResponseConstChar("Error occurs");
			}
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command - read");
		}
		break;
	default:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Invalid command");
	}
}

void Command_SensorsCommandResponse(uint8_t argc, char **argv)
{
	GoToConfig();
	char chResponse[64];
	uint8_t u8NumSensors = Sensors_Length();
	LCD_Clear();
	LCD_SetCursor(0, 0);

	switch(argc)
	{
	case 0:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Invalid command");
		break;
	case 1:
		if (strcmp(argv[0], "count") == 0)
		{
			FormatString(chResponse, "Count: %d", u8NumSensors);
			LCD_SendString(chResponse);
			Command_ResponseConstChar(chResponse);
		}
		else if (strcmp(argv[0], "clear") == 0)
		{
			Sensors_Clear();
			LCD_SendString("Success");
			Command_ResponseConstChar("Success");
		}
		else if (strcmp(argv[0], "data") == 0)
		{
			uint8_t *u8Data = (uint8_t *)malloc(sizeof(uint8_t) * DATA_SIZE * u8NumSensors);
			char chResponse[128];
			Sensors_GetData(u8Data);
			F32 res = { .value = 0 };
			res.buffer[0] = u8Data[0];
			res.buffer[1] = u8Data[1];
			res.buffer[2] = u8Data[2];
			res.buffer[3] = u8Data[3];

			F32 res2 = { .value = 0 };
			res2.buffer[0] = u8Data[4];
			res2.buffer[1] = u8Data[5];
			res2.buffer[2] = u8Data[6];
			res2.buffer[3] = u8Data[7];

			LCD_SendString("Success");
			FormatString(chResponse, "%d, %d, %d, %d, %d, %d, %d, %d - Test Float: %d - Test Float: %d",
							u8Data[0], u8Data[1], u8Data[2], u8Data[3],
							u8Data[4], u8Data[5], u8Data[6], u8Data[7],
							(int)(res.value * 100), (int)(res2.value * 100));

			Command_ResponseConstChar(chResponse);
			free(u8Data);
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 2:
		if (strcmp(argv[0], "info") == 0)
		{
			LCD_SendString("Sensor: ");
			LCD_SetCursor(1, 0);
			if (ConvertHexStringToHex(argv[1]) < Sensors_Length())
			{
				Sensors_ToString(ConvertHexStringToHex(argv[1]), chResponse);
				LCD_SendString(chResponse);
				Command_ResponseConstChar(chResponse);
			}
			else
			{
				LCD_SendString("Out of range");
				Command_ResponseConstChar("Out of range");
			}
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 5:
		if (strcmp(argv[0], "add") == 0)
		{
			Sensors_AddSensors(argv[1]);
			Sensor *sSensor = Sensors_GetSensor(Sensors_Length() - 1);
			sSensor->u8Address = ConvertHexStringToHex(argv[2]);
			sSensor->u8StartRegAddr = ConvertHexStringToHex(argv[3]);
			sSensor->u8RegNum = ConvertHexStringToHex(argv[4]);
			LCD_SendString("Success");
			Command_ResponseConstChar("Success");
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	default:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Invalid command");
	}
}

void Command_LoRaCommandResponse(uint8_t argc, char **argv)
{
	char chResponse[40];
	LCD_Clear();

	switch(argc)
	{
	case 0:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Invalid command");
		break;
	case 2:
		if (strcmp(argv[0], "send") == 0)
		{
			FormatString(chResponse, "Sent");

			LoRa_Send((uint8_t*)argv[1], strlen(argv[1]));

			Command_ResponseConstChar(chResponse);
			LCD_SendString(chResponse);
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	default:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Invalid command");
	}
}

void Command_SPICommandResponse(uint8_t argc, char **argv)
{
	char chResponse[24];
	uint8_t data = 0xff;
	uint8_t address;
	uint8_t newValue = 0;
	LCD_Clear();

	switch(argc)
	{
	case 0:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Invalid command");
		break;
	case 1:
		if (strcmp(argv[0], "off-test") == 0)
		{
			LCD_SendString("Done");
			Command_ResponseConstChar("Done");
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 2:
		if (strcmp(argv[0], "read") == 0)
		{
			address = ConvertHexStringToHex(argv[1]) & 0x7F;
			HAL_GPIO_WritePin(NSS_GPIO_Port, NSS_Pin, GPIO_PIN_RESET);

			HAL_SPI_Transmit(&hspi1, &address, 1, 100);
			while (HAL_SPI_GetState(&hspi1) != HAL_SPI_STATE_READY);

//			HAL_SPI_Receive(&hspi1, &data, 1, 100);
			HAL_SPI_TransmitReceive(&hspi1, &newValue, &data, 1, 100);
			while (HAL_SPI_GetState(&hspi1) != HAL_SPI_STATE_READY);

			HAL_GPIO_WritePin(NSS_GPIO_Port, NSS_Pin, GPIO_PIN_SET);

			FormatString(chResponse, "Result: %d", data);
			LCD_SendString(chResponse);
			Command_ResponseConstChar(chResponse);
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 3:
		if (strcmp(argv[0], "write") == 0)
		{
			address = ConvertHexStringToHex(argv[1]) | 0x80;
			newValue = ConvertHexStringToHex(argv[2]);
			HAL_GPIO_WritePin(NSS_GPIO_Port, NSS_Pin, GPIO_PIN_RESET);

			HAL_SPI_Transmit(&hspi1, &address, 1, 100);
			while (HAL_SPI_GetState(&hspi1) != HAL_SPI_STATE_READY);

			HAL_SPI_TransmitReceive(&hspi1, &newValue, &data, 1, 100);
			while (HAL_SPI_GetState(&hspi1) != HAL_SPI_STATE_READY);

			HAL_GPIO_WritePin(NSS_GPIO_Port, NSS_Pin, GPIO_PIN_SET);

			FormatString(chResponse, "Done");
			LCD_SendString(chResponse);
			Command_ResponseConstChar(chResponse);
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	default:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Invalid command");
	}
}

void Command_MainCommandResponse(uint8_t argc, char **argv)
{
	char chResponse[20] = "";

	LCD_Clear();

	switch(argc)
	{
	case 0:
			GetCurrentState(chResponse);
			LCD_SendString(chResponse);
			Command_ResponseConstChar(chResponse);
		break;
	case 1:
		if (strcmp(argv[0], "id") == 0)
		{
			FormatString(chResponse, "Id: %s", chStationId);

			LCD_SendString(chResponse);
			Command_ResponseConstChar(chResponse);
		}
		else if (strcmp(argv[0], "update-time") == 0)
		{
			FormatString(chResponse, "Updated Time: %d seconds", u32UpdateTime);

			LCD_SendString(chResponse);
			Command_ResponseConstChar(chResponse);
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 2:
		if (strcmp(argv[0], "mode") == 0 && strcmp(argv[1], "config") == 0)
		{
			GoToConfig();
			LCD_SendString("To config mode");
			Command_ResponseConstChar("To config mode");
		}
		else if (strcmp(argv[0], "mode") == 0 && strcmp(argv[1], "running") == 0)
		{
			GoToRun();
			LCD_SendString("To running mode");
			Command_ResponseConstChar("To running mode");
		}
		else if (strcmp(argv[0], "id") == 0)
		{
			memset(chStationId, 0, ID_LENGTH);
			memcpy(chStationId, argv[1], strlen(argv[1]));
			LCD_SendString("Successfully");
			Command_ResponseConstChar("Successfully");
		}
		else if (strcmp(argv[0], "update-time") == 0)
		{
			char *endptr;
			long result = strtol(argv[1], &endptr, 10);
			if (*endptr != '\0' && *endptr != '\n') {
				LCD_SendString("Conversion error: Not a valid integer\n");
				Command_ResponseConstChar("Conversion error: Not a valid integer\n");
			}
			else
			{
				u32UpdateTime = result;
				LCD_SendString("Successfully");
				Command_ResponseConstChar("Successfully");
			}
		}
		else
		{
			LCD_SendString("Invalid command");
			Command_ResponseConstChar("Invalid command");
		}
		break;
	default:
		LCD_SendString("Invalid command");
		Command_ResponseConstChar("Invalid command");
	}
}
