#include <Command.h>
#include <stdlib.h>
#include <string.h>
#include <StringUtils.h>
#include <HashMap.h>
#include <Queue.h>
#include "usbd_cdc_if.h"

typedef struct
{
    char *chCommand;
} Command;

typedef struct
{
    // char *chName;
    CommandHanlderFunction fCallback;
    char *chHelp;
} Command_Def;

static HashMap *m_sCommandDefsMap;
static Queue *m_sCommandQueue;

static void EmptyCommandHanlder(uint8_t argc, char **argv);
static void FreeCommand(Command *sCommand);
static CommandHanlderFunction m_fDefaultCallback = (CommandHanlderFunction)EmptyCommandHanlder;
static void HelpCommandHanlderFunction(uint8_t argc, char **argv);
static void FreeCommandDefElement(HashMap_Node *sNode);
static void FreeCommandQueueElement(Queue_Node *sNode);

void Command_Init()
{
    m_sCommandDefsMap = HashMap_Init();
    m_sCommandQueue = Queue_Init();
    Queue_SetFreeElement(m_sCommandQueue, FreeCommandQueueElement);
    HashMap_SetFreeElementCallback(m_sCommandDefsMap, FreeCommandDefElement);
    Command_Register((char *)"help", HelpCommandHanlderFunction, "Manual use for all commands");
}

void Command_Release()
{
    HashMap_Release(m_sCommandDefsMap);
    Queue_Release(m_sCommandQueue);
}

void Command_Register(char *chCommandKey, CommandHanlderFunction fCallback, const char *chCommandHelp)
{
    Command_Def *sCommandDef = (Command_Def *)malloc(sizeof(Command_Def));
    sCommandDef->chHelp = strdup(chCommandHelp);
    sCommandDef->fCallback = fCallback;
    HashMap_Set(m_sCommandDefsMap, chCommandKey, (void *)sCommandDef);
}

void Command_ReceiveData(char *chCommand, uint8_t u8Length)
{
    Command *sCommand = (Command *)malloc(sizeof(Command));
    sCommand->chCommand = (char *)malloc(u8Length * sizeof(char));
    memcpy(sCommand->chCommand, chCommand, u8Length + 1);

    Queue_EnQueue(m_sCommandQueue, (void *)sCommand);
}

void Command_RegisterDefaultCallback(CommandHanlderFunction fCallback)
{
    m_fDefaultCallback = fCallback;
}

void EmptyCommandHanlder(uint8_t argc, char **argv)
{
}

void Command_Update()
{
    if (!Queue_IsEmpty(m_sCommandQueue))
    {
        Command *sComand = (Command *)Queue_DeQueue(m_sCommandQueue);
        uint8_t argc = 0;

        char **argv = SplitString(sComand->chCommand, &argc);
        if (HashMap_Contains(m_sCommandDefsMap, argv[0]) == HASH_MAP_CONTAIN)
        {
            Command_Def *sNode = (Command_Def *)HashMap_Get(m_sCommandDefsMap, argv[0]);
            sNode->fCallback(argc - 1, &argv[1]);
        }
        else
        {
            m_fDefaultCallback(argc, argv);
        }

        FreeCommand(sComand);
        Free2DArray(argc, argv);
    }
}

void FreeCommand(Command *sCommand)
{
    free(sCommand->chCommand);
    free(sCommand);
}

#ifndef JUST_FOR_TESTING
void Command_Response(char *chResponse, uint8_t u8Length)
{
    // Override this to send the response to the user
	CDC_Transmit_FS((uint8_t *)chResponse, u8Length);
}
#endif

void Command_ResponseConstChar(const char *chResponse)
{
	CDC_Transmit_FS((uint8_t *)chResponse, strlen(chResponse));
}

void HelpCommandHanlderFunction(uint8_t argc, char **argv)
{
    if (HashMap_Contains(m_sCommandDefsMap, argv[0]) == HASH_MAP_CONTAIN)
    {
        Command_Def *cmd = (Command_Def *)HashMap_Get(m_sCommandDefsMap, argv[0]);
        Command_Response(cmd->chHelp, strlen(cmd->chHelp));
    }
    else
    {
        char chResponse[] = "Command does not exist";
        Command_Response(chResponse, strlen(chResponse));
    }
}

static void FreeCommandDefElement(HashMap_Node *sNode)
{
    free((Command_Def *)(sNode->pData));
}

static void FreeCommandQueueElement(Queue_Node *sNode)
{
    FreeCommand((Command *)sNode->pData);
}
