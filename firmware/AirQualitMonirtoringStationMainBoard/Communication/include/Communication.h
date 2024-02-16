#ifndef _NTT_C_UART_LIB
#define _NTT_C_UART_LIB

#ifdef JUST_FOR_TESTING
#include <stdint.h>
#endif

#include <Queue.h>

typedef void (*Communication_TransmitFunction)(uint8_t *, uint8_t);
typedef struct Communication_Instance Communication_Instance;
typedef void (*Communication_FrameHandlerFunction)(Communication_Instance *, uint8_t *, uint8_t);

struct Communication_Instance
{
    Communication_FrameHandlerFunction fHandler;
    Communication_TransmitFunction fTransmitFunction;
    Queue *sQueue;
};

Communication_Instance *Communication_Init();
void Communication_Release(Communication_Instance *sUartInstance);
void Communication_SetTransmitFunction(Communication_Instance *sUartInstance, Communication_TransmitFunction fTransmitFunction);
void Communication_SetHandlerFunction(Communication_Instance *sUartInstance, Communication_FrameHandlerFunction fHanlder);

void Communication_ReceiveData(Communication_Instance *sUartInstance, uint8_t *u8Data, uint8_t u8Length);
void Communication_Update(Communication_Instance *sUartInstance);

#endif