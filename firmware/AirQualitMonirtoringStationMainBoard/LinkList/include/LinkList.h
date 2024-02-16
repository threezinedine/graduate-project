#ifndef _NTT_LINK_LIST
#define _NTT_LINK_LIST

#include "main.h"

typedef struct LinkList_Node
{
    void *pData;
    struct LinkList_Node *sNextNode;
} LinkList_Node;

typedef void (*FreeElementFunction)(LinkList_Node *);

typedef struct
{
    LinkList_Node *sHead;
    LinkList_Node *sTail;
    uint8_t u8Size;
    FreeElementFunction fFreeElement;
} LinkList;

LinkList *LinkList_Init();
void LinkList_Release(LinkList *sList);

uint8_t LinkList_GetSize(LinkList *sList);
void LinkList_Append(LinkList *sList, void *pData);
void *LinkList_Get(LinkList *sList, uint8_t u8Index);
void LinkList_Delete(LinkList *sList, uint8_t u8Index);
void LinkList_SetFreeElement(LinkList *sList, FreeElementFunction fCallback);

#endif
