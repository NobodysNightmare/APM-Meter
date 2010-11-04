#include "APMTrigger.h"

APMTrigger::APMTrigger(APMConfig* n_cfg) {
	cfg = n_cfg;
	hProcess = NULL;

	switch(cfg->trigger_method) {
	case TRIGGER_BY_HOTKEY:
		if(!RegisterHotKey(NULL, HOTKEY_STARTSTOP_MEASURE, MOD_ALT | MOD_SHIFT, 0x50)) {
			exit(GetLastError());
		}
		break;
	case TRIGGER_BY_PROCESS:
		break;
	}
}

APMTrigger::~APMTrigger() {
	switch(cfg->trigger_method) {
	case TRIGGER_BY_HOTKEY:
		UnregisterHotKey(NULL, HOTKEY_STARTSTOP_MEASURE);
		break;
	case TRIGGER_BY_PROCESS:
		CloseHandle(hProcess);
		break;
	}
}

BOOL APMTrigger::triggerStart() {
	switch(cfg->trigger_method) {
	case TRIGGER_BY_HOTKEY:
		return triggerStartByHotkey();
	case TRIGGER_BY_PROCESS:
		return triggerStartByProcess();
	default:
		return TRUE;
	}
}

BOOL APMTrigger::triggerStartByHotkey() {
	MSG msg = {0};
	while(true) {
		BOOL bRet = GetMessage(&msg, NULL, WM_HOTKEY, WM_HOTKEY);
		if(bRet == 0 || bRet == -1)
			exit(bRet);
		if(msg.wParam == HOTKEY_STARTSTOP_MEASURE) {
			return TRUE;
		}
	}
}

BOOL APMTrigger::triggerStartByProcess() {
	while(!(hProcess = getProcessByName(cfg->trigger_process)))
		Sleep(50);
	return TRUE;
}

BOOL APMTrigger::triggerStop(MSG* msg) {
	switch(cfg->trigger_method) {
	case TRIGGER_BY_HOTKEY:
		return triggerStopByHotkey(msg);
	case TRIGGER_BY_PROCESS:
		return triggerStopByProcess();
	default:
		return TRUE;
	}
}

BOOL APMTrigger::triggerStopByHotkey(MSG* msg) {
	if(msg->message == WM_HOTKEY && msg->wParam == HOTKEY_STARTSTOP_MEASURE)
		return TRUE;
	return FALSE;
}

BOOL APMTrigger::triggerStopByProcess() {
	if(WaitForSingleObject(hProcess, 0) == WAIT_OBJECT_0)
		return TRUE;
	return FALSE;
}

HANDLE APMTrigger::getProcessByName(const WCHAR* name) {
	HANDLE hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	PROCESSENTRY32 proc_entry = {0};
	proc_entry.dwSize = sizeof(PROCESSENTRY32);

	if(Process32First(hSnap, &proc_entry)) {
		do {
			if(_wcsicmp(name, proc_entry.szExeFile) == 0) {
				CloseHandle(hSnap);
				return OpenProcess(PROCESS_QUERY_INFORMATION | SYNCHRONIZE, FALSE, proc_entry.th32ProcessID);
			}
		} while(Process32Next(hSnap, &proc_entry));
	}

	CloseHandle(hSnap);
	return NULL;
}