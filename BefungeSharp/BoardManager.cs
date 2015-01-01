using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp
{
    public class BoardManager
    {
        /// <summary>
        /// Represents a 2 dimensional space of characters, non jagged
        /// Accessed with boardArray[row+i][column+j]
        /// TODO - Funge-98 spec says sizeof(T) in stack cell = sizeof(T) in Fungespace Cell
        /// TODO - Fugnge-98 spec says you can have an addressable space of Int32.Min to Int32.Max or more,
        /// we need a large funge space matrix and a mask of the subsection to show users
        /// TODO - Trefunge will require a list of boardArrays
        /// </summary>
        private List<List<char>> _boardArray;
        public List<List<char>> BoardArray { get { return _boardArray; } }

        private BoardUI _bUI;
        public BoardUI BUI { get { return _bUI; } }

        private BoardSideBar _bSideBar;
        private BoardInterpreter _bInterp;

        /// <summary>
        /// Creates a new BoardManager with the options to set up its entire intial state and run type
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="columns">Number of columns</param>
        /// <param name="initChars">
        /// Each element in the array represents a row of text.
        /// If you wish a blank board pass in an empty array
        /// </param>
        /// <param name="initGlobalStack">Initialize the input stack with preset numbers</param>
        /// <param name="mode">Chooses what mode you would like to start the board in</param>
        public BoardManager(int rows, int columns, List<string> initChars = null,
                            Stack<int> initGlobalStack = null, BoardMode mode = BoardMode.Edit)
        {
            //Intialize the board array to be the size of the board
            _boardArray = new List<List<char>>(rows);

            //Fill up the whole rectangle with spaces
            for (int y = 0; y < rows; y++)
            {
                _boardArray.Add(new List<char>());
                for (int x = 0; x < columns; x++)
                {
                    _boardArray[y].Add(' ');
                }
            }

           
            //TODO: Array size checking to make sure it will not be out of bounds?
            if (initChars != null)
            {
                //Fill board it initial strings, if initChars is null this will skip
                //For the number of rows
                for (int y = 0; y < initChars.Count; y++)
                {
                    //Get the current line
                    string currentLine = initChars[y];

                    //For all the characters in the line
                    for (int x = 0; x < currentLine.Length; x++)
                    {
                        //Insert them into the array
                        PutCharacter(y, x, currentLine[x]);
                    }
                }
            }
            
            
            _bInterp = new BoardInterpreter(this,initGlobalStack,mode);
           
            _bUI = new BoardUI(this, _bInterp);
            _bSideBar = new BoardSideBar(this, _bInterp);
          
            Console.CursorVisible = false;
         
            //Draw the field and ui and reset the position
            _bUI.ClearArea(_bInterp.CurMode);
            _bUI.Draw(_bInterp.CurMode);

            _bSideBar.ClearArea(_bInterp.CurMode);
            _bSideBar.Draw(_bInterp.CurMode);
           
            ConEx.ConEx_Draw.DrawScreen();
        }

        /// <summary>
        /// Gets a character at a certain row and column
        /// </summary>
        /// <param name="row">What row to look in</param>
        /// <param name="column">What column to look in</param>
        /// <returns>The given character, or '\0' if it is out of bounds or had an error</returns>
        public char GetCharacter(int row, int column)
        {
            //Make sure the row and column are in range
            if (row > _boardArray.Count - 1 || row < 0)
            {
                return '\0';
            }
            if (column > _boardArray[row].Count - 1 || column < 0)
            {
                return '\0';
            }

            //If it is, return the character
            return _boardArray[row][column];
        }

        /// <summary>
        /// Inserts a character into the board, 
        /// </summary>
        /// <param name="row">row to insert at</param>
        /// <param name="column">column to insert at</param>
        /// <param name="character">character to put in</param>
        /// <return>If it was able to insert the character</return>
        public bool PutCharacter(int row, int column, char character)
        {
            if (row > _boardArray.Count-1 || row < 0)
            {
                return false;
            }
            if (column > _boardArray[row].Count - 1 || column < 0)
            {
                return false;
            }
            else
            {
                _boardArray[row][column] = character;
                return true;
            }
        }

        public void ClearArea(BoardMode mode)
        {

        }

        /// <summary>
        /// Updates the board based on the mode
        /// </summary>     
        public void UpdateBoard()
        {
            //Keep going until we return something
            while (true)
            {
                //Find out what modifier keys are being held down
                bool shift = ConEx.ConEx_Input.ShiftDown;
                bool alt = ConEx.ConEx_Input.AltDown;
                bool control = ConEx.ConEx_Input.CtrlDown;

                //Get the current keys
                ConsoleKeyInfo[] keysHit = ConEx.ConEx_Input.GetInput();
                CommandType type = _bInterp.Update(_bInterp.CurMode, keysHit);
                                   _bUI.Update(_bInterp.CurMode, keysHit);
                                   _bSideBar.Update(_bInterp.CurMode, keysHit);

                //Based on what mode it is handle those keys
                switch (_bInterp.CurMode)
                {
                    case BoardMode.Run_MAX:
                    case BoardMode.Run_FAST:
                    case BoardMode.Run_MEDIUM:
                    case BoardMode.Run_SLOW:
                    case BoardMode.Run_STEP:
                        break;
                    case BoardMode.Edit:
                        HandleModifiers(_bInterp.CurMode, keysHit);

                        #region --HandleInput-------------
                        for (int i = 0; i < keysHit.Length; i++)
                        {
                            System.ConsoleKey k = keysHit[i].Key;
                            var m = keysHit[i].Modifiers;

                            switch (keysHit[i].Key)
                            {
                                case ConsoleKey.UpArrow:
                                case ConsoleKey.LeftArrow:
                                case ConsoleKey.DownArrow:
                                case ConsoleKey.RightArrow:
                                case ConsoleKey.Spacebar:
                                case ConsoleKey.Backspace:
                                case ConsoleKey.Enter:
                                    break;
                                case ConsoleKey.N:
                                    
                                    break;
                                case ConsoleKey.S:
                                    
                                    break;
                                case ConsoleKey.Escape:
                                    ConEx.ConEx_Draw.FillScreen(' ');
                                    return;//Go back to the main menu
                                case ConsoleKey.F4:
                                    if(ConEx.ConEx_Input.AltDown)
                                    {
                                        Environment.Exit(1);//End the program
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        #endregion HandleInput-------------

                        //After the cursor has finished its drawing restore it to the old position
                        break;
                }//switch(currentMode)
                
                //Draw all components of the board
                //First clear it's area, then draw it
                if (true)
                {
                    //Draw the innocent sidebar
                    _bSideBar.ClearArea(_bInterp.CurMode);
                    _bSideBar.Draw(_bInterp.CurMode);

                    //Draw the board
                    this.ClearArea(_bInterp.CurMode);
                    this.Draw(_bInterp.CurMode);

                    //Draw the UI and selection to override the black
                    _bUI.ClearArea(_bInterp.CurMode);
                    _bUI.Draw(_bInterp.CurMode);

                    //Draw the IP ontop of the board
                    _bInterp.DrawIP();
                    ConEx.ConEx_Draw.DrawScreen();
                }
                //Based on the mode sleep the program so it does not scream by
                System.Threading.Thread.Sleep((int)_bInterp.CurMode);
            }//while(true)
        }//Update()
/// <summary>
        /// Handles all keyboard input which involes Shift, Alt, or Control
        /// </summary>
        /// <param name="mode">The mode of the program you wish to conisder</param>
        /// <param name="keysHit">an array of keys hit</param>
        private void HandleModifiers(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
            //Ensures that the user cannot paste when they out of the window
            if (ConEx.ConEx_Window.IsActive() == false)
            {
                return;
            }

            bool shift = ConEx.ConEx_Input.ShiftDown;
            bool alt = ConEx.ConEx_Input.AltDown;
            bool control = ConEx.ConEx_Input.CtrlDown;
            
            /* X indicates not fully implimented
            * Ctrl + N - New File  
            * Ctrl + S - Save
            * */

            bool n = false;// ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_N);
            if (n && false)
            {
                for (int y = 0; y < _boardArray.Count; y++)
                {
                    for (int x = 0; x < _boardArray[0].Count; x++)
                    {
                        PutCharacter(y, x, ' ');
                    }
                }
                //Emergancy sleep so we don't get a whole bunch of operations at once
                System.Threading.Thread.Sleep(150);
            }

            bool s =  ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_S);

            /* Why is it Alt + S instead of our nice Ctrl + S?
             * That is because Ctrl + S is the ASCII character for X-OFF ('19') and it is hardwired to be on
             * for more see this comment and question.
             * http://stackoverflow.com/questions/26436581/is-it-possible-to-disable-system-console-xoff-xon-flow-control-processing-in-my#comment41529177_26436581
             * Strangly enough this doesn't happen in Debug mode, but does in Release! If anyone knows the answer
             * please get in contact with me.
             * */
            if (s && alt)
            {
                ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_CONTROL);
                SaveBoard();
                //Emergancy sleep so we don't get a whole bunch of operations at once
                System.Threading.Thread.Sleep(150);
            }            
        }

        public void Draw(BoardMode mode)
        {
            for (int row = 0; row < _boardArray.Count; row++)
            {
                for (int column = 0; column < _boardArray[0].Count; column++)
                {
                    char character = GetCharacter(row,column);
                    ConEx.ConEx_Draw.InsertCharacter(character, row, column, LookupInfo(character).color, ConsoleColor.Black);
                }
            }
        }
        

        /// <summary>
        /// Based on the character return the CommandInfo associated with it
        /// </summary>
        /// <param name="inChar">The character to reference</param>
        /// <returns>The corisponding CommandInfo</returns>
        public static CommandInfo LookupInfo(char inChar)
        {
            switch (inChar)
            {
                //Logic
                case '!':
                case '_':
                case '|':
                    return new CommandInfo(inChar, CommandType.Logic, ConsoleColor.DarkGreen, 1);
                case '`':
                case 'w':
                    return new CommandInfo(inChar, CommandType.Logic, ConsoleColor.DarkGreen, 2);

                //Flow control
                case '^':
                case '>':
                case '<':
                case 'v':
                case '?':
                case '#':  
                //Funge-98 flow control
                case '[':
                case ']':
                case 'r':
                case ';':
                    CommandInfo flowCommand = new CommandInfo(inChar, CommandType.Movement, ConsoleColor.Cyan, 0);
                    return flowCommand;
                case 'j':
                case 'k':
                    CommandInfo flowCommand98 = new CommandInfo(inChar, CommandType.Movement, ConsoleColor.Cyan, 1);
                    return flowCommand98;
                case 'x':
                    return new CommandInfo(inChar, CommandType.Movement, ConsoleColor.Cyan, 2);
                case '@':
                case 'q':
                    CommandInfo stopCommand = new CommandInfo(inChar, CommandType.StopExecution, ConsoleColor.Red, 0);
                    return stopCommand;
                    
                
                //Arithmatic
                case '*':
                case '+':
                case '-':
                case '/':
                case '%':
                    CommandInfo arithmaticCommand = new CommandInfo(inChar, CommandType.Arithmatic, ConsoleColor.Green, 2);
                    return arithmaticCommand;
                //Numbers
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    CommandInfo numberCommand = new CommandInfo(inChar, CommandType.Numbers, ConsoleColor.Magenta, -1);
                    return numberCommand;

                //Stack Manipulation
                CommandInfo stackManipCommand;
                case ':':
                    stackManipCommand = new CommandInfo(inChar, 
                                                        CommandType.StackManipulation, 
                                                        ConsoleColor.DarkYellow, 1);//Technically we're not poping
                                                                                    //But it does require something on the stack
                    return stackManipCommand;
                case '$':
                    stackManipCommand = new CommandInfo(inChar, CommandType.StackManipulation, ConsoleColor.DarkYellow, 1);
                    return stackManipCommand;
                case '\\':
                    stackManipCommand = new CommandInfo(inChar, CommandType.StackManipulation, ConsoleColor.DarkYellow, 2);
                    return stackManipCommand;
                case 'n':
                    return new CommandInfo(inChar, CommandType.StackManipulation, ConsoleColor.DarkYellow, 1);//Op must be >=1 on stack
                    
                //IO
                case '&':
                case '~':
                    return new CommandInfo(inChar, CommandType.IO, ConsoleColor.Gray, -1);
                case ',':
                case '.':
                    return new CommandInfo(inChar, CommandType.IO, ConsoleColor.Gray, 1);
                //Funge-98
                case 'i':
                case 'o':
                    return new CommandInfo(inChar,
                                           CommandType.FileIO,
                                           ConsoleColor.Gray,-Int32.MaxValue);//Beware, must be a try catch operation!
                //Data Storage
                case 'g':
                    return new CommandInfo(inChar, CommandType.DataStorage, ConsoleColor.Green, 2);
                case 'p':
                    return new CommandInfo(inChar, CommandType.DataStorage, ConsoleColor.Green, 3);
                //String Manipulation
                case '"':
                    return new CommandInfo(inChar, CommandType.String, ConsoleColor.Green, 0);
                case 't'://Split IP, for concurrent Funge
                    return new CommandInfo(inChar, CommandType.Concurrent, ConsoleColor.DarkBlue, 0);
                case 's':

                case '\''://This is the ' charector
            
                

                //Stack-Stack Manipulation 98
                case 'u':
                case '{':
                case '}':

                //Funge-98 ONLY Schematics
                case '=':
                //Handprint stuff
                case 'y':
                //Footprint stuff
                case '(':
                case ')':
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':

                //Trefunge
                case 'h'://Go high, 3D movement
                case 'l'://Go low, 3D movement
                case 'm'://3D if statment
                case 'z'://Does not exist - TODO its actually nop
                //---------------------------
                    return new CommandInfo(inChar, CommandType.NotImplemented, ConsoleColor.DarkRed, 0);
            }
            return new CommandInfo();
        }

        /// <summary>
        /// Saves the current board to the default save location
        /// </summary>
        private void SaveBoard()
        {
            //Clears the screen and writes info
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;
            System.IO.Directory.SetCurrentDirectory(System.IO.Directory.GetCurrentDirectory() + '\\');
            
            Console.WriteLine("Current Working Directory: " + System.IO.Directory.GetCurrentDirectory());
            Console.WriteLine();
            Console.Write("File Name: ");
            Console.Out.Flush();

            //Read filename from user
            string input = "";
            bool badInput = false;//Assume innocent until proven guilty

            int timeout = 0;
            do
            {
                input = Console.ReadLine();
                input = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory() + input);
                Console.WriteLine();

                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    badInput |= System.IO.Path.GetFileName(input).Contains(c); //A good input will never return true
                }

                foreach (char c in System.IO.Path.GetInvalidPathChars())
                {
                    badInput |= input.Contains(c); //A good input will never return true
                }

                if (badInput == true || input.Count() == 0)
                {
                    Console.WriteLine(input + "is not a valid name, please try again");
                    badInput = true;
                    timeout++;
                }

                if (timeout == 3)
                {
                    Console.WriteLine("File could not be saved, press any key to continue");
                    Console.ReadKey(true);
                    Console.CursorVisible = false;
                    return;
                }
            }
            while(badInput == true);

            //Test the ending
            string extention = System.IO.Path.GetExtension(input);

            //TODO - use extension based off the current mode
            //.uf for unfunge
            //.bf for befunge
            //.b98 for befunge98
            //.tf for trefunge
            switch (extention)
            {
                //If they have included either a .txt or .bf then its okay
                case ".txt":
                case ".bf":
                    break;
                default:
                    input += ".bf";//OptionsManager.OptionsDictionary["Default extension"]
                    break;
            }
            List<string> outStrings = new List<string>();

            for (int y = 0; y < _boardArray.Count; y++)
            {
                string currentLine = null;
                for (int x = 0; x < _boardArray[y].Count; x++)
                {
                    currentLine += _boardArray[y][x].ToString();
                }
                outStrings.Add(currentLine);
            }

            Console.WriteLine("Writing file to " + System.IO.Path.GetFullPath(input));
            Console.WriteLine();

            Exception e = Program.WriteFile(input, outStrings);
            if (e != null)
            {
                Console.WriteLine("Error writing file: " + e.Message);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("File written succesfully!");
                Console.WriteLine();
            }
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
            Console.Clear();
            Console.CursorVisible = false;
        }
    }//class BoardManager
}//Namespace BefungeSharp