#pragma once
#include "APMConfig.h"
#include "ProcessResolver.h"

#define HOTKEY_STARTSTOP_MEASURE 1

class APMTrigger {
private:
	APMConfig* cfg;
	HANDLE hProcess;

	BOOL triggerStartByHotkey();
	BOOL triggerStartByProcess();
	BOOL triggerStopByHotkey(MSG* msg);
	BOOL triggerStopByProcess();
public:
	APMTrigger(APMConfig* n_cfg);
	~APMTrigger();
	BOOL triggerStart();
	BOOL triggerStop(MSG* msg);
}