#ifndef _NTT_C_QUEUE_LIB
#define _NTT_C_QUEUE_LIB
#include "main.h"

#define QUEUE_BOOL_FALSE 0
#define QUEUE_BOOL_TRUE 1

typedef struct Queue_Node
{
    void *pData;
    struct Queue_Node *sNextNode;
} Queue_Node;

typedef void (*QueueNodeFreeFunction)(Queue_Node *);

typedef struct
{
    uint8_t u8Size;
    Queue_Node *sHead;
    Queue_Node *sTail;
    QueueNodeFreeFunction fFreeNode;
} Queue;

Queue *Queue_Init();
void Queue_SetFreeElement(Queue *sQueue, QueueNodeFreeFunction fFreeElementFunction);
void Queue_Release(Queue *sQueue);
uint8_t Queue_IsEmpty(Queue *sQueue);
void Queue_EnQueue(Queue *sQueue, void *data);
void *Queue_DeQueue(Queue *sQueue);

#endif
