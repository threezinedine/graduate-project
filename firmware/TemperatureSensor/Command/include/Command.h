#ifndef _NTT_C_COMMAND_LIB
#define _NTT_C_COMMAND_LIB
#include "main.h"

typedef void (*CommandHanlderFunction)(uint8_t, char **);

void Command_Init();
void Command_Release();
void Command_Register(char *chCommandKey, CommandHanlderFunction fCallback, const char *chCommandHelp);
void Command_ReceiveData(char *chCommand, uint8_t u8Length);
void Command_RegisterDefaultCallback(CommandHanlderFunction fCallback);
void Command_Update();
void Command_Response(char *chResponse, uint8_t u8Length);
void Command_ResponseConstChar(const char *chResponse);

#endif
