#ifndef _NTT_C_LCD_LIB
#define _NTT_C_LCD_LIB

#include "main.h"

typedef void (*TransmitI2CDataFunction)(uint8_t *, uint8_t);

void LCD_SetSendI2CData(TransmitI2CDataFunction fTransmitFunction);
void LCD_Init();
void LCD_Release();
void LCD_SetAddress(uint8_t u8Address);
uint8_t LCD_GetAddress();

void LCD_SendCommand(char chCommand);
void LCD_SendData(char chData);
void LCD_SendString(char *chStr);
void LCD_SetCursor(uint8_t u8Row, uint8_t u8Col);
void LCD_Clear(void);

void LCD_GlobalOn(void);
void LCD_GlobalOff(void);

#endif