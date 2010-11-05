#pragma once

#include <windows.h>
#include "APMKeyHook.h"
#include "APMConfig.h"

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
	HANDLE hSharedMemory;
	LPLONG lpSharedMemory;

	DWORD absolute_starttick;

	long total_actions;
	long current_actions_offset;
	DWORD current_starttick;
	APMFrame ring_buffer[RING_SIZE];
	int ring_pos;

	BOOL reset_pending;

	static long computeAPM(long actions, DWORD starttick);
	void resetAllAPM();
	long getTotalActions();
	void setTotalActions(long n);
public:
	APMMeasure(APMConfig* n_cfg);
	~APMMeasure();
	void moveCurrentAPM();
	long getAverageAPM();
	long getCurrentAPM();
	APMSnapshot getSnapshot();
};