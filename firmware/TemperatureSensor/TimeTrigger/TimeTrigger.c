/*
 * TimeTrigger.c
 *
 *  Created on: Jan 6, 2024
 *      Author: Acer
 */


#include "TimeTrigger.h"
#include "main.h"


void TimeTrigger_Reset(TimeTrigger_Ins* ins)
{
	ins->u32CurrentTick = HAL_GetTick();
}

uint8_t TimeTrigger_IsTrigger(TimeTrigger_Ins* ins)
{
	uint32_t u32NewCurrent = HAL_GetTick();

	if (u32NewCurrent > ins->u32CurrentTick || u32NewCurrent < ins->u32CurrentTick)
	{
		return TIME_TRIGGERD;
	}
	else
	{
		return TIME_NOT_TRIGGERED;
	}
}
