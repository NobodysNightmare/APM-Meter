#include "targetver.h"
#include "APMMeasure.h"
#include "APMLogger.h"
#include "ProcessResolver.h"
#include "APMConfig.h"
#include "APMTrigger.h"
#include <stdio.h>
#include <tchar.h>

int _tmain(int argc, _TCHAR* argv[]) {
	APMLogger* logger = NULL;
	HINSTANCE hinstAPMSharedDll;
	HOOKPROC hprocKeyboard;
	HOOKPROC hprocMouse;
	HHOOK keyboardHook;
	HHOOK mouseHook;
	UINT_PTR timerId;

	APMConfig* cfg = new APMConfig(argc, argv);
	APMTrigger* trigger = new APMTrigger(cfg);

	hinstAPMSharedDll = LoadLibrary(TEXT("APMKeyHook.dll"));
	hprocKeyboard = (HOOKPROC)GetProcAddress(hinstAPMSharedDll, "?KeyboardProc@APMKeyHook@@SGJHIJ@Z");
	hprocMouse = (HOOKPROC)GetProcAddress(hinstAPMSharedDll, "?MouseProc@APMKeyHook@@SGJHIJ@Z");
	if(hinstAPMSharedDll == NULL || hprocKeyboard == NULL || hprocMouse == NULL) {
		printf("Failed to load APMKeyHook.dll:\r\n");
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

	// TODO: renew logger-constructor and remove this
	if(argc > 1)
		for(int i=1;i<argc;i++) {
			if(wcscmp(argv[i],L"-o") == 0 && (i+1) < argc)
				logger = new APMLogger(argv[i+1]);
		}
	
	
	
	printf("Waiting to start... ");
	trigger->triggerStart();
	printf("\rStarted measure!       \r\n");

	APMMeasure::resetAllAPM();
	timerId = SetTimer(NULL, 0, MEASURE_CYCLE_LENGTH-10, NULL);
	long max = 0;
	MSG msg = {0};
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
		}
		
		if(trigger->triggerStop(&msg)) {
			printf("\r\n");
			break;
		}
		DispatchMessage(&msg);
	}
	KillTimer(NULL, timerId);
	UnhookWindowsHookEx(keyboardHook);
	UnhookWindowsHookEx(mouseHook);
	delete logger;
	delete trigger;
	delete cfg;
	system("pause");
	return 0;
}
