/*
 * test.h
 *
 *  Created on: Feb 3, 2024
 *      Author: Acer
 */

#ifndef INC_TEST_H_
#define INC_TEST_H_

#include "main.h"

typedef enum
{
	UINT16,
	INT16,
	UINT32,
	INT32,
	FLOAT32,
} AnalogInputType;

typedef union
{
	uint16_t u16Val;
	int16_t i16Val;
	uint32_t u32Val;
	int32_t i32Val;
	float f32Val;
} AnalogOutputValue;

typedef struct
{
	uint8_t u8Index;
	uint8_t u8Active;
	float fInputMin;
	float fInputMax;

	AnalogInputType eOutputType;
	AnalogOutputValue eOutputMin;
	AnalogOutputValue eOutputMax;
} AnalogInput;

#endif /* INC_TEST_H_ */
