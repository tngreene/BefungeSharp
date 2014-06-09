using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    /// <summary>
    /// An enum of how the board should behave while running
    /// </summary>
    enum BoardMode
    {
        Run_MAX = 0,//Program runs instantanously, user will mostlikely not be able to see the IP or The stacks changing
        Run_FAST = 50,//Program delayed by 50ms. The IP and stacks will move rapidly but visibly
        Run_MEDIUM = 100,//Program delayed by 100ms. IP and stack changes are more easy to follow
        Run_SLOW = 200,//Program delayed by 200ms. IP and stack changes are slow enough to follow on paper
        Run_STEP = 101,//Program delayed until user presses the "Next Step" Key
        Edit = 20//Program is running in edit mode
    }
    

    class BoardManager
    {
        /// <summary>
        /// Represents a 2 dimensional space of charecters, non jagged
        /// Accessed with boardArray[row+i][column+j]
        /// </summary>
        private List<List<char>> boardArray;
        public List<List<char>> BoardArray { get { return boardArray; } }

        /// <summary>
        /// Represents the main stack
        /// </summary>
        private Stack<int> globalStack;

        public Stack<int> GlobalStack { get { return globalStack; } }

        /// <summary>
        /// A stack of stacks for our custom foot prints allowing functions and local stacks
        /// </summary>
        private Stack<Stack<int>> localStacks;

        /// <summary>
        /// A stack that the input values are in put in
        /// </summary>
        private Stack<int> inputStack;

        /// <summary>
        /// Flag to determine if the board needs to be redrawn
        /// </summary>
        private bool needsRedraw;

        private BoardField bF;
        private BoardUI bUI;
        public BoardUI BUI { get { return bUI; } }

        private BoardInterpreter bInterp;

        //The current mode of the board
        private BoardMode curMode;

        /*/// <summary>
        /// Creates a completely blank board
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="columns">Number of rows</param>
        public BoardManager(int rows, int columns)
        {
            boardArray = new List<List<char>>(rows);

            //Fill up the whole rectangle with spaces
            for (int y = 0; y < rows; y++)
            {
                boardArray.Add(new List<char>());
                for (int x = 0; x < columns; x++)
                {
                    boardArray[y].Add(' ');
                }
            }

            //Initialize stacks
            globalStack = new Stack<int>();
            localStacks = new Stack<Stack<int>>();
            inputStack = new Stack<int>();

            //Nothing yet to draw
            needsRedraw = true;
            curMode = BoardMode.Edit;
//            DrawUI(curMode);
//            DrawField();
            Console.SetCursorPosition(0, 0);
            
            
            bF = new BoardField(this);
            bUI = new BoardUI(this);
            bInterp = new BoardInterpreter(this);
        }*/

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
        public BoardManager(int rows, int columns, string[] initChars = null,
                            int[] initGlobalStack = null, BoardMode mode = BoardMode.Edit)
        {
            //Intialize the board array to be the size of the board
            boardArray = new List<List<char>>(rows);

            //Initialize stacks
            //globalStack = new Stack<int>();
            localStacks = new Stack<Stack<int>>();

            //Copy all the data from the initialInput
            if (initGlobalStack != null)
            {
                globalStack = new Stack<int>(initGlobalStack);
            }
            else
            {
                globalStack = new Stack<int>();
            }
            needsRedraw = true;
            curMode = mode;

            //Fill up the whole rectangle with spaces
            for (int y = 0; y < rows; y++)
            {
                boardArray.Add(new List<char>());
                for (int x = 0; x < columns; x++)
                {
                    boardArray[y].Add(' ');
                }
            }

            //TODO: Array size checking to make sure it will not be out of bounds?
            if (initChars != null)
            {
                //Fill board it initial strings, if initChars is null this will skip
                //For the number of rows
                for (int y = 0; y < initChars.Length; y++)
                {
                    //Get the current line
                    string currentLine = initChars[y];

                    //For all the charecters in the line
                    for (int x = 0; x < currentLine.Length; x++)
                    {
                        //Insert them into the array
                        InsertChar(y, x, currentLine[x]);
                    }
                }
            }
            //Console.SetCursorPosition(0, 0);
            //DrawUI(curMode);
            //DrawField();

            //Create the components
            bF = new BoardField(this);
            bUI = new BoardUI(this);
            bInterp = new BoardInterpreter(this);

            //Draw the field and ui and reset the position
            bF.Draw(curMode);
            bUI.Draw(curMode);
            Console.SetCursorPosition(0, 0);
        }

        /// <summary>
        /// Inserts a charecter into the board, 
        /// </summary>
        /// <param name="row">row to insert at</param>
        /// <param name="column">column to insert at</param>
        /// <param name="charecter">charecter to put in</param>
        /// <return>If it was able to insert the charecter</return>
        public bool InsertChar(int row, int column, char charecter)
        {
            if (row > boardArray.Count-1)
            {
                return false;
            }
            if (column > boardArray[row].Count - 1)
            {
                return false;
            }
            else
            {
                boardArray[row][column] = charecter;
                Console.SetCursorPosition(column, row);
                Console.ForegroundColor = LookupInfo(charecter).color;
                Console.Write(charecter);
                //needsRedraw = true;
                return true;
            }
        }

        /// <summary>
        /// Updates the board based on the mode
        /// </summary>     
        public void UpdateBoard()
        {
            //Stores the cursor left position
            int editCursorL = Console.CursorLeft;

            //Stores the cursor top position
            int editCursorT = Console.CursorTop;

            //Keep going until we return something
            while (true)
            {
                //Get the current keys
                ConsoleKeyInfo[] keysHit = HandleInput();
                CommandType type;
                //Based on what mode it is handle those keys
                switch (curMode)
                {
                    case BoardMode.Run_MAX:
                    case BoardMode.Run_FAST:
                    case BoardMode.Run_MEDIUM:
                    case BoardMode.Run_SLOW:
                    case BoardMode.Run_STEP:
                        type = bInterp.Update();
                        
                        bUI.ClearArea(curMode);
                        bUI.Draw(curMode);
                       
                        if (type == CommandType.StopExecution)
                        {
                            curMode = BoardMode.Edit;
                            //Draw the field and ui and reset the position
                            bF.Draw(curMode);
                            bUI.ClearArea(curMode);
                            bUI.Draw(curMode);
                            Console.SetCursorPosition(0, 0);
                        }
                        break;
                    case BoardMode.Edit:
                        Console.CursorVisible = true;
                        #region --HandleInput-------------
                        for (int i = 0; i < keysHit.Length; i++)
                        {
                            switch (keysHit[i].Key)
                            {
                                //Arrow Keys move the cursor
                                case ConsoleKey.UpArrow:
                                    if (editCursorT > 0)
                                    {
                                        editCursorT--;
                                    }
                                    break;
                                case ConsoleKey.LeftArrow:
                                    if (editCursorL > 0)
                                    {
                                        editCursorL--;
                                    }
                                    break;
                                case ConsoleKey.DownArrow:
                                    if (editCursorT < boardArray.Count-1)
                                    {
                                        editCursorT++;
                                    }
                                    break;
                                case ConsoleKey.RightArrow:
                                    if (editCursorL < Console.WindowWidth - 1)
                                    {
                                        editCursorL++;
                                    }
                                    break;
                                case ConsoleKey.Spacebar:
                                    InsertChar(editCursorT, editCursorL, ' ');
                                    if (editCursorL < Console.WindowWidth - 1)
                                    {
                                        editCursorL++;
                                    }
                                    break;
                                case ConsoleKey.Backspace:
                                    if (editCursorL > 0)
                                    {
                                        editCursorL--;
                                    }
                                    InsertChar(editCursorT, editCursorL, ' ');
                                    //editCursorL--;
                                    break;
                                case ConsoleKey.Enter:
                                    Console.SetCursorPosition(0, editCursorT + 1);
                                    break;
                                case ConsoleKey.F5:
                                    curMode = BoardMode.Run_MEDIUM;
                                    Console.CursorVisible = false;

                                    //Reset UI
                                    bUI.OutputList.Clear();
                                    bUI.ClearArea(curMode);

                                    //Reset Interpreter
                                    bInterp = new BoardInterpreter(this);
                                    break;
                                case ConsoleKey.Escape:
                                    return;//Go back to the main menu
                                case ConsoleKey.End:
                                    Environment.Exit(1);//End the program
                                    break;
                                default:
                                    if (keysHit[0].KeyChar > 32 && keysHit[0].KeyChar < 126)
                                    {
                                       bool success = InsertChar(editCursorT, editCursorL, keysHit[0].KeyChar);

                                        //Go to the next space, if you can
                                        if (editCursorL < Console.WindowWidth - 1 && success == true)
                                        {
                                            editCursorL++;
                                        }
                                    }
                                    break;
                            }
                        }
                        #endregion HandleInput-------------
                        
                        //After the cursor has finished its drawing restore it to the old position
                        Console.SetCursorPosition(editCursorL, editCursorT);
                        break;
                }//switch(currentMode)

                //Based on the mode sleep the program so it does not scream by
                System.Threading.Thread.Sleep((int)curMode);
            }//while(true)
        }//Update()

        /// <summary>
        /// Gets a charecter at a certain row and column
        /// </summary>
        /// <param name="row">What row to look in</param>
        /// <param name="column">What column to look in</param>
        /// <returns>The given charecter, or '\0' if it is out of bounds or had an error</returns>
        public char GetCharecter(int row, int column)
        {
            //Make sure the row and column are in range
            if (row > boardArray.Count-1 || row < 0)
            {
                return '\0';
            }
            if (column > boardArray[row].Count - 1 || column < 0)
            {
                return '\0';
            }

            //If it is, return the charecter
            return boardArray[row][column];
        }

        /// <summary>
        /// Get which keys are currently pressed down and return them
        /// </summary>
        private ConsoleKeyInfo[] HandleInput()
        {
            // A list of characters
            List<ConsoleKeyInfo> input = new List<ConsoleKeyInfo>();

            // Loop while keys are available or we hit 10 keys
            for (int i = 0; Console.KeyAvailable && i < 10; i++)
            {
                // Read a key (preventing it from being printed) 
                // and put it in the key list (if it's not in there yet)
                ConsoleKeyInfo info = Console.ReadKey(true);
                if (!input.Contains(info))
                {
                    input.Add(info);
                }
            }

            // Use up any remaining key presses
            while (Console.KeyAvailable)
            {
                // Read a single key
                Console.ReadKey(true);
            }

            // Convert the list to an array and return
            return input.ToArray();
        }

        /// <summary>
        /// Based on the charecter return the CommandInfo associated with it
        /// </summary>
        /// <param name="inChar">The charecter to reference</param>
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
                CommandInfo ioCommand;
                case '&':
                case '~':
                    ioCommand = new CommandInfo(inChar, CommandType.IO, ConsoleColor.Gray, -1);
                    return ioCommand;
                case ',':
                case '.':
                    ioCommand = new CommandInfo(inChar, CommandType.IO, ConsoleColor.Gray, 1);
                    return ioCommand;
                //Funge-98
                case 'i':
                case 'o':
                    return new CommandInfo(inChar,
                                           CommandType.IO,
                                           ConsoleColor.Gray,-Int32.MaxValue);//Beware, must be a try catch operation!
                //Data Storage
                case 'g':
                case 'p':
                    return new CommandInfo(inChar, CommandType.DataStorage, ConsoleColor.Green, 3);
                //String Manipulation
                case '"':
                    return new CommandInfo(inChar, CommandType.String, ConsoleColor.Green, 0);
                case 's':

                case '\''://This is the ' charector
            
                case 't':

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

                //--Will never be implemented
                case 'h'://Go high, 3D movement
                case 'l'://Go low, 3D movement
                case 'm'://3D if statment
                case 'z'://Does not exist
                //---------------------------
                    return new CommandInfo(inChar, CommandType.NotImplemented, ConsoleColor.DarkRed, 0);
            }
            return new CommandInfo();
        }
    }
}