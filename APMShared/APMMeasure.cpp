#include "APMMeasure.h"
#include <stdio.h>

APMMeasure::APMMeasure(APMConfig* n_cfg) {
	reset_pending = n_cfg->skip_begin;

	hSharedMemory = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHARED_MEMORY_SIZE, SHARED_MEMORY_NAME);
	lpSharedMemory = (LPLONG)MapViewOfFile(hSharedMemory, FILE_MAP_WRITE, 0, 0, SHARED_MEMORY_SIZE);

	resetAllAPM();
}

APMMeasure::~APMMeasure() {
	CloseHandle(hSharedMemory);
}

long APMMeasure::getTotalActions() {
	total_actions += InterlockedExchange(lpSharedMemory, 0);
	if(reset_pending && total_actions > 0) {
		current_starttick = absolute_starttick = GetTickCount();
		reset_pending = FALSE;
	}
	return total_actions;
}

void APMMeasure::setTotalActions(long n) {
	total_actions = n;
	InterlockedExchange(lpSharedMemory, 0);
}

void APMMeasure::resetAllAPM() {
	setTotalActions(0);
	absolute_starttick = GetTickCount();

	current_actions_offset = 0;
	current_starttick = absolute_starttick;
	ring_pos = 0;
	for(int i=0;i<RING_SIZE;i++) {
		ring_buffer[i].actions = 0;
		ring_buffer[i].time = MEASURE_CYCLE_LENGTH;
	}
}

void APMMeasure::moveCurrentAPM() {
	ring_pos++;
	if(ring_pos >= RING_SIZE)
		ring_pos = 0;

	long actions = getTotalActions();
	DWORD now = GetTickCount();
	ring_buffer[ring_pos].actions = actions-current_actions_offset;
	ring_buffer[ring_pos].time = now-current_starttick;

	current_actions_offset = actions;
	current_starttick = now;
}

long APMMeasure::computeAPM(long actions, DWORD raw_span) {
	double span = (double)(raw_span)/1000.0;
	return (long)(((double)actions/span)*60);
}

long APMMeasure::getCurrentAPM() {
	long action_sum = 0;
	DWORD duration_sum = 0;
	for(int i=0;i<RING_SIZE;i++) {
		action_sum += ring_buffer[i].actions;
		duration_sum += ring_buffer[i].time;
	}
	return computeAPM(action_sum, duration_sum);
}

long APMMeasure::getAverageAPM() {
	return computeAPM(getTotalActions(), GetTickCount()-absolute_starttick);
}

APMLoggableSnapshot APMMeasure::getSnapshot() {
	APMLoggableSnapshot snap;
	snap.valid = !reset_pending;

	snap.snap.time = GetTickCount()-absolute_starttick;
	snap.snap.actions = getTotalActions();
	snap.snap.apm = getCurrentAPM();

	return snap;
}
