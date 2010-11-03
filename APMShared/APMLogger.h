#pragma once

#include <windows.h>
#include "APMMeasure.h"

class APMLogger {
private:
	HANDLE hFile;
public:
	APMLogger(LPCTSTR filename);
	~APMLogger();

	void addEntry(APMSnapshot snap);
};
