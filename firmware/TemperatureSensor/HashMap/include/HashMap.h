#ifndef _NTT_C_HASH_MAP_LIB
#define _NTT_C_HASH_MAP_LIB

#include "main.h"

#define HASH_MAP_CONTAIN 0x01
#define HASH_MAP_NOT_CONTAIN 0x00

typedef struct HashMap_Node
{
    const char *chKey;
    void *pData;
    struct HashMap_Node *sNextNode;
} HashMap_Node;

typedef void (*FreeElementCallback)(HashMap_Node *);

typedef struct
{
    HashMap_Node *sHead;
    HashMap_Node *sTail;
    FreeElementCallback fFreeElementCallback;
} HashMap;

HashMap *HashMap_Init();
void HashMap_Set(HashMap *sHashMap, char *chKey, void *pData);
void *HashMap_Get(HashMap *sHashMap, char *chKey);
void HashMap_Release(HashMap *sHashMap);
uint8_t HashMap_Contains(HashMap *sHashMap, char *chKey);
void HashMap_SetFreeElementCallback(HashMap *sHashMap, FreeElementCallback fCallback);

#endif
