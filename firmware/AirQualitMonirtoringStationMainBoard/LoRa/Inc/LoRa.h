#ifndef _LORA_HEADER
#define _LORA_HEADER

#include "main.h"

void LoRa_Init(UART_HandleTypeDef *huart,
				GPIO_TypeDef *md0_port, uint16_t md0_pin,
				GPIO_TypeDef *md1_port, uint16_t md1_pin);
void LoRa_Release(void);

void LoRa_Send(uint8_t *data, uint16_t size);

#endif
