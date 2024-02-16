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
