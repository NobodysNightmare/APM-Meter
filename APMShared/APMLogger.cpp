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

void APMLogger::addEntry(APMLoggableSnapshot snap) {
	if(!snap.valid)
		return;
	DWORD written = 0;
	if(!WriteFile(hFile, &(snap.snap), sizeof(APMSnapshot), &written, NULL))
		printf("#%d#\r\n", GetLastError());
}
