using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    //2D cardinal directions,
    //Multiply by -1 to invert the direction
    //Numbers 1 and 2 chosen arbitraily
    public enum Direction
    {
        North = -2,
        East = -1,
        South = 2,
        West = 1
    }

    //Types of commands that there could be
    public enum CommandType
    {
              //These are not necissarily complete lists
        Logic,//!_|`
        Movement,//>v^<?#
        Arithmatic,//Operators like +-*/
        Numbers,//0-9,a-f that will be pushed onto the stack
        StackManipulation,//:$
        IO,//&~,.
        DataStorage,//gp
        StopExecution,//@
        String,
        NotImplemented//Many of the Funge-98 instructions
    }

    /// <summary>
    /// Stores some information about a command
    /// </summary>
    public struct CommandInfo
    {
        public char name;//What is the name of it, such as < or | or 4
        public CommandType type;//What type the command is
        public ConsoleColor color;//What color to display it as
        public int numToPop;//Could be 0 (for things like direction),
        //1 for things like _ or |, 2 for most things, or 3 for the rare g and p
        //-1 signifies we are pushing
        
        /// <summary>
        /// A struct to store information relating to a command
        /// </summary>
        /// <param name="inName">The char representing it</param>
        /// <param name="inType">The command type it is</param>
        /// <param name="inColor">How the console should display it</param>
        /// <param name="numToPop">How many pop operations you'll need (-1 for push)</param>
        public CommandInfo(char inName, CommandType inType, ConsoleColor inColor, int numToPop)
        {
            name = inName;
            type = inType;
            color = inColor;
            this.numToPop = numToPop;
        }
    }

    struct Vector2D
    {
        public int x;
        public int y;
    }

    /// <summary>
    /// An enum of how the board should behave while running
    /// </summary>
    public enum BoardMode
    {
        Run_MAX = 1,//Program runs instantanously, user will mostlikely not be able to see the IP or The stacks changing
        Run_FAST = 50,//Program delayed by 50ms. The IP and stacks will move rapidly but visibly
        Run_MEDIUM = 100,//Program delayed by 100ms. IP and stack changes are more easy to follow
        Run_SLOW = 200,//Program delayed by 200ms. IP and stack changes are slow enough to follow on paper
        Run_STEP = 101,//Program delayed until user presses the "Next Step" Key
        Edit = 0//Program is running in edit mode
    }

    public class BoardInterpreter
    {
        //Constants for directions
        const int NORTH = -1;
        const int EAST = 1;
        const int SOUTH = 1;
        const int WEST = -1;

        private BoardManager bRef;

        private bool _debugMode;

        /// <summary>
        /// Represents the main stack
        /// </summary>
        private Stack<int> _globalStack;

        public Stack<int> GlobalStack { get { return _globalStack; } }

        //The current mode of the board
        private BoardMode _curMode;
        public BoardMode CurMode { get { return _curMode; } set { _curMode = value; } }

        //Direction of the instruction pointer
        private Vector2D direction;

        //For if we are currently picking up chars in a string mode
        private bool isStringMode;

        //Instruction Pointer X(column) and Y(row)
        private Vector2D IP;
        private Vector2D Last_IP;

        public int X { get { return IP.x; } }
        public int Y { get { return IP.y; } }

        /// <summary>
        /// Controls the intepretation and execution of commands
        /// </summary>
        /// <param name="mgr">A reference to the manager</param>
        public BoardInterpreter(BoardManager mgr, Stack<int> stack = null, BoardMode mode = BoardMode.Edit)
        {
            bRef = mgr;
            
            //Copy all the data from the initialInput
            if (stack != null)
            {
                _globalStack = new Stack<int>(stack);
            }
            else
            {
                _globalStack = new Stack<int>();
                _globalStack.Push(0);//We always have 0 on the stack
            }

            isStringMode = false;
            SetDirection(Direction.East);

            IP.x = 0;
            IP.y = 0;
        }
        
        public void Reset()
        {
            isStringMode = false;
            SetDirection(Direction.East);
            GlobalStack.Clear();
            GlobalStack.Push(0);
            IP.x = 0;
            IP.y = 0;
        }

        public CommandType Update(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
            //Save the last position because we're about to take a step
            Last_IP = IP;
            CommandType type = CommandType.NotImplemented;

            //Based on what mode it is handle those keys
            switch (mode)
            {
                case BoardMode.Run_MAX:
                case BoardMode.Run_FAST:
                case BoardMode.Run_MEDIUM:
                case BoardMode.Run_SLOW:
                case BoardMode.Run_STEP:
                    //If we're not in stepping mode take a step
                    if (_curMode != BoardMode.Run_STEP)
                    {
                        type = TakeStep();
                    }

                    #region --HandleInput-------------
                    for (int i = 0; i < keysHit.Length; i++)
                    {
                        switch (keysHit[i].Key)
                        {
                            //1-5 adjusts execution speed
                            case ConsoleKey.D1:
                                _curMode = BoardMode.Run_STEP;
                                break;
                            case ConsoleKey.D2:
                                _curMode = BoardMode.Run_SLOW;
                                break;
                            case ConsoleKey.D3:
                                _curMode = BoardMode.Run_MEDIUM;
                                break;
                            case ConsoleKey.D4:
                                _curMode = BoardMode.Run_FAST;
                                break;
                            case ConsoleKey.D5:
                                _curMode = BoardMode.Run_MAX;
                                break;
                            //Takes us back to editor mode
                            case ConsoleKey.F12:
                                _curMode = BoardMode.Edit;
                                break;
                            //Takes the next step
                            case ConsoleKey.RightArrow:
                                if (_curMode == BoardMode.Run_STEP)
                                {
                                    type = TakeStep();
                                }
                                break;
                        }
                    }
                    break;
                    #endregion
                case BoardMode.Edit:
                    bool needsMove = false;
                    #region --HandleInput-------------
                    for (int i = 0; i < keysHit.Length; i++)
                    {
                        //--Debugging key presses
                        System.ConsoleKey k = keysHit[i].Key;
                        var m = keysHit[i].Modifiers;
                        //------------------------

                        switch (keysHit[i].Key)
                        {
                            /*Arrow Keys
                             * Shift + Arrow Key changes IP direction
                             * Arrow Key press:
                             * Save the old direction
                             * Temporarily set the movement for how it should be
                             * Move
                             * Set it back to how it was
                             * Ignore future movement
                             * 
                             * This system makes sure we still use our generalizations and
                             * allows for neat editing tricks
                             */
                            case ConsoleKey.UpArrow:
                                {
                                    if(keysHit[i].Modifiers.HasFlag(ConsoleModifiers.Shift))
                                    {
                                        SetDirection(Direction.North);
                                        break;
                                    }

                                    Direction old = GetDirection();
                                    SetDirection(Direction.North);
                                    MoveIP();
                                    SetDirection(old);
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.LeftArrow:
                                {
                                    if (keysHit[i].Modifiers.HasFlag(ConsoleModifiers.Shift))
                                    {
                                        SetDirection(Direction.West);
                                        break;
                                    }
                                                                    
                                    Direction old = GetDirection();
                                    SetDirection(Direction.West);
                                    MoveIP();
                                    SetDirection(old);
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                {
                                    if (keysHit[i].Modifiers.HasFlag(ConsoleModifiers.Shift))
                                    {
                                        SetDirection(Direction.South);
                                        break;
                                    }
                                                                    
                                    Direction old = GetDirection();
                                    SetDirection(Direction.South);
                                    MoveIP();
                                    SetDirection(old);
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.RightArrow:
                                {
                                    if (keysHit[i].Modifiers.HasFlag(ConsoleModifiers.Shift))
                                    {
                                        SetDirection(Direction.West);
                                        break;
                                    }
                                                                    
                                    Direction old = GetDirection();
                                    SetDirection(Direction.East);
                                    MoveIP();
                                    SetDirection(old);
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.Delete:
                                {
                                    bool success = bRef.InsertChar(IP.y, IP.x, ' ');
                                }
                                break;
                            case ConsoleKey.Backspace:
                                {
                                    Direction old = GetDirection();
                                    SetDirection((Direction)((int)old * -1));//Invert the direction
                                    MoveIP();
                                    bool success = bRef.InsertChar(IP.y, IP.x, ' ');
                                    SetDirection(old);
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.Enter:
                                SetDirection(Direction.East);
                                needsMove = true;
                                break;
                            case ConsoleKey.F5:
                                Reset();
                                _curMode = BoardMode.Run_MEDIUM;
                                break;
                            case ConsoleKey.F6:
                                Reset();
                                _curMode = BoardMode.Run_STEP;
                                break;
                            case ConsoleKey.F12:
                                _curMode = BoardMode.Run_MEDIUM;
                                break;
                            case ConsoleKey.Escape:
                                return type = CommandType.StopExecution;//Go back to the main menu
                            case ConsoleKey.End:
                                Environment.Exit(1);//End the program
                                break;
                            default:
                                if (keysHit[0].KeyChar >= 32 && keysHit[0].KeyChar <= 126)
                                {
                                    bool success = bRef.InsertChar(IP.y, IP.x, keysHit[0].KeyChar);
                                    if (success)
                                    {
                                        needsMove = true;
                                    }
                                }
                                break;
                        }
                    }
                    if (needsMove)
                    {
                        MoveIP();//Move now that we've done some kind of moving input
                    }
                    #endregion HandleInput-------------
                    break;
            }//switch(currentMode)

            
            DrawIP();
            return type;
        }

        private void DrawIP()
        {
            Direction direct = GetDirection();
            
            //Get the last place we were, reset it's color
            //Get our current place, set it's color

            char prevChar = '\0';

            prevChar = bRef.GetCharecter(Last_IP.y, Last_IP.x);
            if (prevChar != '\0')
            {
                ConEx.ConEx_Draw.SetAttributes(Last_IP.y, Last_IP.x, BoardManager.LookupInfo(prevChar).color, ConsoleColor.Black);
            }
            
            //Get the current ip's
            char charecterUnder = bRef.GetCharecter(IP.y, IP.x);
            ConEx.ConEx_Draw.SetAttributes(IP.y, IP.x, BoardManager.LookupInfo(charecterUnder).color, ConsoleColor.Gray);

            Console.SetCursorPosition(IP.x, IP.y);
        }
        
        /// <summary>
        /// Gets the direction the IP is going
        /// </summary>
        /// <returns>The direction the IP is going</returns>
        public Direction GetDirection()
        {
            //If x is not 0 then we are going E or W
            if (direction.x != 0)
            {
                if (direction.x == -1)
                {
                    return Direction.West;
                }
                else
                {
                    return Direction.East;
                }
            }
            else//else we are going N or S
            {
                if (direction.y == -1)
                {
                    return Direction.North;
                }
                else
                {
                    return Direction.South;
                }
            }
        }

        /// <summary>
        /// Changes the IP's direction where you want it to go
        /// </summary>
        /// <param name="intendedDirection">The direction you'd like to set the IP to</param>
        private void SetDirection(Direction intendedDirection)
        {
            switch (intendedDirection)
            {
                case Direction.North:
                    direction.x = 0;
                    direction.y = NORTH;
                    break;
                case Direction.East:
                    direction.x = EAST;
                    direction.y = 0;
                    break;
                case Direction.South:
                    direction.x = 0;
                    direction.y = SOUTH;
                    break;
                case Direction.West:
                    direction.x = WEST;
                    direction.y = 0;
                    break;
            }
        }

        private void MoveIP(int extraAmount = 0)
        {
            //Based on the direction move or wrap the pointer around
            Direction dir = GetDirection();
            switch (dir)
            {
                case Direction.North:
                    IP.y -= 1 + extraAmount;
                    if (IP.y < 0)
                    {
                        IP.y = 24 + IP.y + 1;
                    }
                    break;
                case Direction.East:
                    IP.x += 1 + extraAmount;
                    if (IP.x > 79)
                    {
                        IP.x = IP.x - 79 - 1;
                    }
                    break;
                case Direction.South:
                    IP.y += 1 + extraAmount;
                    if (IP.y > 24)
                    {
                        IP.y = IP.y - 24 - 1;
                    }
                    break;
                case Direction.West:
                    IP.x -= 1 + extraAmount;
                    if (IP.x < 0)
                    {
                        IP.x = 79 + IP.x + 1;
                    }
                    break;
            }
        }

        private CommandType TakeStep()
        {
            /* 1.) Find out what is under the IP
             * 2.) Lookup Info about it
             * 3.) Based on number to pop check to make sure the stack has enough to keep going
             * 4.) Execute Command
             * 5.) Move along delta
             */
            char cmd = bRef.GetCharecter(IP.y, IP.x);

            CommandInfo info = BoardManager.LookupInfo(cmd);

            //If we are currently in string mode
            //And its not a space and not a " (so we can leave string mode)
            if (isStringMode == true && cmd != ' ' && cmd != '"')
            {
                //Push the charecter value, move and return
                GlobalStack.Push((int)cmd);
                MoveIP();
                return CommandType.String;
            }

            //Ensure that there will always be enough in the stack
            while (GlobalStack.Count < info.numToPop)
            {
                GlobalStack.Push(0);
            }

            switch (cmd)
            {
                //Logic
                case '!'://not
                    if (GlobalStack.Pop() != 0)
                    {
                        GlobalStack.Push(0);
                    }
                    else
                    {
                        GlobalStack.Push(1);
                    }
                    break;
                case '_':
                    if (GlobalStack.Pop() == 0)
                    {
                        SetDirection(Direction.East);
                    }
                    else
                    {
                        SetDirection(Direction.West);
                    }
                    break;
                case '|':
                    if (GlobalStack.Pop() == 0)
                    {
                        SetDirection(Direction.South);
                    }
                    else
                    {
                        SetDirection(Direction.North);
                    }
                    break;
                case '`'://Greater than 
                    {
                        int a = GlobalStack.Pop();
                        int b = GlobalStack.Pop();

                        if (b > a)
                        {
                            GlobalStack.Push(1);
                        }
                        else
                        {
                            GlobalStack.Push(0);
                        }
                    }
                    break;
                case 'w'://Funge98 compare function
                    {
                        //Pop a and b off the stack
                        int a = GlobalStack.Pop();
                        int b = GlobalStack.Pop();
                        
                        //Get our current direction
                        Direction currentDir = GetDirection();
                        
                        if (b < a)//If b is less than turn left
                        {
                            switch (currentDir)
                            {
                                case Direction.North:
                                    SetDirection(Direction.West);
                                    break;
                                case Direction.East:
                                    SetDirection(Direction.North);
                                    break;
                                case Direction.South:
                                    SetDirection(Direction.East);
                                    break;
                                case Direction.West:
                                    SetDirection(Direction.South);
                                    break;
                            }
                        }
                        else if (b > a)//if b is more turn right
                        {
                            switch (currentDir)
                            {
                                case Direction.North:
                                    SetDirection(Direction.East);
                                    break;
                                case Direction.East:
                                    SetDirection(Direction.South);
                                    break;
                                case Direction.South:
                                    SetDirection(Direction.West);
                                    break;
                                case Direction.West:
                                    SetDirection(Direction.North);
                                    break;
                            }
                        }
                        else //if b = a do nothing
                        {
                            if (b != a)
                            {
                                int we_have_a_problem = 0;
                                throw new Exception("WTF?!");
                            }
                        }
                    }
                    break;

                //Flow control
                case '^':
                    SetDirection(Direction.North);
                    break;
                case '>':
                    SetDirection(Direction.East);
                    break;
                case '<':
                    SetDirection(Direction.West);
                    break;
                case 'v':
                    SetDirection(Direction.South);
                    break;
                case '?':
                    Random rnd = new Random();
                    SetDirection((Direction)rnd.Next(0, 4));
                    break;
                case '#':
                    MoveIP();//Skip one space
                    break;
                //Funge-98 flow control
                case '['://Rotate 90 degrees counter clockwise
                    {
                        Direction currentDir = GetDirection();
                        switch (currentDir)
                        {
                            case Direction.North:
                                SetDirection(Direction.West);
                                break;
                            case Direction.East:
                                SetDirection(Direction.North);
                                break;
                            case Direction.South:
                                SetDirection(Direction.East);
                                break;
                            case Direction.West:
                                SetDirection(Direction.South);
                                break;
                        }
                    }
                    break;
                case ']'://Rotate 90's clockwise
                    {
                        Direction currentDir = GetDirection();
                        switch (currentDir)
                        {
                            case Direction.North:
                                SetDirection(Direction.East);
                                break;
                            case Direction.East:
                                SetDirection(Direction.South);
                                break;
                            case Direction.South:
                                SetDirection(Direction.West);
                                break;
                            case Direction.West:
                                SetDirection(Direction.North);
                                break;
                        }
                    }
                    break;
                //--Not implemented instructions that will act like r
                //---------------------------------------------------
                case 'r':
                    {
                        Direction currentDir = GetDirection();
                        switch (currentDir)
                        {
                            case Direction.North:
                                SetDirection(Direction.South);
                                break;
                            case Direction.East:
                                SetDirection(Direction.West);
                                break;
                            case Direction.South:
                                SetDirection(Direction.North);
                                break;
                            case Direction.West:
                                SetDirection(Direction.East);
                                break;
                        }
                    }
                    break;
                case ';':
                case 'x'://Absolute delta                 
                case 'j':
                case 'k':
                    break;
                case '@':
                    
                case 'q'://Not fully implimented
                    return CommandType.StopExecution;

                //Arithmatic
                case '+':
                    GlobalStack.Push(GlobalStack.Pop() + GlobalStack.Pop());
                    break;
                case '-'://Subtract b-a
                    {
                        int a = GlobalStack.Pop();
                        int b = GlobalStack.Pop();
                        GlobalStack.Push(b - a);
                    }
                    break;
                case '*':
                    GlobalStack.Push(GlobalStack.Pop() * GlobalStack.Pop());
                    break;
                case '/'://Divide b/a
                    {
                        int a = GlobalStack.Pop();
                        int b = GlobalStack.Pop();
                        double result = b / a;
                        GlobalStack.Push((int)Math.Round(result));
                    }
                    break;
                case '%'://modulous b % a
                    {
                        int a = GlobalStack.Pop();
                        int b = GlobalStack.Pop();
                        GlobalStack.Push(b % a);
                    }
                    break;              
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
                    GlobalStack.Push((int)cmd-48);
                    break;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    GlobalStack.Push((int)cmd - 87);
                    break;
                //Stack Manipulation
                case ':'://Duplication
                    GlobalStack.Push(GlobalStack.Peek());
                    break;
                case '$'://Discard Top Value
                    GlobalStack.Pop();
                    break;
                case '\\'://Swap the top two values
                    {
                        int a = GlobalStack.Pop();
                        int b = GlobalStack.Pop();

                        GlobalStack.Push(a);
                        GlobalStack.Push(b);//Now b is on top
                    }
                    break;
                case 'n'://Clear stack
                    GlobalStack.Clear();
                    break;
                    //IO
                case '&'://Read int
                    string input = Console.ReadLine();
                    int outResult = 0;
                    bool succeded = int.TryParse(input,out outResult);
                    if (succeded == true)
                    {
                        GlobalStack.Push(outResult);
                        bRef.BUI.AddText(input, BoardUI.Categories.IN);
                    }
                    else
                    {
                        GlobalStack.Push(0);
                        bRef.BUI.AddText("0", BoardUI.Categories.IN);
                    }
                    break;
                case '~'://Read char
                    //TODO - allow for mass input
                    char charInput = Console.ReadKey(true).KeyChar;
                    GlobalStack.Push((int)charInput);
                    bRef.BUI.AddText(charInput.ToString(), BoardUI.Categories.IN);
                    break;
                case ','://Output charecter
                    {
                        char outChar = (char)GlobalStack.Pop();
                        string outVal = outChar.ToString();

                        bRef.BUI.AddText(outVal,BoardUI.Categories.OUT);
                    }
                    break;
                case '.'://Output as number
                    bRef.BUI.AddText(GlobalStack.Pop().ToString(),BoardUI.Categories.OUT);
                    break;
                //Funge 98 stack manipulation
                case 'u':
                case '{':
                case '}':
                    break;
                //Funge-98
                case 'i':
                case 'o':
                    try
                    {

                    }
                    catch (Exception e)
                    {

                    }
                    finally
                    {

                    }
                    break;
                //Data Storage
                case 'g':
                    {
                        int y = GlobalStack.Pop();
                        int x = GlobalStack.Pop();
                        char foundChar = bRef.GetCharecter(y,x);
                        GlobalStack.Push((int)foundChar);
                    }
                    break;
                case 'p':
                    {
                        int y = GlobalStack.Pop();
                        int x = GlobalStack.Pop();
                        int charToPlace = GlobalStack.Pop();
                        bool couldPlace = bRef.InsertChar(y,x,(char)charToPlace);

                        //Do this?
                        //if (couldPlace == false)
                        //{
                            //return CommandType.StopExecution;
                        //}
                    }
                    break;
                case 's':

                    break;
                //String Manipulation
                case '"':
                    //Negates and assaigns, a fancy toggle
                    isStringMode = !isStringMode;
                    break;
                case '\''://' "Fetch Charecter", 
                //pushes the next charecter at (pos + delta)'s char value
                //and skips over it, like a # command

                case 't'://Split IP Concurrent

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
                    break;
                
                //no operations
                case ' ':
                    break;
                case 'z'://nop, in 98 it consumes a tick making it different than a simple space
                    break;
                //Trefunge or more
                case 'h':
                case 'l':
                case 'm':
                
                    break;
            }

            MoveIP();
            return info.type;
        }
    }
}
