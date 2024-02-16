#include <LCD.h>
#define LCD_ON 0x01
#define LCD_OFF 0x00

static void EmptyTransmitFunction(uint8_t *u8Data, uint8_t u8Length);

static TransmitI2CDataFunction m_fTransmitFunction = EmptyTransmitFunction;
static uint8_t m_u8Address = 0x00;
static uint8_t m_u8IsOn = LCD_ON;

void LCD_Init()
{
	if (m_u8IsOn == LCD_OFF) return;

    LCD_SendCommand(0x08);
    LCD_SendCommand(0x0e);
    LCD_SendCommand(0x30);
    LCD_SendCommand(0x38);
    LCD_SendCommand(0x20);
    LCD_SendCommand(0x28);
    LCD_SendCommand(0x01);
}

void LCD_SetSendI2CData(TransmitI2CDataFunction fTransmitFunction)
{
	if (m_u8IsOn == LCD_OFF) return;

    m_fTransmitFunction = fTransmitFunction;
}

void LCD_Release()
{
	if (m_u8IsOn == LCD_OFF) return;
}

void EmptyTransmitFunction(uint8_t *u8Data, uint8_t u8Length)
{
}

void LCD_SetAddress(uint8_t u8Address)
{
	if (m_u8IsOn == LCD_OFF) return;
    m_u8Address = u8Address;
}

uint8_t LCD_GetAddress()
{
	if (m_u8IsOn == LCD_OFF) return 0x00;
    return m_u8Address;
}

void LCD_SendCommand(char chCommand)
{
	if (m_u8IsOn == LCD_OFF) return;

    char data_u, data_l;
    uint8_t data_t[4];
    data_u = (chCommand & 0xf0);
    data_l = ((chCommand << 4) & 0xf0);
    data_t[0] = data_u | 0x0C;
    data_t[1] = data_u | 0x08;
    data_t[2] = data_l | 0x0C;
    data_t[3] = data_l | 0x08;
    m_fTransmitFunction(data_t, 4);
}

void LCD_SendData(char chData)
{
	if (m_u8IsOn == LCD_OFF) return;

    char data_u, data_l;
    uint8_t data_t[4];
    data_u = (chData & 0xf0);
    data_l = ((chData << 4) & 0xf0);
    data_t[0] = data_u | 0x0D;
    data_t[1] = data_u | 0x09;
    data_t[2] = data_l | 0x0D;
    data_t[3] = data_l | 0x09;
    m_fTransmitFunction(data_t, 4);
}

void LCD_SendString(char *chStr)
{
    while (*chStr)
    {
        LCD_SendData(*chStr++);
    }
}

void LCD_SetCursor(uint8_t u8Row, uint8_t u8Col)
{
	if (m_u8IsOn == LCD_OFF) return;

    switch (u8Row)
    {
    case 0:
        u8Col |= 0x80;
        break;
    case 1:
        u8Col |= 0xC0;
        break;
    }

    LCD_SendCommand(u8Col);
}

void LCD_Clear(void)
{
	if (m_u8IsOn == LCD_OFF) return;

    LCD_SendCommand(0x80);
    for (int i = 0; i < 70; i++)
    {
        LCD_SendData(' ');
    }
    LCD_SetCursor(0, 0);
}

void LCD_GlobalOn(void)
{
	m_u8IsOn = LCD_ON;
}

void LCD_GlobalOff(void)
{
	m_u8IsOn = LCD_OFF;
}
