#include "LinkList.h"
#include "stdlib.h"

#include "main.h"

static void EmptyFreeElement(LinkList_Node *sNode);

LinkList *LinkList_Init()
{
	LinkList *sList = (LinkList *)malloc(sizeof(LinkList));

    sList->sHead = NULL;
    sList->sTail = NULL;
    sList->u8Size = 0;
    sList->fFreeElement = EmptyFreeElement;
    return sList;
}

void LinkList_Release(LinkList *sList)
{
    LinkList_Node *ptr = sList->sHead;

    while (ptr != NULL)
    {
        LinkList_Node *temp = ptr->sNextNode;
        sList->fFreeElement(ptr);
        free(ptr);
        ptr = temp;
    }

    free(sList);
}

uint8_t LinkList_GetSize(LinkList *sList)
{
    return sList->u8Size;
}

void LinkList_Append(LinkList *sList, void *pData)
{
    LinkList_Node *sNode = (LinkList_Node *)malloc(sizeof(LinkList_Node));
    sNode->pData = pData;
    sNode->sNextNode = NULL;

    if (sList->u8Size == 0)
    {
        sList->sHead = sNode;
        sList->sTail = sNode;
    }
    else
    {
        sList->sTail->sNextNode = sNode;
        sList->sTail = sNode;
    }

    sList->u8Size++;
}

void *LinkList_Get(LinkList *sList, uint8_t u8Index)
{
    LinkList_Node *ptrNode = sList->sHead;

    while (u8Index != 0 && ptrNode != NULL)
    {
        ptrNode = ptrNode->sNextNode;
        u8Index--;
    }

    if (u8Index != 0)
    {
        return NULL;
    }
    else
    {
        return ptrNode->pData;
    }
}

void LinkList_SetFreeElement(LinkList *sList, FreeElementFunction fCallback)
{
    sList->fFreeElement = fCallback;
}

void LinkList_Delete(LinkList *sList, uint8_t u8Index)
{
    LinkList_Node *ptr = sList->sHead;

    if (u8Index == 0)
    {
        if (sList->u8Size == 0)
        {
            sList->sTail = NULL;
        }
        sList->sHead = ptr->sNextNode;
        sList->u8Size--;
        sList->fFreeElement(ptr);
        free(ptr);

        return;
    }

    while (u8Index > 1)
    {
        ptr = ptr->sNextNode;
        u8Index--;
    }

    LinkList_Node *sDeleted = ptr->sNextNode;

    ptr->sNextNode = sDeleted->sNextNode;

    sList->u8Size--;
    sList->fFreeElement(sDeleted);
    free(sDeleted);
}

void EmptyFreeElement(LinkList_Node *sNode)
{
}
