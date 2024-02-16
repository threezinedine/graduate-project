/*
 * I2CCommunicationConfig.h
 *
 *  Created on: Dec 4, 2023
 *      Author: Acer
 */

#ifndef INC_I2CCOMMUNICATIONCONFIG_H_
#define INC_I2CCOMMUNICATIONCONFIG_H_

#include "Communication.h"
#include "Queue.h"

#define DATA_FRAME_SIZE				64

#define READ_REGISTER_FUNCTION		0x01
#define WRITE_REGISTER_FUNCTION		0x02
#define READ_REGISTERS_FUNCTION		0x03
#define WRITE_REGISTERS_FUNCTION	0x04

#define I2C_OK						0x00
#define I2C_NO_RESPONSE				0x01
#define I2C_FUNCTION_NOT_REGISTERED	0x02
#define I2C_REGISTER_ADDR_ERROR		0x03

typedef struct
{
	uint8_t u8Data[DATA_FRAME_SIZE];
	uint8_t u8Length;
} I2C_TransmitFrame;

void I2C_TransmitFunction(uint8_t *u8Data, uint8_t u8Length);
void I2C_HandlerFunction(Communication_Instance *sInstance,
							uint8_t *u8Data, uint8_t u8Length);


#endif /* INC_I2CCOMMUNICATIONCONFIG_H_ */
