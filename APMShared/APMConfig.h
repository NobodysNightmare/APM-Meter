#pragma once
#include <Windows.h>

#define TRIGGER_BY_HOTKEY 1
#define TRIGGER_BY_PROCESS 2

#define HOTKEY_STARTSTOP_MEASURE 1

typedef class APMConfig {
public:
	APMConfig(int argc, WCHAR* argv[]);

	int		trigger_method;
	WCHAR*	trigger_process;
	WCHAR*	log_file;
} APMConfig;