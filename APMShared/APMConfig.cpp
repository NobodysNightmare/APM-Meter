#include "APMConfig.h"

APMConfig::APMConfig(int argc, WCHAR* argv[]) {
	//defaults
	trigger_method		= TRIGGER_BY_HOTKEY;
	trigger_process		= NULL;
	log_file			= NULL;

	if(argc > 1)
		for(int i=1;i<argc;i++) {
			if(wcscmp(argv[i],L"-o") == 0 && (i+1) < argc)
				log_file = argv[i+1];
			if(wcscmp(argv[i],L"-p") == 0 && (i+1) < argc) {
				trigger_method = TRIGGER_BY_PROCESS;
				trigger_process = argv[i+1];
			}
		}
}