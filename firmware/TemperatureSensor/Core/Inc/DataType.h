/*
 * DataType.h
 *
 *  Created on: Dec 7, 2023
 *      Author: Acer
 */

#ifndef INC_DATATYPE_H_
#define INC_DATATYPE_H_

#define COIL_FALSE 		0x00
#define COIL_TRUE		0x0f

typedef union
{
	uint8_t value;
	uint8_t buffer[1];
} Bool;

typedef union
{
	uint8_t value;
	uint8_t buffer[1];
} U8;

typedef union
{
	uint16_t value;
	uint8_t buffer[2];
} U16;

typedef union
{
	uint32_t value;
	uint8_t buffer[4];
} U32;

typedef union
{
	int8_t value;
	uint8_t buffer[1];
} I8;

typedef union
{
	int16_t value;
	uint8_t buffer[2];
} I16;

typedef union
{
	int32_t value;
	uint8_t buffer[4];
} I32;

typedef union
{
	float value;
	uint8_t buffer[4];
} F32;

#endif /* INC_DATATYPE_H_ */
