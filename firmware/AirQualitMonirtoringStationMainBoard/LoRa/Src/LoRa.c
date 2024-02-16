#include "LoRa.h"

static UART_HandleTypeDef *m_huart;


void LoRa_Init(UART_HandleTypeDef *huart,
				GPIO_TypeDef *md0_port, uint16_t md0_pin,
				GPIO_TypeDef *md1_port, uint16_t md1_pin)
{
	m_huart = huart;
	HAL_GPIO_WritePin(md0_port, md0_pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(md1_port, md1_pin, GPIO_PIN_RESET);
}

void LoRa_Release(void)
{

}

void LoRa_Send(uint8_t* data, uint16_t size)
{
	HAL_UART_Transmit(m_huart, data, size, 300);
}