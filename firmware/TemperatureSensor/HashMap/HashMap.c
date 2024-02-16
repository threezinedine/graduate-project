#include <stdlib.h>
#include <HashMap.h>
#include <string.h>

// TODO: Remove at the end
#include <stdio.h>

static void DefaultFreeElementCallback(HashMap_Node *sNode)
{
}

HashMap *HashMap_Init()
{
    HashMap *sHashMap = (HashMap *)malloc(sizeof(HashMap));
    sHashMap->sHead = NULL;
    sHashMap->sTail = NULL;
    sHashMap->fFreeElementCallback = DefaultFreeElementCallback;
    return sHashMap;
}

void HashMap_Set(HashMap *sHashMap, char *chKey, void *pData)
{
    HashMap_Node *ptr = sHashMap->sHead;

    while (ptr != NULL)
    {
        if (strcmp(ptr->chKey, chKey) == 0)
        {
            ptr->pData = pData;
            return;
        }
        ptr = ptr->sNextNode;
    }

    HashMap_Node *sNewNode = (HashMap_Node *)malloc(sizeof(HashMap_Node));
    sNewNode->chKey = chKey;
    sNewNode->pData = pData;
    sNewNode->sNextNode = NULL;

    if (sHashMap->sHead == NULL)
    {
        sHashMap->sHead = sNewNode;
        sHashMap->sTail = sNewNode;
    }
    else
    {
        sHashMap->sTail->sNextNode = sNewNode;
        sHashMap->sTail = sNewNode;
    }
}

void *HashMap_Get(HashMap *sHashMap, char *chKey)
{
    HashMap_Node *ptr = sHashMap->sHead;

    while (ptr != NULL)
    {
        if (!strcmp(ptr->chKey, chKey))
        {
            return ptr->pData;
        }
        else
        {
            ptr = ptr->sNextNode;
        }
    }

    return NULL;
}

void HashMap_Release(HashMap *sHashMap)
{
    HashMap_Node *ptr = sHashMap->sHead;

    while (ptr != NULL)
    {
        HashMap_Node *ptrTemp = ptr->sNextNode;
        sHashMap->fFreeElementCallback(ptr);
        free(ptr);
        ptr = ptrTemp;
    }

    free(sHashMap);
}

uint8_t HashMap_Contains(HashMap *sHashMap, char *chKey)
{
    HashMap_Node *ptr = sHashMap->sHead;

    while (ptr != NULL)
    {
        if (!strcmp(ptr->chKey, chKey))
        {
            return HASH_MAP_CONTAIN;
        }

        ptr = ptr->sNextNode;
    }

    return HASH_MAP_NOT_CONTAIN;
}

void HashMap_SetFreeElementCallback(HashMap *sHashMap, FreeElementCallback fCallback)
{
    sHashMap->fFreeElementCallback = fCallback;
}
