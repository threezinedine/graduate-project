/*
 * CommandConfig.h
 *
 *  Created on: Dec 4, 2023
 *      Author: Acer
 */

#ifndef INC_COMMANDCONFIG_H_
#define INC_COMMANDCONFIG_H_

#define DATA_FRAME_SIZE				64

#define READ_REGISTER_FUNCTION		0x01
#define WRITE_REGISTER_FUNCTION		0x02
#define READ_REGISTERS_FUNCTION		0x03
#define WRITE_REGISTERS_FUNCTION	0x04

#define I2C_OK						0x00
#define I2C_NO_RESPONSE				0x01
#define I2C_FUNCTION_NOT_REGISTERED	0x02
#define I2C_REGISTER_ADDR_ERROR		0x03

#include "main.h"

void Command_DefaultResponse(uint8_t argc, char **argv);
void Command_LCDCommandResponse(uint8_t argc, char **argv);
void Command_I2CCommandResponse(uint8_t argc, char **argv);
void Command_SensorsCommandResponse(uint8_t argc, char **argv);
void Command_LoRaCommandResponse(uint8_t argc, char **argv);
void Command_SPICommandResponse(uint8_t argc, char **argv);
void Command_MainCommandResponse(uint8_t argc, char **argv);


#endif /* INC_COMMANDCONFIG_H_ */
