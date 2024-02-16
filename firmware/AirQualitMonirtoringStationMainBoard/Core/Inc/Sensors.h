/*
 * Sensors.h
 *
 *  Created on: Dec 7, 2023
 *      Author: Acer
 */

#ifndef INC_SENSORS_H_
#define INC_SENSORS_H_

#include "main.h"

typedef struct
{
	char chName[12];
	uint8_t u8Address;
	uint8_t u8StartRegAddress;
	uint8_t u8RegAddressLength;
	uint8_t u8Data[32];
} SensorConfig;

void GetSensorList(char *chSensorListStr);


#endif /* INC_SENSORS_H_ */
