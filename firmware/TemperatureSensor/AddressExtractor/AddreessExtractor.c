/*
 * AddreessExtractor.c
 *
 *  Created on: Jan 15, 2024
 *      Author: Acer
 */

#include "AddressExtractor.h"


static GPIO_TypeDef* firstBitPort;
static uint32_t firstBitPin;

static GPIO_TypeDef* secondBitPort;
static uint32_t secondBitPin;

static GPIO_TypeDef* thirdBitPort;
static uint32_t thirdBitPin;


void I2CAddres_SetFirstBit(GPIO_TypeDef* port, uint32_t pin)
{
	firstBitPort = port;
	firstBitPin = pin;
}

void I2CAddres_SetSecondBit(GPIO_TypeDef* port, uint32_t pin)
{
	secondBitPort = port;
	secondBitPin = pin;
}

void I2CAddres_SetThirdBit(GPIO_TypeDef* port, uint32_t pin)
{
	thirdBitPort = port;
	thirdBitPin = pin;
}

uint16_t I2CAddress_GetAddress()
{
	return (HAL_GPIO_ReadPin(firstBitPort, firstBitPin) |
				HAL_GPIO_ReadPin(secondBitPort, secondBitPin) << 1 |
				HAL_GPIO_ReadPin(firstBitPort, thirdBitPin) << 2) & 0x07;
}
