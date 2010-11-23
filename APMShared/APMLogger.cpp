#include "APMLogger.h"
#include <stdio.h>

APMLogger::APMLogger(LPCTSTR filename) {
	hFile = CreateFile(filename, GENERIC_WRITE, NULL, NULL, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, NULL);
	if(hFile == INVALID_HANDLE_VALUE)
		printf("\r\nCould not open log-file\r\n");
	else {
		//set basic header info
		APMLogHeader* head = new APMLogHeader();
		head->file_id[0]	= 'A';
		head->file_id[1]	= 'P';
		head->file_id[2]	= 'M';
		head->version		= (char)1;
		head->header_size = sizeof(APMLogHeader);

		//set time-info
		SYSTEMTIME now;
		GetLocalTime(&now);
		head->year = now.wYear;
		head->month = now.wMonth;
		head->day = now.wDay;
		head->hour = now.wHour;
		head->minute = now.wMinute;

		//write the header
		DWORD written = 0;
		//WriteFile(hFile, head, sizeof(APMLogHeader), &written, NULL);
		//TODO write converter and reader first

		delete head;
	}
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
