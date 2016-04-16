BefungeSharp - The Premier Funge-98 IDE for Windows
===================================================
BefungeSharp is a Funge-98 IDE that aims to have the best Befunge code editor, a fully compliant interpreter, and useful debugging features like edit and continue, all while being very easy to use!

Download the latest stable release [here.](http://www.tedngreene.com/projects/befunge/befungesharp_latest.zip) Be sure to read the "4 Minute Intro" if you are unfamiliar with Befunge.

![The interpreter running itoroman.bf](./BefungeSharp/doc/screenshots/editor_window_1.png "The interpreter running itoroman.bf")
  
<br/>

Why does this need to exist?
============================
This project fills the void for Windows users who want to quickly get started with Funge-98, not just Befunge-93, without complex compilation, and, most of all, want to see the IP dance around FungeSpace. In addition, the Funge specific editor makes writing Funge code much more enjoyable than using Notepad. No other Funge tool combines these features as of time.

Where is the project at right now?
==================================

Completed Features
------------------
* Editor
	- Type in any cardinal direction!
	- Edit and view ALL of FungeSpace (yes, even the negative parts!)
	- Windows clipboard integration
	- Insert code snippets
	- Auto save feature
	- Sidebar with ASCII and keyboard shortcuts reference
	- CMD-like interface for editor file I/O
* Interpreter
	- Visualizes FungeSpace and all active IPs!
	- FungeSpace is Funge-98 compliant
	- Implemented i, o, t, =, y, {, and } instructions
	- Switch among step, slow - fast speeds, and output only view
	- Sandbox mode for i, o, and = instructions
* Debugging
	- Stack, input, and output information displayed
	- Shows FungeSpace change in real time
* Other features
	- Works out of the box, no make files or downloading dependencies!
	- Customizable syntax highlighting throughout program
	- Built in options editor
	- Very colorful menus
	- Entirely open source
	- Does not alter cmd.exe settings
	
Planned features
----------------
* Editor
	- Undo and redo program states
	- Insert mode
	- "Tool chain" feature to open current source file in other interpreters
	- "Reverse selection in place" feature
	- "Rotate selection" feature
* Interpreter
	- Schematics system
	- Trefunge support
	- Better performance
	- Logging output to files
	- Befunge-93 compatibility mode
	- 'y' instruction fully compliant
* Debugging
	- Edit and Continue mode
	- Command line debugging
* Help
	- A help screen and sub-screens showing
		* A full list of keyboard short cuts and program help
		* Implementation notes
		* Full language reference and tutorials
		* Partial 'y' read out
* Other features
	- Add use of command line arguments
	- Using MonoGame for graphics and input instead of (cleverly) twisting the console's intended usage
	- A properly chosen open source license
	- An even more awesome icon for the program

System Requirements
-------------------
.NET Framewrk 4.5 Runtime Libraries, Visual Studio 2012 or greater for compiling.

Please enjoy, and if you'd really like to be a pal send the author a bug report or a comment about it!
