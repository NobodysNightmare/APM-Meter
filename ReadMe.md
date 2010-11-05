APM-Meter
=========
APM-Meter is a tool to measure your actions per minute. This is most probably used for computer games.
This is especially aimed to games that do not offer a method of displaying one's own APM.

Using APMConsole
----------------
After starting APMConsole the program will wait for an event to occur.
By default this event is pressing the hotkey Shift + Alt + P.

You can also define other wait events (see below).

After the event occured measurement starts.
The measurement stops when the counter-event occurs (e.g. pressing the hotkey again).

### Command-line arguments
	-o <filename>
			Log the APM to a file. e.g.: -o logfile.bin
			Can be used to create graphs or get other statistics.
			The output is binary and not human-readable, but you can convert it to a
			human- (and gnuplot-) readable format using convert.py
			Binary format is as follows:
				12 Byte blocks that consist of
					4 Byte long integer: total number of actions
					4 Byte long integer: elapsed time in ms
					4 Byte long integer: current APM as computed by APMConsole

	-p <process name>
			Don't wait for a special hotkey to be pressed, but synchronize measurement to a process.
			The measurement will start when a process with the specified image-name is started.
			The measurement ends when this process terminates.
			Example: -p notepad.exe

	--no-skip-begin
			By default the real apm-measurement starts with the first action after the start-event,
			use this option to  start all measurement immediately.

Using convert.py
----------------
This is a minimal python-script to convert the binary logs created by APMConsole into text-files.
The resulting files can for example be read by gnuplot.
The easiest way using the script is just drag and drop the binary log onto the convert.py script in your explorer.
This is equal to calling the script with the filename of the log as its first parameter.

Note: Either way you will need python installed to run convert.py