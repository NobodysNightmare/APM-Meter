#include "APMKeyHook.h"

HANDLE	APMKeyHook::hSharedMemory = NULL;
LPLONG	APMKeyHook::total_actions = NULL;

void APMKeyHook::initSharedMemory() {
	hSharedMemory = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHARED_MEMORY_SIZE, SHARED_MEMORY_NAME);
	total_actions = (LPLONG)MapViewOfFile(hSharedMemory, FILE_MAP_WRITE, 0, 0, SHARED_MEMORY_SIZE);
}

void APMKeyHook::freeSharedMemory() {
	CloseHandle(hSharedMemory);
}

void APMKeyHook::addAction() {
	InterlockedIncrement(total_actions);
}

LRESULT CALLBACK APMKeyHook::KeyboardProc(int code, WPARAM wParam, LPARAM lParam) {
	if(code>=0) {
		if((lParam & KEY_FIRST_PRESS) == 0) {
			if(!(wParam == VK_TAB || wParam == VK_SHIFT || wParam == VK_CONTROL ||
				wParam == VK_MENU || wParam == VK_LWIN || wParam == VK_RWIN ||
				wParam == VK_CAPITAL || wParam == VK_RETURN))
				addAction();
		}
	}
	return CallNextHookEx(NULL, code, wParam, lParam);
}

LRESULT CALLBACK APMKeyHook::MouseProc(int code, WPARAM wParam, LPARAM lParam) {
	if(code>=0) {
		if(wParam == WM_LBUTTONUP || wParam == WM_RBUTTONUP || wParam == WM_XBUTTONUP)
			addAction();
	}
	return CallNextHookEx(NULL, code, wParam, lParam);
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call)	{
	case DLL_PROCESS_ATTACH:
		APMKeyHook::initSharedMemory();
		break;
	case DLL_PROCESS_DETACH:
		APMKeyHook::freeSharedMemory();
		break;
	}
	return TRUE;
}
