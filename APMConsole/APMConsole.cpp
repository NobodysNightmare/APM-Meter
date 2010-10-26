#include "targetver.h"
#include "APMMeasure.h"
#include "APMLogger.h"
#include <stdio.h>
#include <tchar.h>

#define HOTKEY_STARTSTOP_MEASURE 1

int _tmain(int argc, _TCHAR* argv[]) {
	APMLogger* logger = NULL;
	HINSTANCE hinstAPMSharedDll;
	HOOKPROC hprocKeyboard;
	HOOKPROC hprocMouse;
	HHOOK keyboardHook;
	HHOOK mouseHook;
	UINT_PTR timerId;
	MSG msg = {0};

	hinstAPMSharedDll = LoadLibrary(TEXT("APMSharedDll.dll"));
	hprocKeyboard = (HOOKPROC)GetProcAddress(hinstAPMSharedDll, "?KeyboardProc@APMShared@@SGJHIJ@Z");
	hprocMouse = (HOOKPROC)GetProcAddress(hinstAPMSharedDll, "?MouseProc@APMShared@@SGJHIJ@Z");
	if(hinstAPMSharedDll == NULL || hprocKeyboard == NULL || hprocMouse == NULL) {
		printf("Failed to load APMSharedDll.dll:\r\n");
		if(hinstAPMSharedDll == NULL)
			printf("DLL not found\r\n");
		else
			printf("Couldn't find procedure (incompatible dll)\r\n");
		return GetLastError();
	}

	keyboardHook = SetWindowsHookEx(WH_KEYBOARD, hprocKeyboard, hinstAPMSharedDll, 0);
	mouseHook = SetWindowsHookEx(WH_MOUSE, hprocMouse, hinstAPMSharedDll, 0);
	if(keyboardHook == NULL || mouseHook == NULL) {
		printf("Failed to register hooks\r\n");
		return GetLastError();
	}

	if(!RegisterHotKey(NULL, HOTKEY_STARTSTOP_MEASURE, MOD_ALT | MOD_SHIFT, 0x50 /*P*/)) {
		printf("Failed to register the hotkey\r\n");
		return GetLastError();
	}

	if(argc > 1)
		for(int i=1;i<argc;i++)
			if(wcscmp(argv[i],L"-o") == 0 && (i+1) < argc)
				logger = new APMLogger(argv[i+1]);
	
	
	printf("Waiting for start-signal... ");
	while(true) {
		BOOL bRet = GetMessage(&msg, (HWND)-1, WM_HOTKEY, WM_HOTKEY);
		if(bRet == 0 || bRet == -1)
			return bRet; //program exit
		if(msg.wParam == HOTKEY_STARTSTOP_MEASURE) {
			printf("START\r\n");
			break;		//start measurement
		}
	}

	APMMeasure::resetAllAPM();
	timerId = SetTimer(NULL, 0, MEASURE_CYCLE_LENGTH-10, NULL);
	long max = 0;
	while(true) {
		BOOL bRet = GetMessage(&msg, NULL, 0, 0);
		if(bRet == 0 || bRet == -1)
			break;

		if(msg.message == WM_TIMER) {
			long current = APMMeasure::getCurrentAPM();
			if(current > max)
				max = current;
			printf("Current: %d    Average: %d    Max: %d    \r", current, APMMeasure::getAverageAPM(), max);
			APMMeasure::moveCurrentAPM();

			if(logger != NULL)
				logger->addEntry(APMMeasure::getSnapshot());
		} else if(msg.message == WM_HOTKEY) {
			if(msg.wParam == HOTKEY_STARTSTOP_MEASURE) {
				printf("\r\n");
				break;
			}
		}
	}
	KillTimer(NULL, timerId);
	UnhookWindowsHookEx(keyboardHook);
	UnhookWindowsHookEx(mouseHook);
	UnregisterHotKey(NULL, HOTKEY_STARTSTOP_MEASURE);
	delete logger;
	system("pause");
	return 0;
}
