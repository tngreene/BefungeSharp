using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
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

        
        //For if we are currently picking up chars in a string mode
        private bool isStringMode;

        private List<IP> _IPs;

        /// <summary>
        /// The instruction pointer list, 
        /// _IPs[0] is the special "Edit cursor"
        /// _IPs[1] is essentially the IP for Unfunge through non concurrent Funge-98 and non-concurrent Tre-Funge
        /// _IPs[2 + n] are only created when using BF98-C
        /// </summary>
        public List<IP> IPs { get { return _IPs; } }

        private IP _Last_IP;
        
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

            _IPs = new List<IP>();

            //Add the EDIT IP
            _IPs.Add(new IP());

            //Add the main thread IP/standard IP
            _IPs.Add(new IP());
        }
        
        public void Reset()
        {
            isStringMode = false;
        }

        public CommandType Update(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
            //Save the last position because we're about to take a step
            _Last_IP = _IPs[0];
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
                                        _IPs[0].Delta = Vector2.North;
                                        break;
                                    }

                                    Vector2 old = _IPs[0].Delta;
                                    _IPs[0].Delta = Vector2.North;
                                    _IPs[0].Move();
                                    _IPs[0].Delta = old;
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.LeftArrow:
                                {
                                    if (keysHit[i].Modifiers.HasFlag(ConsoleModifiers.Shift))
                                    {
                                        _IPs[0].Delta = Vector2.West;
                                        break;
                                    }
                                                                    
                                    Vector2 old = _IPs[0].Delta;
                                    _IPs[0].Delta = Vector2.West;
                                    _IPs[0].Move();
                                    _IPs[0].Delta = old;
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                {
                                    if (keysHit[i].Modifiers.HasFlag(ConsoleModifiers.Shift))
                                    {
                                        _IPs[0].Delta = Vector2.South;
                                        break;
                                    }
                                                                    
                                    Vector2 old = _IPs[0].Delta;
                                    _IPs[0].Delta = Vector2.South;
                                    _IPs[0].Move();
                                    _IPs[0].Delta = old;
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.RightArrow:
                                {
                                    if (keysHit[i].Modifiers.HasFlag(ConsoleModifiers.Shift))
                                    {
                                        _IPs[0].Delta = Vector2.East;
                                        break;
                                    }
                                                                    
                                    Vector2 old = _IPs[0].Delta;
                                    _IPs[0].Delta = Vector2.East;
                                    _IPs[0].Move();
                                    _IPs[0].Delta = old;
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.Delete:
                                {
                                    bool success = bRef.InsertChar(_IPs[0].Position.y, _IPs[0].Position.x, ' ');
                                }
                                break;
                            case ConsoleKey.Backspace:
                                {
                                    Vector2 old = _IPs[0].Delta;
                                    _IPs[0].Delta.Negate();
                                    _IPs[0].Move();
                                    bool success = bRef.InsertChar(_IPs[0].Position.y, _IPs[0].Position.x, ' ');
                                    _IPs[0].Delta = old;
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.Enter:
                                _IPs[0].Delta = Vector2.East;
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
                                if (keysHit[i].KeyChar >= 32 && keysHit[i].KeyChar <= 126 
                                    && keysHit[i].Modifiers.HasFlag(ConsoleModifiers.Alt | ConsoleModifiers.Control) == false)
                                {
                                    bool success = bRef.InsertChar(_IPs[0].Position.y, _IPs[0].Position.x, keysHit[0].KeyChar);
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
                        _IPs[0].Move();//Move now that we've done some kind of moving input
                    }
                    #endregion HandleInput-------------
                    break;
            }//switch(currentMode)

            
            DrawIP();
            return type;
        }

        private void DrawIP()
        {
            Vector2 direct = _IPs[0].Delta;
            
            //Get the last place we were, reset it's color
            //Get our current place, set it's color

            char prevChar = '\0';

            prevChar = bRef.GetCharecter(_Last_IP.Position.y, _Last_IP.Position.x);
            if (prevChar != '\0')
            {
                ConEx.ConEx_Draw.SetAttributes(_Last_IP.Position.y, _Last_IP.Position.x, BoardManager.LookupInfo(prevChar).color, ConsoleColor.Black);
            }
            
            //Get the current ip's
            char charecterUnder = bRef.GetCharecter(_IPs[0].Position.y, _IPs[0].Position.x);
            ConEx.ConEx_Draw.SetAttributes(_IPs[0].Position.y, _IPs[0].Position.x, BoardManager.LookupInfo(charecterUnder).color, ConsoleColor.Gray);

            //Although we are switched to the ConEx drawing library this is still important
            Console.SetCursorPosition(_IPs[0].Position.x, _IPs[0].Position.y);
        }

        public static Vector2 Wrap(Vector2 _position)
        {
            Vector2 newVector = _position;

            //int[] bounds = GetWorldBounds() bounds of world

            //Bounds of Befunge93
            //TODO - if(language == "93")
            //Bounds are Left, Top, Right, Bottom, in their size (not real co-ordinants)
            int[] bounds = { 0, 0, 80, 25 };

            int xAmount = newVector.x % bounds[2];
            int yAmount = newVector.y % bounds[3];
            //Going in the negative x direction
            if (newVector.x < bounds[0])
            {
                newVector.x = bounds[2] + xAmount;// newVector.x;
            }
            else if (newVector.x > bounds[2])//X is positive
            {
                newVector.x = xAmount - bounds[0];//newVector.x - bounds[0];
            }

            //Going in the negative y direction
            if (newVector.y < bounds[1])
            {
                newVector.y = bounds[3] + yAmount;// newVector.y;
            }
            else if (newVector.y > bounds[3])//X is positive
            {
                newVector.y = yAmount - bounds[1];// newVector.y - bounds[1];
            }

            Console.WriteLine("X: " + newVector.x + ", Y:" + newVector.y);
            return newVector;
        }

        private CommandType TakeStep()
        {
            /* 1.) Find out what is under the IP
             * 2.) Lookup Info about it
             * 3.) Based on number to pop check to make sure the stack has enough to keep going
             * 4.) Execute Command
             * 5.) Move along delta
             */
            char cmd = bRef.GetCharecter(_IPs[0].Position.y, _IPs[0].Position.x);

            CommandInfo info = BoardManager.LookupInfo(cmd);

            //If we are currently in string mode
            //And its not a space and not a " (so we can leave string mode)
            if (isStringMode == true && cmd != ' ' && cmd != '"')
            {
                //Push the charecter value, move and return
                GlobalStack.Push((int)cmd);
                _IPs[0].Move();
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
                        _IPs[0].Delta = Vector2.East;
                    }
                    else
                    {
                        _IPs[0].Delta = Vector2.West;
                    }
                    break;
                case '|':
                    if (GlobalStack.Pop() == 0)
                    {
                        _IPs[0].Delta = Vector2.South;
                    }
                    else
                    {
                        _IPs[0].Delta = Vector2.North;
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
                        Vector2 currentDir = _IPs[0].Delta;
                        
                        if (b < a)//If b is less than turn left
                        {
                            _IPs[0].Delta = new Vector2(_IPs[0].Delta.y * -1, _IPs[0].Delta.x);
                        }
                        else if (b > a)//if b is more turn right
                        {
                            _IPs[0].Delta = new Vector2(_IPs[0].Delta.y, _IPs[0].Delta.x * -1);
                        }
                    }
                    break;

                //Flow control
                case '^':
                    _IPs[0].Delta = Vector2.North;
                    break;
                case '>':
                    _IPs[0].Delta = Vector2.East;
                    break;
                case '<':
                    _IPs[0].Delta = Vector2.West;
                    break;
                case 'v':
                    _IPs[0].Delta = Vector2.South;
                    break;
                case '?':
                    Random rnd = new Random();
                    _IPs[0].Delta = Vector2.CardinalDirections[rnd.Next(0, 4)];
                    break;
                case '#':
                    _IPs[0].Move();//Skip one space
                    break;
                //Funge-98 flow control
                case '['://Rotate 90 degrees counter clockwise
                    {
                        _IPs[0].Delta = new Vector2(_IPs[0].Delta.y * -1, _IPs[0].Delta.x);
                    }
                    break;
                case ']'://Rotate 90's clockwise
                    {
                        _IPs[0].Delta = new Vector2(_IPs[0].Delta.y, _IPs[0].Delta.x * -1);
                    }
                    break;
                //--Not implemented instructions that will act like r
                //---------------------------------------------------
                case 'r':
                    {
                        _IPs[0].Delta.Negate();
                    }
                    break;
                case ';':
                case 'x':
                    {
                        Vector2 nDelta = new Vector2();
                        nDelta.x = _IPs[0].Stack.Pop();
                        nDelta.y = _IPs[0].Stack.Pop();

                        _IPs[0].Delta = nDelta;
                    }
                    break;
                case 'j':
                    //.Pop() - 1 to account for the fact we're moving already at the bottom
                    for (int i = 0; i < _globalStack.Pop() - 1 ; i++)
			        {
                        _IPs[0].Move();
			        }
                    break;
                case 'k':
                    break;
                case '@':
                    
                case 'q'://Not fully implimented
                    _curMode = BoardMode.Edit;//TODO - Change behavior in f98CNote will change when
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

            _IPs[0].Move();
            return info.type;
        }
    }
}
