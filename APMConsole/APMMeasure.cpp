#include "APMMeasure.h"
#include <stdio.h>

HANDLE	APMMeasure::hSharedMemory = NULL;
LPLONG APMMeasure::lpSharedMemory = NULL;

DWORD	APMMeasure::absolute_starttick = 0;

long	APMMeasure::total_actions = 0;
long		APMMeasure::current_actions_offset = 0;
DWORD	APMMeasure::current_starttick = 0;
APMFrame APMMeasure::ring_buffer[RING_SIZE];
int		APMMeasure::ring_pos = 0;

void APMMeasure::initSharedMemory() {
	hSharedMemory = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, SHARED_MEMORY_SIZE, SHARED_MEMORY_NAME);
	lpSharedMemory = (LPLONG)MapViewOfFile(hSharedMemory, FILE_MAP_WRITE, 0, 0, SHARED_MEMORY_SIZE);
}

long APMMeasure::getTotalActions() {
	total_actions += InterlockedExchange(lpSharedMemory, 0);
	return total_actions;
}

void APMMeasure::setTotalActions(long n) {
	total_actions = n;
	InterlockedExchange(lpSharedMemory, 0);
}

void APMMeasure::resetAllAPM() {
	if(hSharedMemory == NULL)
		APMMeasure::initSharedMemory();
	setTotalActions(0);
	absolute_starttick = GetTickCount();

	current_actions_offset = 0;
	current_starttick = absolute_starttick;
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

APMSnapshot APMMeasure::getSnapshot() {
	APMSnapshot snap;
	snap.time = GetTickCount()-absolute_starttick;
	snap.actions = getTotalActions();
	snap.apm = getCurrentAPM();

	return snap;
}
