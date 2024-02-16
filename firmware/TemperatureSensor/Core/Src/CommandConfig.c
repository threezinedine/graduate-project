/*
 * CommandConfig.c
 *
 *  Created on: Feb 12, 2024
 *      Author: Acer
 */

#include "CommandConfig.h"
#include "Command.h"
#include "string.h"
#include "AnalogInput.h"
#include "TimeTrigger.h"
#include "AddressExtractor.h"
#include "StringUtils.h"

extern TimeTrigger_Ins ins;

static float customAtof(char *str);


void Command_DefaultResponse(uint8_t argc, char **argv)
{
	Command_ResponseConstChar("Unknown command");
}

static uint8_t GetIndexFromString(char *argv);

void Command_AnalogInputConfig(uint8_t argc, char **argv)
{
	char response[256];
	uint8_t index = 0;

	switch (argc)
	{
	case 0:
		AnalogInput_GetStatusString(response, 256);
		Command_Response(response, strlen(response));
		break;
	case 1:
		uint8_t data[16];
		memset(data, 0, 16);
		AnalogInput_GetData(data);
		if (strcmp(argv[0], "test") == 0)
		{
			FormatString(response, "Data:");
			for (int i=0; i<16; i++)
			{
				FormatString(response, "%s %02X", response, data[i]);
			}
			Command_ResponseConstChar(response);
		}
		else
		{
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 2:
		if (strcmp(argv[0], "on") == 0)
		{
			index = GetIndexFromString(argv[1]);
			if (index == -1) return;

			AnalogInput_TurnOn(index);
			Command_ResponseConstChar("Setup successfully");
		}
		else if (strcmp(argv[0], "off") == 0)
		{
			index = GetIndexFromString(argv[1]);
			if (index == -1) return;

			AnalogInput_TurnOff(index);
			Command_ResponseConstChar("Setup successfully");
		}
		else
		{
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 3:
		if (strcmp(argv[0], "type") == 0)
		{
			index = GetIndexFromString(argv[1]);
			if (index == -1) return;

			uint8_t typeVal = AnalogInput_TypeFromString(argv[2]);
			if (typeVal == -1)
			{
				Command_ResponseConstChar("Type must be in: int8, uint8, "
											"int16, uint16, "
											"int32, uint32, "
											"float32.");
				return;
			}
			AnalogInput_SetType(index, typeVal);
			Command_ResponseConstChar("Setup successfully");
		}
		else if (strcmp(argv[0], "input-max") == 0)
		{
			index = GetIndexFromString(argv[1]);
			if (index == -1) return;

			AnalogInput_SetInputMax(index, customAtof(argv[2]));
			Command_ResponseConstChar("Setup successfully");
		}
		else
		{
			Command_ResponseConstChar("Invalid command");
		}
		break;
	case 4:
		if (strcmp(argv[0], "minmax") == 0)
		{
			index = GetIndexFromString(argv[1]);
			if (index == -1) return;

			AnalogInput_SetMinAndMax(index, customAtof(argv[2]), customAtof(argv[3]));
			Command_ResponseConstChar("Setup successfully");
		}
		else
		{
			Command_ResponseConstChar("Invalid command");
		}
		break;
	default:
		Command_ResponseConstChar("Invalid command");
	}
}

uint8_t GetIndexFromString(char *argv)
{
	if (argv[0] > '3' || argv[0] < '0')
	{
		Command_ResponseConstChar("Index must be from 0 - 3");
		return -1;
	}
	return argv[0] - '0';
}

float customAtof(char *str) {
    float result = 0.0;
    int i = 0;
    int sign = 1;

    // Handle leading whitespaces
    while (str[i] == ' ' || str[i] == '\t' || str[i] == '\n')
        i++;

    // Handle sign
    if (str[i] == '-' || str[i] == '+') {
        sign = (str[i] == '-') ? -1 : 1;
        i++;
    }

    // Process digits before the decimal point
    while (str[i] >= '0' && str[i] <= '9') {
        result = result * 10 + (str[i] - '0');
        i++;
    }

    // Process the decimal point and digits after it
    if (str[i] == '.') {
        float fraction = 0.1;

        i++;  // Move past the decimal point

        while (str[i] >= '0' && str[i] <= '9') {
            result += (str[i] - '0') * fraction;
            fraction *= 0.1;
            i++;
        }
    }

    return sign * result;
}

static uint32_t customAtoi(char *str);

void Command_TimeTrigger(uint8_t argc, char **argv)
{
	char response[32];

	switch (argc)
	{
	case 0:
		FormatString(response, "TriggerTime: %d ms", ins.u32TriggeredTime);
		Command_ResponseConstChar(response);
		break;
	case 1:
		ins.u32TriggeredTime = customAtoi(argv[0]);
		FormatString(response, "TriggerTime: %d ms", ins.u32TriggeredTime);
		Command_ResponseConstChar(response);
		break;
	default:
		Command_ResponseConstChar("Invalid command");
	}
}

uint32_t customAtoi(char *str) {
    uint32_t result = 0;
    int i = 0;

    // Handle leading whitespaces
    while (str[i] == ' ' || str[i] == '\t' || str[i] == '\n')
        i++;

    // Process digits
    while (str[i] >= '0' && str[i] <= '9') {
        result = result * 10 + (str[i] - '0');
        i++;
    }

    return result;
}

void Command_AddressInfo(uint8_t argc, char **argv)
{
	if (argc != 0)
	{
		Command_ResponseConstChar("Invalid command");
		return;
	}

	char response[32];
	FormatString(response, "I2C address: %d", I2CAddress_GetAddress());
	Command_ResponseConstChar(response);
	return;
}
