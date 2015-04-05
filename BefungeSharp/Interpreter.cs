﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BefungeSharp.FungeSpace;
namespace BefungeSharp
{
    
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
        Edit = 0,//Program is running in edit mode
        Debug = 2
    }

    /// <summary>
    /// A Funge Interpreter, containing a FungeSpace, a list of instruction pointers,
    /// and capabilities for editing FungeSpace
    /// </summary>
    public class Interpreter
    {
        /// <summary>
        /// Our Representation of FungeSpace
        /// </summary>
        private FungeSpace.FungeSparseMatrix _fungeSpace;
        public FungeSparseMatrix FungeSpace { get { return _fungeSpace; } }

        /// <summary>
        /// The size of FungeSpace when in Befunge-93 mode
        /// </summary>
        private readonly FungeSpaceArea FS_93;

        /// <summary>
        /// The size of FungeSpace that is automatically created, and filled, at the start of the program
        /// It is only in quadrant 1.
        /// </summary>
        private readonly FungeSpaceArea FS_DEFAULT;

        /// <summary>
        /// A subset of FS_DEFAULT which the program uses for deciding what to draw and what to cull.
        /// Where selection, copy, and paste are allowed to occur are also determined by this.
        /// It will be moveable eventually.
        /// </summary>
        private FungeSpaceArea fs_view_screen;

        /// <summary>
        /// A subset of FS_DEFAULT which the program uses for deciding what portion of FungeSpace is
        /// savable/loadable and what is beyond the ability to. Saving one row of Theoretical FungeSpace results in a 4.3 GB file
        /// Thus we must put a limit at some point. We set it to the default, but it can be changed
        /// </summary>
        private readonly FungeSpaceArea FS_SAVEABLE;
        
        /// <summary>
        /// Extended FungeSpace, the space the interpreter uses. It is sparse, fully addressable, and fully travelable.
        /// </summary>
        private FungeSpaceArea FS_EXTENDED;

        /// <summary>
        /// Theoretical FungeSpace is the FungeSpace described in the language specification
        /// </summary>
        private readonly FungeSpaceArea FS_THEORETICAL;

        //The current mode of the board
        private BoardMode _curMode;
        public BoardMode CurMode { get { return _curMode; } }

        private List<IP> _IPs;

        /// <summary>
        /// The instruction pointer list, 
        /// _IPs[0] is essentially the IP for Unfunge through non concurrent Funge-98 and non-concurrent Tre-Funge
        /// _IPs[1 + n != _IPs.Last()] are only created when using BF98-C
        /// _IPs.Last() is the special "Edit cursor"
        /// </summary>
        public List<IP> IPs { get { return _IPs; } }

        private IP _editIP;

        /// <summary>
        /// The Instruction Pointer representing the editor cursor
        /// </summary>
        public IP EditIP { get { return _editIP; } }

        /// <summary>
        /// Controls the intepretation and execution of commands
        /// </summary>
        /// <param name="mgr">A reference to the manager</param>
        public Interpreter(List<List<int>> initial_chars = null, Stack<int> stack = null, BoardMode mode = BoardMode.Edit)
        {
            //Set up the area's the program will refer to
            FS_93 = new FungeSpaceArea(0, 0, 24, 79);
            FS_DEFAULT = new FungeSpaceArea(0, 0, 24, 79);
            fs_view_screen = FS_93;
            FS_SAVEABLE = FS_DEFAULT;
            FS_EXTENDED = FS_DEFAULT;
            FS_THEORETICAL = new FungeSpaceArea(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);

            //Create FungeSpace, prefilled with ' '
            Console.WriteLine("Creating FungeSpace with a width of {0} and a height of {1}, ",FS_DEFAULT.right,FS_DEFAULT.bottom);
            Console.WriteLine("Please wait");
            
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _fungeSpace = new FungeSparseMatrix(FS_DEFAULT.bottom, FS_DEFAULT.right);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadKey(true);
            if (initial_chars != null)
            {
                //if there is a file to be loaded in, put those into fungespace
                FungeSpaceUtils.DynamicArrayToMatrix(_fungeSpace, initial_chars);
            }

            _IPs = new List<IP>();
            _editIP = new IP(_fungeSpace.Origin, Vector2.East, _fungeSpace.Origin, new Stack<int>(), -1, false);
            
            _curMode = mode;

            Instructions.InstructionManager.BuildInstructionSet();
            
            if (mode == BoardMode.Edit || mode == BoardMode.Debug)
            {
                PauseExecution();
            }
        }

        /// <summary>
        /// Sets up the interpreter to begin executing a program
        /// </summary>
        private void BeginExecution()
        {
            //Reset the IP system
            _IPs.Clear();
            IP.ResetCounter();

            //Add the main thread IP/standard IP
            _IPs.Add(new IP(_fungeSpace.Origin, Vector2.East, _fungeSpace.Origin, new Stack<int>(), 0, false));
            _IPs[0].Active = true;

            //Rebuild the instruction set
            Instructions.InstructionManager.BuildInstructionSet();
        }

        /// <summary>
        /// Pauses the execution of a program to enter edit or debug mode
        /// </summary>
        private void PauseExecution()
        {
            _editIP.Active = true;
        }

        /// <summary>
        /// Prepares the interpreter to continue executing the program 
        /// after being in edit or debug mode
        /// </summary>
        private void UnpauseExecution()
        {
            //Remove the edit IP
            _editIP.Active = false;
        }

        /// <summary>
        /// Finishes the execution and cleans up after itself
        /// </summary>
        private void EndExecution()
        {

        }

        public void ChangeMode(Instructions.IAffectsRunningMode affecter)
        {
            _curMode = affecter.NewMode;
        }

        public Instructions.CommandType Update(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
            Instructions.CommandType type = Instructions.CommandType.NotImplemented;

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
                             * MoveBy(position, direction)
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
                                    Vector2 direction = Vector2.Zero;

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

                                    if (control == true)
                                    {
                                        EditIP.Delta = direction;
                                        break;
                                    }


                                    //Get where we'll be going next
                                    int nextX = EditIP.Position.Data.x + direction.x;
                                    int nextY = EditIP.Position.Data.y + direction.y;

                                    //Since the EditIP wraps around the viewing screen we need to use the old
                                    //Wrapping function
                                    {
                                        //Going in the negative x direction
                                        if (nextX < fs_view_screen.left)
                                        {
                                            nextX = fs_view_screen.right;// +nextX;
                                        }
                                        else if (nextX > fs_view_screen.right)
                                        {
                                            nextX = fs_view_screen.left;
                                        }

                                        //Going in the negative y direction
                                        if (nextY < fs_view_screen.top)
                                        {
                                            nextY = fs_view_screen.bottom;// +nextY;
                                        }
                                        else if (nextY > fs_view_screen.bottom)
                                        {
                                            nextY = fs_view_screen.top;
                                        }

                                        EditIP.Position = FungeSpaceUtils.MoveTo(EditIP.Position, nextY, nextX);
                                    }

                                    needsMove = false;
                                }
                                break;
                            case ConsoleKey.Enter:
                                {
                                    //Move down a line                                    
                                    _IPs[0].Position = FungeSpaceUtils.MoveBy(_IPs[0].Position, new Vector2(0,1));
                                    _IPs[0].Delta = Vector2.East;
                                }
                                break;
                            case ConsoleKey.Delete:
                                {
                                    FungeSpaceUtils.ChangeData(EditIP.Position, ' ');
                                }
                                break;
                            case ConsoleKey.Backspace:
                                {
                                    Vector2 nVec = EditIP.Delta;
                                    nVec.Negate();
                                    EditIP.Position = FungeSpaceUtils.MoveBy(EditIP.Position, nVec);
                                    FungeSpaceUtils.ChangeData(EditIP.Position, ' ');
                                }
                                break;
                            case ConsoleKey.F5:
                                BeginExecution();
                                _curMode = BoardMode.Run_MEDIUM;
                                break;
                            case ConsoleKey.F6:
                                BeginExecution();
                                _curMode = BoardMode.Run_STEP;
                                break;
                            case ConsoleKey.Escape:
                                EndExecution();
                                return type = Instructions.CommandType.StopExecution;//Go back to the main menu
                            default:
                                if (keysHit[i].KeyChar >= 32 && keysHit[i].KeyChar <= 126 
                                    && (ConEx.ConEx_Input.AltDown || ConEx.ConEx_Input.CtrlDown) == false)
                                {
                                    EditIP.Position = _fungeSpace.InsertCell(EditIP.Position.Data.x, EditIP.Position.Data.y, keysHit[0].KeyChar);

                                    int nextX = EditIP.Position.Data.x + EditIP.Delta.x;
                                    int nextY = EditIP.Position.Data.y + EditIP.Delta.y;

                                    if ((nextX >= 0 && nextX <= 79) && (nextY >= 0 && nextY <= 24))
                                    {
                                        EditIP.Position = FungeSpaceUtils.MoveTo(EditIP.Position, nextY, nextX);
                                    }
                                    else
                                    {
                                        needsMove = true;
                                    }
                                }
                                break;
                        }
                    }
                    if (needsMove)
                    {
                        EditIP.Move();//Move now that we've done some kind of moving input
                    }
                    #endregion HandleInput-------------
                    break;
            }//switch(currentMode)

            return type;
        }

        public void Draw()
        {
            DrawFungeSpace();
            if (_curMode != BoardMode.Edit && _curMode != BoardMode.Debug)
            {
                DrawIP();
            }
            else
            {
                DrawEditIP();
            }
        }

        private void DrawFungeSpace()
        {
            FungeSpaceUtils.DrawFungeSpace(_fungeSpace.Origin, fs_view_screen);
        }

        private void DrawIP()
        {
            //For every IP in the list
            for (int n = 0; n < _IPs.Count(); n++)
			{
                if (_IPs[n].Active == false)
                {
                    continue;
                }
                ConsoleColor color = ConsoleColor.White;

                int value = _IPs[n].Position.Data.value;
                if(value >= ' ' && value <= '~')
                {
                    color = Instructions.InstructionManager.InstructionSet[value].Color;
                }
                
                ConEx.ConEx_Draw.SetAttributes(_IPs[n].Position.Data.y, _IPs[n].Position.Data.x, color, (ConsoleColor)ConsoleColor.Gray + (n % 3));
            }
        }
        
        private void DrawEditIP()
        {
            ConsoleColor color = ConsoleColor.White;

            int value = EditIP.Position.Data.value;
            if (value >= ' ' && value <= '~')
            {
                color = Instructions.InstructionManager.InstructionSet[value].Color;
            }

            ConEx.ConEx_Draw.SetAttributes(EditIP.Position.Data.y, EditIP.Position.Data.x, color, ConsoleColor.Gray);
        }

        private Instructions.CommandType TakeStep()
        {
            //Start at the end of the list
            int n = IPs.Count - 1;

            //For every active IP in the list
            do
            {
                //If the IP is active skip over handling it
                if (_IPs[n].Active == false)
                {
                    n--;
                    continue;
                }

                /* If we are in string mode and not ending it, push the value of the command and continue, else
                 * Attempt to consume all whitespace and "ethereal" space 
                 * Attempt the next instruction
                 * If there are no active instructions left, set the interpreter to go back to "edit" mode
                 * Otherwise, move and wrap all IPs
                 */

                int cmd = _fungeSpace.GetNode(_IPs[n].Position.Data.y, _IPs[n].Position.Data.x).Data.value;
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
                if (cmd == ';' || cmd == ' ')
                {
                    Instructions.InstructionManager.InstructionSet[cmd].Preform(_IPs[n]);
                    cmd = _fungeSpace.GetNode(_IPs[n].Position.Data.y, _IPs[n].Position.Data.x).Data.value;
                }             

                bool success = Instructions.InstructionManager.InstructionSet[cmd].Preform(_IPs[n]);

                /*if (success == false)
                    switch (cmd)
                    {                  
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
                        //Trefunge or more
                        case 'h':
                        case 'l':
                        case 'm':

                            break;
                    }*/

                //Increment n
                n--;
            } while (n >= 0);

            //If ther are no more active IP's left then set us back to edit mode
            if (IPs.Exists(item => item.Active == true) == false)
            {
                _curMode = BoardMode.Edit;
            }

            //Move and wrap every delta
            for (int i = 0; i < IPs.Count; i++)
            {
                _IPs[i].Move();
            }

            return Instructions.CommandType.NotImplemented;//TODO - Needs a better idea
        }
    }
}
