#include "ProcessResolver.h"

HANDLE ProcessResolver::getProcessByName(WCHAR* name) {
	HANDLE hSnap = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	PROCESSENTRY32 proc_entry = {0};
	proc_entry.dwSize = sizeof(PROCESSENTRY32);

	if(Process32First(hSnap, &proc_entry)) {
		do {
			if(_wcsicmp(name, proc_entry.szExeFile) == 0)
				return OpenProcess(PROCESS_QUERY_INFORMATION, FALSE, proc_entry.th32ProcessID);
		} while(Process32Next(hSnap, &proc_entry));
	}

	return NULL;
}