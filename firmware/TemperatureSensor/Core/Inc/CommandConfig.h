/*
 * CommandConfig.h
 *
 *  Created on: Feb 12, 2024
 *      Author: Acer
 */

#ifndef INC_COMMANDCONFIG_H_
#define INC_COMMANDCONFIG_H_

#include "main.h"

void Command_DefaultResponse(uint8_t argc, char **argv);
void Command_AnalogInputConfig(uint8_t argc, char **argv);
void Command_TimeTrigger(uint8_t argc, char **argv);
void Command_AddressInfo(uint8_t argc, char **argv);

#endif /* INC_COMMANDCONFIG_H_ */
