#pragma once

#include <windows.h>
#include "APMKeyHook.h"

#define RING_SIZE 15
#define MEASURE_CYCLE_LENGTH 500

typedef struct {
	long	actions;
	DWORD	time;
} APMFrame;

typedef struct {
	long actions;
	DWORD time;
	long apm;
} APMSnapshot;

class APMMeasure {
private:
	static HANDLE hSharedMemory;
	static LPLONG lpSharedMemory;

	static DWORD absolute_starttick;

	static long total_actions;
	static long current_actions_offset;
	static DWORD current_starttick;
	static APMFrame ring_buffer[RING_SIZE];
	static int ring_pos;

	static long computeAPM(long actions, DWORD starttick);
	static void initSharedMemory();
	static long getTotalActions();
	static void setTotalActions(long n);
public:
	static void resetAllAPM();
	static void moveCurrentAPM();
	static long getAverageAPM();
	static long getCurrentAPM();
	static APMSnapshot getSnapshot();
};