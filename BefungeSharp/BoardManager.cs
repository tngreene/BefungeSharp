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
        /// A two dimensional grid of the original file input, if there was any
        /// </summary>
        private List<List<int>> _boardArray;
        public List<List<int>> BoardArray { get { return _boardArray; } }
        

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
            //TODO: Array size checking to make sure it will not be out of bounds?
            if (initChars != null)
            {
                //Intialize the board array to be the size of the board
                _boardArray = new List<List<int>>(rows);

                //Fill up the whole rectangle with spaces
                for (int y = 0; y < rows; y++)
                {
                    _boardArray.Add(new List<int>());
                    for (int x = 0; x < columns; x++)
                    {
                        _boardArray[y].Add(' ');
                    }
                }

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

            Program.Interpreter = new Interpreter(this._boardArray, initGlobalStack, mode);

            Program.WindowUI = new WindowUI(Program.Interpreter);
            Program.WindowSideBar = new WindowSideBar(this, Program.Interpreter);
          
            Console.CursorVisible = false;
         
            //Draw the field and ui and reset the position
            Program.WindowUI.ClearArea(Program.Interpreter.CurMode);
            Program.WindowUI.Draw(Program.Interpreter.CurMode);

            Program.WindowSideBar.ClearArea(Program.Interpreter.CurMode);
            Program.WindowSideBar.Draw(Program.Interpreter.CurMode);
           
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
            return (char)_boardArray[row][column];
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

                var watch = System.Diagnostics.Stopwatch.StartNew();
                //Find out what modifier keys are being held down
                bool shift = ConEx.ConEx_Input.ShiftDown;
                bool alt = ConEx.ConEx_Input.AltDown;
                bool control = ConEx.ConEx_Input.CtrlDown;

                //Get the current keys
                ConsoleKeyInfo[] keysHit = ConEx.ConEx_Input.GetInput();
                Instructions.CommandType type =  Program.Interpreter.Update( Program.Interpreter.CurMode, keysHit);
                                   Program.WindowUI.Update( Program.Interpreter.CurMode, keysHit);
                                   Program.WindowSideBar.Update( Program.Interpreter.CurMode, keysHit);

                //Based on what mode it is handle those keys
                switch ( Program.Interpreter.CurMode)
                {
                    case BoardMode.Run_MAX:
                    case BoardMode.Run_FAST:
                    case BoardMode.Run_MEDIUM:
                    case BoardMode.Run_SLOW:
                    case BoardMode.Run_STEP:
                        break;
                    case BoardMode.Edit:
                        HandleModifiers( Program.Interpreter.CurMode, keysHit);

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
                    Program.WindowSideBar.ClearArea( Program.Interpreter.CurMode);
                    Program.WindowSideBar.Draw( Program.Interpreter.CurMode);

                    //Draw the board and draw the IP ontop of the board
                     Program.Interpreter.Draw();

                    //Draw the UI and selection to override the black
                    Program.WindowUI.ClearArea( Program.Interpreter.CurMode);
                    Program.WindowUI.Draw( Program.Interpreter.CurMode);

                    ConEx.ConEx_Draw.DrawScreen();
                }
                double mm = watch.ElapsedMilliseconds;
                //Based on the mode sleep the program so it does not scream by
                System.Threading.Thread.Sleep((int) Program.Interpreter.CurMode);
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

            Exception e = FileUtils.WriteFile(input, outStrings);
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