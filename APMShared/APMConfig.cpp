#include "APMConfig.h"

APMConfig::APMConfig(int argc, WCHAR* argv[]) {
	//defaults
	trigger_method		= TRIGGER_BY_HOTKEY;
	trigger_process		= NULL;
	log_file			= NULL;
	skip_begin			= TRUE;

	if(argc > 1)
		for(int i=1;i<argc;i++) {
			if(wcscmp(argv[i],L"-o") == 0 && (i+1) < argc) {
				log_file = argv[i+1];
			} else if(wcscmp(argv[i],L"-p") == 0 && (i+1) < argc) {
				trigger_method = TRIGGER_BY_PROCESS;
				trigger_process = argv[i+1];
			} else if(wcscmp(argv[i],L"--no-skip-begin") == 0) {
				skip_begin = FALSE;
			} else if(wcscmp(argv[i],L"--help") == 0) {
				showHelp();
				exit(0);
			}
		}
}

void APMConfig::showHelp() {
	//printf("implement me ^^");
}