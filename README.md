BefungeSharp - The Premier Funge-98 IDE for Windows
===================================================
BefungeSharp is a Funge-98 IDE that aims to have the best Befunge code editor, a fully compliant interpreter, and useful debugging features like edit and continue, all while being very easy to start using (especially on Windows!)

Why does this need to exist?
============================
While discovering Funge-98 I noticed that most tools that ran on windows only were for Befunge-93, were difficult to compile, and didn't show the IP moving around FungeSpace (arguably the most intreguing part of Funge!) There were some "Try it on the web" interpreters, but they were hard to use and stashed away on old archived webpages, sometimes barely working in modern browsers. Hardly a good solution to sharing this wonderful language.

Where is the project at right now?
==================================
After quiet a lot of effort most of the spec is complete and the editor is very well polished.

Completed Features
------------------
* Editor
	- Type in any cardinal direction!
	- Cut, Copy, and Paste with Windows integration
	- Sidebar with full ASCII reference
	- CMD like file opening and saving, featuring DIR and CD-like commands
	- Syntax Highlighting
	- Edit and Pane around ALL of FungeSpace (yes, even the negative parts)
* Interpreter
	- i,o,t,=,y,{, and } implemented
	- Switch between step, slow -  fast speed, and output only view
	- Vizualizes FungeSpace
	- Follows main IP around ALL of FungeSpace
* Debugging
	- Stack, input, and output information displayed
* Other features
	- Very colorful menus
	- Only requires .NET Framework and the Windows console
	
Planned features
---------------------------------
* Editor
	- Insert mode
	- "Reverse in place" mode
	- Undo and Redo
	- Insert Snippet
	- "Tool chain" to open current source file in other interpreters
* Interpreter
	- Schematics
	- Trefunge
	- Better preformance
	- Logging
	- Befunge- 93 compatability mode
	- Sandbox mode
	- y
* Debugging
	- Edit and Continue
	- Command line debugging
* Help
	- An actual help screen and subscreens showing
		* Implentation details
		* Nuaces of various instructions
		* Full language reference
		* How to reach the author
		* A full list of keyboard short cuts and program help
		* 'y' read out
* Other features
	- Using MonoGame for graphics and input instead of twisting the console's intended useage
	- An awesome icon for the program

Requirements
-------------
.NET Framework 4.5 Runtime Libraries
Visual Studio 2012 for compiling

Please enjoy, and if you'd really like to be a pal send the author a bug report or a comment about it!
