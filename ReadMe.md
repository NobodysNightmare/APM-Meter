APM-Meter
=========
APM-Meter is a tool to measure your actions per minute. This is most probably used for computer games.
This is especially aimed to games that do not offer a method of displaying one's own APM.

Using APMConsole
----------------
After starting APMConsole the program will wait for a global hotkey to be pressed.
This hotkey is predefined as Shift + Alt + P.

After pressing the hotkey measurement starts, if you press it again it stops.

### Command-line arguments
	-o <filename>
			Log the APM to a file. Can be used to create graphs or get other statistics.
			The output is binary and not human-readable, but you can convert it to a
			human- (and gnuplot-) readable format using convert.py
			Binary format is as follows:
				12 Byte blocks that consist of
					4 Byte long integer: total number of actions
					4 Byte long integer: elapsed time in ms
					4 Byte long integer: current APM as computed by APMConsole

Using convert.py
----------------
This is a minimal python-script to convert the binary logs created by APMConsole into text-files.
The resulting files can for example be read by gnuplot.
The easiest way using the script is just drag and drop the binary log onto the convert.py script in your explorer.
This is equal to calling the script with the filename of the log as its first parameter.

Note: Either way you will need python installed to run convert.py