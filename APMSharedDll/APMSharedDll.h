#define DLL_EXPORT __declspec(dllexport)
#include "targetver.h"
#define WIN32_LEAN_AND_MEAN
#include <windows.h>

#define SHARED_MEMORY_SIZE sizeof(long)
#define SHARED_MEMORY_NAME TEXT("APM-Shared-Memory")

#define KEY_FIRST_PRESS 3221225472

class APMShared {
private:
	static HANDLE hSharedMemory;
	static LPLONG total_actions;

	static void addAction();
public:
	static void initSharedMemory();
	static void freeSharedMemory();
	static DLL_EXPORT LRESULT CALLBACK KeyboardProc(int code, WPARAM wParam, LPARAM lParam);
	static DLL_EXPORT LRESULT CALLBACK MouseProc(int code, WPARAM wParam, LPARAM lParam);
};
