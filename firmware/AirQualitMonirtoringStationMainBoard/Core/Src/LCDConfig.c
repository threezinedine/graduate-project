/*
 * LCDConfig.c
 *
 *  Created on: Dec 4, 2023
 *      Author: Acer
 */

#include "LCDConfig.h"
#include "LCD.h"

extern I2C_HandleTypeDef hi2c2;

void I2CSendData(uint8_t *u8Data, uint8_t u8Length)
{
	HAL_I2C_Master_Transmit(&hi2c2, LCD_GetAddress(), u8Data, u8Length, 50);
}
