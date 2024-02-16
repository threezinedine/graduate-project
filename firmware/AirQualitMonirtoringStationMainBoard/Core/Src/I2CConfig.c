/*
 * I2CConfig.c
 *
 *  Created on: Dec 5, 2023
 *      Author: Acer
 */

#include "main.h"

extern void HAL_I2C_MasterListenCpltCallback(I2C_HandleTypeDef * hi2c)
{

}

extern void HAL_I2C_MasterRxCpltCallback (I2C_HandleTypeDef * hi2c)
{
  // RX Done .. Do Something!
}
