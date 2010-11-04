#pragma once
#include <Windows.h>

#define TRIGGER_BY_HOTKEY 1
#define TRIGGER_BY_PROCESS 2

typedef struct {
	int trigger_method;
	WCHAR* trigger_process;
} APMConfig