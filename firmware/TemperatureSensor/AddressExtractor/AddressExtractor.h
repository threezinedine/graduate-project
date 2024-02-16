/*
 * AddressExtractor.h
 *
 *  Created on: Jan 15, 2024
 *      Author: Acer
 */

#ifndef ADDRESSEXTRACTOR_H_
#define ADDRESSEXTRACTOR_H_

#include "main.h"

void I2CAddres_SetFirstBit(GPIO_TypeDef* port, uint32_t pin);
void I2CAddres_SetSecondBit(GPIO_TypeDef* port, uint32_t pin);
void I2CAddres_SetThirdBit(GPIO_TypeDef* port, uint32_t pin);
uint16_t I2CAddress_GetAddress();

#endif /* ADDRESSEXTRACTOR_H_ */
