BefungeSharp - The Premier Funge-98 IDE for Windows
===================================================
BefungeSharp is a Funge-98 IDE that aims to have the best Befunge code editor, a fully compliant interpreter, and useful debugging features like edit and continue, all while being very easy to use!

![The interpreter running itoroman.bf](./BefungeSharp/doc/screenshots/editor_window_1.png "The interpreter running itoroman.bf")
  
<br/>
Download
--------
Download the latest stable release [here!](http://www.tedngreene.com/BefungeSharp/befungesharp_latest.zip)

Why does this need to exist?
============================
While discovering Funge-98 I noticed that most interpreters weren't written for Windows, had no pre-built binaries, and, worst of all, didn't show the IP moving around FungeSpace! In addition, writing Funge code with a normal text editor is very hard, given nature of 2D code. I felt a specialized editor would benefit the community greatly.

This project fills the void for Windows users who want to quickly get started with Funge-98 (not just Befunge-93), without installing or compiling anything, and most of all want to see the IP dance around FungeSpace. In addition, a Funge-98 specific editor makes creating Funge code much more enjoyable than using Notepad. No other Funge tool combines these features as of writing.

Where is the project at right now?
==================================

Completed Features
------------------
* Editor
	- Type in any cardinal direction!
	- Cut, copy, and paste with Windows integration
	- Syntax highlighting
	- Sidebar with full ASCII reference
	- CMD like file handling, featuring DIR and CD-like commands
	- Edit and view ALL of FungeSpace (yes, even the negative parts!)
* Interpreter
	- Visualizes FungeSpace, all active IPs
	- All Befunge-93 instructions implemented
	- Follows main IP around ALL of FungeSpace
	- FungeSpace is properly sized
	- i, o, t, =, y, {, and } have been implemented
	- Switch between step, slow - fast speed, and output only view
* Debugging
	- Stack, input, and output information displayed
	- Shows FungeSpace change in realtime
* Other features
	- Automatically creates backup versions of a program
	- Only requires .NET Framework and the Windows console
	- Very colorful menus
	- Does not alter cmd.exe settings
	
Planned features
----------------
* Editor
	- Insert mode
	- "Reverse selection in place" feature
	- "Rotate selection" feature
	- Undo and redo
	- Insert code snippets
	- "Tool chain" feature to open current source file in other interpreters
* Interpreter
	- Schematics system
	- Trefunge support
	- Better performance
	- Logging output to files
	- Befunge-93 compatibility mode
	- Sandbox mode
	- 'y' instruction fully compliant
* Debugging
	- Edit and Continue
	- Command line debugging
* Help
	- A help screen and sub-screens showing
		* Implementation details
		* Nuances of various instructions
		* Full language reference
		* A full list of keyboard short cuts and program help
		* 'y' read out
* Other features
	- Add usuage of command line arguments
	- A properly chosen open source license
	- Using MonoGame for graphics and input instead of (cleverly) twisting the console's intended usage
	- An even more awesome icon for the program

Requirements
-------------
.NET Framework 4.5 Runtime Libraries
Visual Studio 2012 for compiling

Please enjoy, and if you'd really like to be a pal send the author a bug report or a comment about it!