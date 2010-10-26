#include "APMLogger.h"
#include <stdio.h>

APMLogger::APMLogger(LPCTSTR filename) {
	hFile = CreateFile(filename, GENERIC_WRITE, NULL, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if(hFile == INVALID_HANDLE_VALUE)
		printf("");
}

APMLogger::~APMLogger() {
	CloseHandle(hFile);
}

void APMLogger::addEntry(APMSnapshot snap) {
	DWORD written = 0;
	if(!WriteFile(hFile, &snap, sizeof(APMSnapshot), &written, NULL))
		printf("#%d#", GetLastError());
}
