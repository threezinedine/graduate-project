#include <StringUtils.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>
#include <stdarg.h>

#include "main.h"

char **SplitString(char *str, uint8_t *argc)
{
    *argc = 0;
    char *token;

    char *copy = strdup(str);
    token = strtok(copy, " ");

    while (token != NULL)
    {
        (*argc)++;
        token = strtok(NULL, " ");
    }

    copy = strdup(str);
    token = strtok(copy, " ");
    char **argv = (char **)malloc((*argc) * sizeof(char *));

    for (int i = 0; i < *argc; ++i)
    {
        argv[i] = strdup(token);
        token = strtok(NULL, " ");
    }

    free(copy);
    return argv;
}

void Free2DArray(int argc, char **argv)
{
    for (uint8_t i = 0; i < argc; i++)
    {
        free(argv[i]);
    }

    free(argv);
}

void FormatString(char *buffer, char *format, ...)
{
    va_list args;
    va_start(args, format);

    vsnprintf(buffer, 256, format, args);

    va_end(args);
}

uint8_t CompareStr(char *chFirst, char *chSecond)
{
    if (strcmp(chFirst, chSecond) == 0)
    {
        return STR_EQUAL;
    }
    else
    {
        return STR_NOT_EQUAL;
    }
}

uint8_t ConvertHexStringToHex(char *hexStr)
{
    return strtoul(hexStr, NULL, 16);
}

void ConvertHexToString(char *chBuffer, uint8_t u8Value)
{
    chBuffer[0] = '0';
    chBuffer[1] = 'x';
    sprintf(&chBuffer[2], "%X", u8Value);
}
