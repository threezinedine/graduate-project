/*
 * TimeTrigger.h
 *
 *  Created on: Jan 6, 2024
 *      Author: Acer
 */

#ifndef TIMETRIGGER_H_
#define TIMETRIGGER_H_

#define TIME_TRIGGERD		0x01
#define TIME_NOT_TRIGGERED	0x00

#include "main.h"

typedef struct
{
	uint32_t u32TriggeredTime;
	uint32_t u32CurrentTick;
} TimeTrigger_Ins;


void TimeTrigger_Reset(TimeTrigger_Ins* ins);
uint8_t TimeTrigger_IsTrigger(TimeTrigger_Ins* ins);


#endif /* TIMETRIGGER_H_ */
