#include <Queue.h>
#include <stdlib.h>

#include "main.h"
static void EmptyFreeNodeFunction(Queue_Node *sNode);

Queue *Queue_Init()
{
    Queue *sQueue = (Queue *)malloc(sizeof(Queue));
    sQueue->sHead = NULL;
    sQueue->u8Size = 0;
    sQueue->sTail = NULL;
    sQueue->fFreeNode = EmptyFreeNodeFunction;
    return sQueue;
}

void Queue_SetFreeElement(Queue *sQueue, QueueNodeFreeFunction fFreeElementFunction)
{
    sQueue->fFreeNode = fFreeElementFunction;
}

void Queue_Release(Queue *sQueue)
{
    Queue_Node *ptr = sQueue->sHead;

    while (ptr != NULL)
    {
        Queue_Node *ptrTemp = ptr->sNextNode;
        sQueue->fFreeNode(ptr);
        free(ptr);
        ptr = ptrTemp;
    }

    free(sQueue);
}

uint8_t Queue_IsEmpty(Queue *sQueue)
{
    return (sQueue->u8Size == 0) * QUEUE_BOOL_TRUE + (1 - (sQueue->u8Size == 0)) * QUEUE_BOOL_FALSE;
}

void Queue_EnQueue(Queue *sQueue, void *data)
{
    Queue_Node *sNewNode = (Queue_Node *)malloc(sizeof(Queue_Node));
    sNewNode->pData = data;
    sNewNode->sNextNode = NULL;

    if (sQueue->u8Size == 0)
    {
        sQueue->sHead = sNewNode;
        sQueue->sTail = sNewNode;
    }
    else
    {
        sQueue->sTail->sNextNode = sNewNode;
    }

    sQueue->u8Size++;
}

void *Queue_DeQueue(Queue *sQueue)
{
    void *pData = sQueue->sHead->pData;
    Queue_Node *sHead = sQueue->sHead;

    if (sQueue->u8Size == 1)
    {
        sQueue->sHead = NULL;
        sQueue->sTail = NULL;
    }
    else
    {
        sQueue->sHead = sQueue->sHead->sNextNode;
    }

    sQueue->u8Size--;
    free(sHead);
    return pData;
}

void EmptyFreeNodeFunction(Queue_Node *sNode)
{
}
