#ifndef _NTT_C_STRING_UTILS_LIB
#define _NTT_C_STRING_UTILS_LIB
#include "main.h"

#define STR_EQUAL 0x01
#define STR_NOT_EQUAL 0x00

char **SplitString(char *str, uint8_t *argc);
void Free2DArray(int argc, char **argv);

void FormatString(char *buffer, char *format, ...);
uint8_t CompareStr(char *chFirst, char *chSecond);

uint8_t ConvertHexStringToHex(char *hexStr);
void ConvertHexToString(char *chBuffer, uint8_t u8Value);

#endif
