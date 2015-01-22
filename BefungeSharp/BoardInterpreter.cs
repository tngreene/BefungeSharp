using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp
{
    //Types of commands that there could be
    public enum CommandType
    {
        //These are not necissarily complete lists
        Movement,//>v^<?
        FlowControl,//#@;jqk
        Logic,//!_|`
        
        Arithmetic,//Operators like +-*/
        Numbers,//0-9,a-f that will be pushed onto the stack
        StackManipulation,//:$u{}
        IO,//&~,.
        FileIO,//io
        DataStorage,//gp
        StopExecution,//@
        String,//"
        Concurrent,//t
        Trefunge,//hlm
        NotImplemented//Many of the Funge-98 instructions. For now! - 12/31/2014
    }

    /// <summary>
    /// Stores some information about a command
    /// </summary>
    public struct CommandInfo
    {
        public char name;//What is the name of it, such as < or | or 4
        public CommandType type;//What type the command is
        public ConsoleColor color;//What color to display it as
        public int requiredCells;//Could be 0 (for things like direction),
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
            this.requiredCells = numToPop;
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
        private BoardManager _boardRef;

        private bool _debugMode;

        //The current mode of the board
        private BoardMode _curMode;
        public BoardMode CurMode { get { return _curMode; } set { _curMode = value; } }

        private List<IP> _IPs;

        /// <summary>
        /// The instruction pointer list, 
        /// _IPs[0] is essentially the IP for Unfunge through non concurrent Funge-98 and non-concurrent Tre-Funge
        /// _IPs[1 + n != _IPs.Last()] are only created when using BF98-C
        /// _IPs.Last() is the special "Edit cursor"
        /// </summary>
        public List<IP> IPs { get { return _IPs; } }

        /// <summary>
        /// The Instruction Pointer representing the editor cursor
        /// </summary>
        public IP EditIP { get { return _IPs.Last(); } }

        private Vector2 _Last_IP;

        private int _IPFollowID;
        public int IPFollowID { get { return _IPFollowID; } set { _IPFollowID = value; } }


        

        /// <summary>
        /// Controls the intepretation and execution of commands
        /// </summary>
        /// <param name="mgr">A reference to the manager</param>
        public BoardInterpreter(BoardManager mgr, Stack<int> stack = null, BoardMode mode = BoardMode.Edit)
        {
            _boardRef = mgr;

            _IPs = new List<IP>();

            //Add the EDIT IP
            _IPs.Add(new IP());
            EditIP.Active = true;

            _curMode = mode;
            _IPFollowID = 0;

            Instructions.InstructionManager.BuildInstructionSet();
        }
        
        public void Reset()
        {
            //Start by removing every except the edit IP
            _IPs.Clear();
            IP.ResetCounter();

            //Add the main thread IP/standard IP
            _IPs.Add(new IP());

            _IPs[0].Reset();
            _IPs[0].Active = true;

            
        }

        public CommandType Update(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
            //Save the last position because we're about to take a step
            _Last_IP = _IPs[0].Position;
            CommandType type = CommandType.NotImplemented;

            bool shift = ConEx.ConEx_Input.ShiftDown;
            bool alt = ConEx.ConEx_Input.AltDown;
            bool control = ConEx.ConEx_Input.CtrlDown;

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
                            case ConsoleKey.RightArrow:
                            case ConsoleKey.DownArrow:
                            case ConsoleKey.LeftArrow:
                                {
                                    
                                    Vector2 direction = new Vector2();

                                    switch (keysHit[i].Key)
                                    {
                                        case ConsoleKey.UpArrow:
                                            direction = Vector2.North;
                                            break;
                                        case ConsoleKey.RightArrow:
                                            direction = Vector2.East;
                                            break;
                                        case ConsoleKey.DownArrow:
                                            direction = Vector2.South;
                                            break;
                                        case ConsoleKey.LeftArrow:
                                            direction = Vector2.West;
                                            break;
                                    }

                                    if(control == true)
                                    {
                                        _IPs[0].Delta = direction;
                                        break;
                                    }

                                    Vector2 old = _IPs[0].Delta;
                                    _IPs[0].Delta = direction;
                                    _IPs[0].Move();
                                    _IPs[0].Delta = old;
                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.Enter:
                                {
                                    //Move down a line                                    
                                    _IPs[0].Position = new Vector2(0, _IPs[0].Position.y + 1);
                                    _IPs[0].Delta = Vector2.East;
                                }
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
                                _curMode = BoardMode.Edit;
                                break;
                            case ConsoleKey.Escape:
                                IP.ResetCounter();
                                return type = CommandType.StopExecution;//Go back to the main menu
                            default:
                                if (keysHit[i].KeyChar >= 32 && keysHit[i].KeyChar <= 126 
                                    && (ConEx.ConEx_Input.AltDown || ConEx.ConEx_Input.CtrlDown) == false)
                                {
                                    bool success = _boardRef.PutCharacter(_IPs[0].Position.y, _IPs[0].Position.x, keysHit[0].KeyChar);
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
            _IPs[0].Position = Wrap(_IPs[0].Position);
            
            
            return type;
        }

        public void DrawIP()
        {
            int n = 0;
            //For every IP in the list
            while(n < _IPs.Count() && _IPs[n].Active == true)
            {
                Vector2 direct = _IPs[n].Delta;

                //Get the last place we were, reset it's color
                //Get our current place, set it's color

                char prevChar = '\0';

                prevChar = _boardRef.GetCharacter(_Last_IP.y, _Last_IP.x);

                if (prevChar != '\0')
                {
                   // ConEx.ConEx_Draw.SetAttributes(_Last_IP.y, _Last_IP.x, BoardManager.LookupInfo(prevChar).color, ConsoleColor.Black);
                }

                //Get the current ip's
                char characterUnder = _boardRef.GetCharacter(_IPs[n].Position.y, _IPs[n].Position.x);
                ConEx.ConEx_Draw.SetAttributes(_IPs[n].Position.y, _IPs[n].Position.x, BoardManager.LookupInfo(characterUnder).color, ConsoleColor.Gray);
                n++;
            }
        }

        public static Vector2 Wrap(Vector2 _position)
        {
            Vector2 newVector = _position;

            //int[] bounds = GetWorldBounds() bounds of world

            //Bounds of Befunge93
            //TODO - if(language == "93")
            //Bounds are Left, Top, Right, Bottom, in their size (not real co-ordinants)
            int[] bounds = { 0, 0, 80, 25};
            
            //Going in the negative x direction
            if (newVector.x < bounds[0])
            {
                newVector.x = bounds[2] + newVector.x;
            }
            else if (newVector.x >= bounds[2])//X is positive
            {
                newVector.x = newVector.x - bounds[2];
            }

            //Going in the negative y direction
            if (newVector.y < bounds[1])
            {
                newVector.y = bounds[3] + newVector.y;
            }
            else if (newVector.y >= bounds[3])//Y is positive
            {
                newVector.y = newVector.y - bounds[3];
            }

            return newVector;
        }

        private CommandType TakeStep()
        {
            //Start at the end of the list
            int n = IPs.Count - 1;

            //For every active IP in the list (except the EditIP)
            while (n >= 0 && _IPs[n].Active)
            {
                /* 1.) Find out what is under the IP
                 * 2.) Lookup Info about it
                 * 3.) Based on number to pop check to make sure the stack has enough to keep going
                 * 4.) Execute Command
                 
                 */
                char cmd = _boardRef.GetCharacter(_IPs[n].Position.y, _IPs[n].Position.x);

                CommandInfo info = BoardManager.LookupInfo(cmd);

                //If we are currently in string mode
                //And its not a space and not a " (so we can leave string mode)
                if (_IPs[n].StringMode == true && cmd != '"')
                {
                    //Push the character value, move and return
                    _IPs[n].Stack.Push((int)cmd);

                    n--;
                    //Move onto the next thread
                    continue;
                }

                bool success = false;
                try
                {
                    success = Instructions.InstructionManager.InstructionSet[cmd].Preform(_IPs[n]);
                }
                catch (Exception)
                {
                    
                    
                }
               
                if (success == false)
                switch (cmd)
                {
                    case '#':
                        _IPs[n].Move();//Skip one space
                        break;
                    case 'j':
                        //.Pop() - 1 to account for the fact we're moving already at the bottom
                        for (int i = 0; i < _IPs[n].Stack.Pop() - 1; i++)
                        {
                            _IPs[n].Move();
                        }
                        break;
                    case 'k':
                        break;
                    case '@':
                        _IPs[n].Active = false;
                        _IPs[n].Stop();
                        if (_IPs.Exists(item => item.Active == true && item.ID != 0))
                        {
                            n--;
                            continue;
                        }
                        else
                        {
                            _curMode = BoardMode.Edit;
                        }
                        break;
                    case 'q'://Not fully implimented
                        _curMode = BoardMode.Edit;//TODO - Change behavior in f98CNote will change when
                        return CommandType.StopExecution;
                    //IO
                    case '&'://Read int
                        string input = Console.ReadLine();
                        int outResult = 0;
                        bool succeded = int.TryParse(input, out outResult);
                        if (succeded == true)
                        {
                            _IPs[n].Stack.Push(outResult);
                            _boardRef.UI.AddText(input, BoardUI.Categories.IN);
                        }
                        else
                        {
                            _IPs[n].Stack.Push(0);
                            _boardRef.UI.AddText("0", BoardUI.Categories.IN);
                        }
                        break;
                    case '~'://Read char
                        //TODO - allow for mass input
                        char charInput = Console.ReadKey(true).KeyChar;
                        _IPs[n].Stack.Push((int)charInput);
                        _boardRef.UI.AddText(charInput.ToString(), BoardUI.Categories.IN);
                        break;
                    case ','://Output character
                        {
                            char outChar = (char)_IPs[n].Stack.Pop();
                            string outVal = outChar.ToString();

                            _boardRef.UI.AddText(outVal, BoardUI.Categories.OUT);
                        }
                        break;
                    case '.'://Output as number
                        _boardRef.UI.AddText(_IPs[n].Stack.Pop().ToString(), BoardUI.Categories.OUT);
                        break;
                    //Funge 98 stack manipulation
                    //TODO - implement
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
                    case 's':

                        break;
                    //String Manipulation
                    case '"':
                        //Negates and assaigns, a fancy toggle
                        _IPs[n].StringMode = !_IPs[n].StringMode;
                        break;
                    case '\''://' "Fetch Character", 
                        //pushes the next character at (pos + delta)'s char value
                        //and skips over it, like a # command
                        break;
                    case 't'://Split IP Concurrent
                        {
                            //A temporary reference to the new IP
                            IP childIP = new IP(_IPs[n]);

                            //Insert before this one
                            _IPs.Insert(n + 1, childIP);
                            
                            childIP.Negate();
                        }
                        break;
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
                    case ';':
                        break;
                    case 'z'://nop, in 98 it consumes a tick making it different than a simple space
                        break;
                    //Trefunge or more
                    case 'h':
                    case 'l':
                    case 'm':

                        break;
                }

                //Increment n
                n--;
            }
            
            //Move and wrap every delta
            for (int i = 0; i < IPs.Count; i++)
            {
                //Activate all the newly born IPs
                _IPs[i].Active = true;
                _IPs[i].Move();
                _IPs[i].Position = Wrap(_IPs[i].Position);
            }
            
            //Although we are switched to the ConEx drawing library this is still important
            //TODO - for what though
            Console.SetCursorPosition(_IPs[_IPFollowID].Position.x, _IPs[_IPFollowID].Position.y);

            return CommandType.NotImplemented;//TODO - Needs a better idea
        }
    }
}
