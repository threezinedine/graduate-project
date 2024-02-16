#include <Communication.h>
#include <stdlib.h>
#include <string.h>

static void EmptyTransmitFunction(uint8_t *u8Data, uint8_t u8Length);
static void EmptyHanlderFunction(Communication_Instance *sUartInstance, uint8_t *u8Data, uint8_t u8Length);

typedef struct
{
    uint8_t *u8Data;
    uint8_t u8Length;
} UART_Frame;

static void FreeUARTFrame(UART_Frame *sFrame);
static void QueueFreeUARTFrameNode(Queue_Node *sNode);

//static Queue *m_sUARTFrameQueue;

Communication_Instance *Communication_Init()
{
    Communication_Instance *sUartInstance = (Communication_Instance *)malloc(sizeof(Communication_Instance));
    sUartInstance->fTransmitFunction = EmptyTransmitFunction;
    sUartInstance->fHandler = EmptyHanlderFunction;
    sUartInstance->sQueue = Queue_Init();
    Queue_SetFreeElement(sUartInstance->sQueue, QueueFreeUARTFrameNode);

    return sUartInstance;
}

void Communication_Release(Communication_Instance *sUartInstance)
{
    free(sUartInstance);
}

void Communication_SetTransmitFunction(Communication_Instance *sUartInstance, Communication_TransmitFunction fTransmitFunction)
{
    sUartInstance->fTransmitFunction = fTransmitFunction;
}

void Communication_SetHandlerFunction(Communication_Instance *sUartInstance, Communication_FrameHandlerFunction fHanlder)
{
    sUartInstance->fHandler = fHanlder;
}

void Communication_ReceiveData(Communication_Instance *sUartInstance, uint8_t *u8Data, uint8_t u8Length)
{
    UART_Frame *sFrame = (UART_Frame *)malloc(sizeof(UART_Frame));
    sFrame->u8Data = (uint8_t *)malloc(sizeof(uint8_t) * u8Length);
    memcpy(sFrame->u8Data, u8Data, u8Length);
    sFrame->u8Length = u8Length;
    Queue_EnQueue(sUartInstance->sQueue, sFrame);
}

void Communication_Update(Communication_Instance *sUartInstance)
{
    if (!Queue_IsEmpty(sUartInstance->sQueue))
    {
        UART_Frame *sFrame = (UART_Frame *)Queue_DeQueue(sUartInstance->sQueue);

        sUartInstance->fHandler(sUartInstance, sFrame->u8Data, sFrame->u8Length);

        FreeUARTFrame(sFrame);
    }
}

void EmptyTransmitFunction(uint8_t *u8Data, uint8_t u8Length)
{
}

void EmptyHanlderFunction(Communication_Instance *sUartInstance, uint8_t *u8Data, uint8_t u8Length)
{
}

void FreeUARTFrame(UART_Frame *sFrame)
{
    free(sFrame->u8Data);
    free(sFrame);
}

void QueueFreeUARTFrameNode(Queue_Node *sNode)
{
    FreeUARTFrame((UART_Frame *)sNode->pData);
}
