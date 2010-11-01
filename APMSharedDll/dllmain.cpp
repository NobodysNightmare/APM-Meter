#include "APMSharedDll.h"

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
	switch (ul_reason_for_call)	{
	case DLL_PROCESS_ATTACH:
		APMShared::initSharedMemory();
		break;
	case DLL_PROCESS_DETACH:
		APMShared::freeSharedMemory();
		break;
	}
	return TRUE;
}
