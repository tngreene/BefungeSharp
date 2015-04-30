using System;
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
        Run_TERMINAL = 3,
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
        public FungeSpaceArea ViewScreen { get { return fs_view_screen; } }
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
            
            //Create FungeSpace, prefilled with ' '
            Console.WriteLine("Creating FungeSpace with a width of {0} and a height of {1}, ",FS_DEFAULT.right,FS_DEFAULT.bottom);
            Console.WriteLine("Please wait");
            
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _fungeSpace = new FungeSparseMatrix(0,0
            );//FS_DEFAULT.bottom, FS_DEFAULT.right);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            //Console.ReadKey(true);
            if (initial_chars != null)
            {
                //if there is a file to be loaded in, put those into fungespace
                FungeSpaceUtils.DynamicArrayToMatrix(_fungeSpace, initial_chars);
            }

            _IPs = new List<IP>();
            _editIP = new IP(_fungeSpace.Origin, Vector2.East, Vector2.Zero, new Stack<int>(), -1, false);
            
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
        private void BeginExecution(BoardMode mode)
        {
            _curMode = mode;

            fs_view_screen = FS_93;
            //Reset the IP system
            _IPs.Clear();
            IP.ResetCounter();

            //Add the main thread IP/standard IP
            _IPs.Add(new IP(_fungeSpace.Origin, Vector2.East, Vector2.Zero, new Stack<int>(), 0, false));
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
            Program.WindowUI.Reset();
        }

        public void ChangeMode(Instructions.IAffectsRunningMode affecter)
        {
            _curMode = affecter.NewMode;

            if (_curMode == BoardMode.Edit)
            {
                EndExecution();
            }
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
                case BoardMode.Run_TERMINAL:
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
                            case ConsoleKey.F1:
                                _curMode = BoardMode.Run_STEP;
                                break;
                            case ConsoleKey.F2:
                                _curMode = BoardMode.Run_SLOW;
                                break;
                            case ConsoleKey.F3:
                                _curMode = BoardMode.Run_MEDIUM;
                                break;
                            case ConsoleKey.F4:
                                _curMode = BoardMode.Run_FAST;
                                break;
                            case ConsoleKey.F5:
                                _curMode = BoardMode.Run_MAX;
                                break;
                            case ConsoleKey.F6:
                                _curMode = BoardMode.Run_TERMINAL;
                                ConEx.ConEx_Draw.FillScreen(' ');
                                ConEx.ConEx_Draw.DrawScreen();
                                Console.CursorLeft = 0;
                                Console.CursorTop = 0;
                                Console.CursorVisible = false;
                                break;
                            //Takes us back to editor mode
                            case ConsoleKey.F12:
                                EndExecution();
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
                             * Tab   + Arrow Key moves view screen
                             * Shift + Arrow Key changes IP direction
                             * Arrow Key press:
                             * MoveTo
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
                                    //If shift is pressed right now WindowUI is about to start a new selection
                                    //So we don't want to move the EditIP right now
                                    if (ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_SHIFT))
                                    {
                                        break;
                                    }
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

                                    if (ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_TAB))
                                    {
                                        //TODO:OPTION? Inverse scrolling or not. direction.Negate();
                                        MoveViewScreen(direction);
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
                                    Vector2 wrappedPosition = WrapEditIPViewScreen(nextX, nextY);
                                    EditIP.Position = FungeSpaceUtils.MoveTo(EditIP.Position, wrappedPosition.y, wrappedPosition.x);
                                }
                                break;
                            case ConsoleKey.Enter:
                                {
                                    //Move down a line                                    
                                    EditIP.Position = FungeSpaceUtils.MoveTo(EditIP.Position, EditIP.Position.Data.y + Vector2.South.y, EditIP.Position.Data.x + Vector2.South.x);
                                    EditIP.Delta = Vector2.East;
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
                                    EditIP.Position = FungeSpaceUtils.MoveTo(EditIP.Position, EditIP.Position.Data.y + nVec.y, EditIP.Position.Data.x + nVec.x);
                                    FungeSpaceUtils.ChangeData(EditIP.Position, ' ');
                                }
                                break;
                            case ConsoleKey.F1:
                                BeginExecution(BoardMode.Run_STEP);
                                break;
                            case ConsoleKey.F5:
                                BeginExecution(BoardMode.Run_MEDIUM);
                                break;
                            case ConsoleKey.F6:
                                BeginExecution(BoardMode.Run_TERMINAL);
                                ConEx.ConEx_Draw.FillScreen(' ');
                                ConEx.ConEx_Draw.DrawScreen();
                                Console.CursorLeft = 0;
                                Console.CursorTop = 0;
                                Console.CursorVisible = false;
                                break;
                            case ConsoleKey.Escape:
                                EndExecution();
                                return type = Instructions.CommandType.StopExecution;//Go back to the main menu
                            default:
                                if (keysHit[i].KeyChar >= ' ' && keysHit[i].KeyChar <= '~' 
                                    && (ConEx.ConEx_Input.AltDown || ConEx.ConEx_Input.CtrlDown) == false)
                                {
                                    EditIP.Position = _fungeSpace.InsertCell(EditIP.Position.Data.x, EditIP.Position.Data.y, keysHit[0].KeyChar);

                                    int nextX = EditIP.Position.Data.x + EditIP.Delta.x;
                                    int nextY = EditIP.Position.Data.y + EditIP.Delta.y;
                                    
                                    EditIP.Position = FungeSpaceUtils.MoveTo(EditIP.Position, nextY, nextX);
                                }
                                break;
                        }
                    }
                    
                    if (_curMode == BoardMode.Edit || _curMode == BoardMode.Debug)
                    {
                        Vector2 confirmedPosition = WrapEditIPViewScreen(EditIP.Position.Data.x, EditIP.Position.Data.y);
                        EditIP.Position = FungeSpaceUtils.MoveTo(EditIP.Position, confirmedPosition.y, confirmedPosition.x);
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
                //If we are going to draw the IP we are following we must ask,
                //if it has gone out of the view screen, past which bound did it go?
                Vector2 followingIPPosition = _IPs[0].Position.Data;//For now, we are always following the main one
                Vector2 moveDirection = Vector2.Zero;
 
                //If it went past the right edge
                if (followingIPPosition.x > fs_view_screen.right)
                {
                    moveDirection = Vector2.East;
                }
                else if (followingIPPosition.x < fs_view_screen.left)
                {
                    moveDirection = Vector2.West;
                }

                if (followingIPPosition.y > fs_view_screen.bottom)
                {
                    moveDirection = Vector2.South;
                }
                else if (followingIPPosition.y < fs_view_screen.top)
                {
                    moveDirection = Vector2.North;
                }
                
                //Keep moving the view screen until the IP we are following is inside it
                while (fs_view_screen.Contains(followingIPPosition.x, followingIPPosition.y) == false)
                {
                    MoveViewScreen(moveDirection);
                }
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
                
                int value = _IPs[n].Position.Data.value;
                FungeNode drawing_position = _IPs[n].Position;

                //If the next space we are at is actually one we will be skipping over then
                //look forward to where we ACTUALLY will be going and draw the IP as there
                //However! If we are in string mode the IP will be traveling to those spaces

                //TODO:It's not looking up ; or ' ', its looking up those instructions,
                //whatever their form might be
                while ((value == ';' || value == ' ') && _IPs[n].StringMode == false)
                {
                    drawing_position = FungeSpaceUtils.MoveBy(drawing_position, _IPs[n].Delta);
                    value = drawing_position.Data.value;
                }
                value = drawing_position.Data.value;
                //Only draw the drawing IP if it is inside the view screen
                if (fs_view_screen.Contains(drawing_position.Data.x, drawing_position.Data.y) == true)
                {
                    ConsoleColor color = ConsoleColor.White;
                    if (value >= ' ' && value <= '~')
                    {
                        color = Instructions.InstructionManager.InstructionSet[value].Color;
                    }

                    ConEx.ConEx_Draw.SetAttributes( drawing_position.Data.y - fs_view_screen.top,
                                                    drawing_position.Data.x - fs_view_screen.left,
                                                    color,
                                                    (ConsoleColor)ConsoleColor.Gray + (n % 3));
                }               
            }
        }
        
        private void DrawEditIP()
        {
            ConsoleColor color = ConsoleColor.White;
            if (fs_view_screen.Contains(EditIP.Position.Data.x, EditIP.Position.Data.y) == true)
            {
                if (Program.WindowUI.SelectionActive == false)
                {
                    int value = EditIP.Position.Data.value;
                    if (value >= ' ' && value <= '~')
                    {
                        color = Instructions.InstructionManager.InstructionSet[value].Color;
                    }

                    ConEx.ConEx_Draw.SetAttributes( EditIP.Position.Data.y - fs_view_screen.top,
                                                    EditIP.Position.Data.x - fs_view_screen.left,
                                                    color,
                                                    ConsoleColor.Gray);
                }
            }
        }

        public void ClearArea()
        {
                                                                           //+1 because FillArea is not inclusive in its area
            ConEx.ConEx_Draw.FillArea(' ', FS_93.top, FS_93.left, FS_93.left + FS_93.right + 1, FS_93.top + FS_93.bottom + 1);
        }

        /// <summary>
        /// Moves the view screen in a certain direction by half the certain axis of FS_93
        /// </summary>
        /// <param name="direction">The direction to move the screen in</param>
        public void MoveViewScreen(Vector2 direction)
        {
            //TODO:MAJOR!MoveViewScreen does not do well when not moving in a + sign
            int xOffset = 16;//TODO:Options["xOffset"]
            int yOffset = 5;
            if(direction == Vector2.North)
            {
                fs_view_screen.top -= yOffset;
                fs_view_screen.bottom =  fs_view_screen.top + (FS_93.Height - 1);
            }
            else if(direction == Vector2.East)
            {
                fs_view_screen.left += xOffset;
                fs_view_screen.right = fs_view_screen.left + (FS_93.Width - 1);
            }
            else if(direction == Vector2.South)
            {
                fs_view_screen.top += yOffset;
                fs_view_screen.bottom =  fs_view_screen.top + (FS_93.Height - 1);
            }
            else if(direction == Vector2.West)
            {
                fs_view_screen.left -= xOffset;
                fs_view_screen.right = fs_view_screen.left + (FS_93.Width - 1);
            }

            if (fs_view_screen.Width != 80 || fs_view_screen.Height != 25)
            {
                int stophere = 0;
            }
        }
        
        /// <summary>
        /// Wraps the EditIP's position inside the View Screen
        /// </summary>
        /// <returns>The new position</returns>
        private Vector2 WrapEditIPViewScreen(int unverifiedX, int unverifiedY)
        {
            if (unverifiedX < fs_view_screen.left)
            {
                unverifiedX = fs_view_screen.right;
            }
            else if (unverifiedX > fs_view_screen.right)
            {
                unverifiedX = fs_view_screen.left;
            }

            //Going in the negative y direction
            if (unverifiedY < fs_view_screen.top)
            {
                unverifiedY = fs_view_screen.bottom;
            }
            else if (unverifiedY > fs_view_screen.bottom)
            {
                unverifiedY = fs_view_screen.top;
            }
            return new Vector2(unverifiedX, unverifiedY);
        }
        
        private Instructions.CommandType TakeStep()
        {
            //For every active IP in the list
            for (int n = _IPs.Count - 1; n >= 0; n--)
            {
                //If the IP isn't active skip over handling it
                if (_IPs[n].Active == false)
                {
                    continue;
                }

                //1.) If we're in string mode, add the push the value
                //2.) Otherwise try as hard as we can to get to the next instruction
                //  - We skip over any space that needs to be skipped
                //  - We execute any command in range
                //3.) We move the IP
                int cmd = _IPs[n].Position.Data.value;
                
                if (_IPs[n].StringMode == true && cmd != '"')
                {
                    //Push a single ' ' then consume all spaces
                    _IPs[n].Stack.Push(cmd);

                    if (/*Language == 98 &&*/cmd == ' ')
                    {
                        Instructions.InstructionManager.InstructionSet[cmd].Preform(_IPs[n]);
                        //We skip moving with .Move() because by now the IP is already in the correct position
                    }
                    //else if(Language == 93)
                    //{

                    //}
                    else
                    {
                        _IPs[n].Move();
                    }
                }
                else 
                {
                    while (cmd == ';' || cmd == ' ')
                    {
                        Instructions.InstructionManager.InstructionSet[cmd].Preform(_IPs[n]);
                        cmd = _IPs[n].Position.Data.value;
                    }
                    if (cmd >= ' ' && cmd <= '~')
                    {
                        bool success = Instructions.InstructionManager.InstructionSet[cmd].Preform(_IPs[n]);
                    }
                    _IPs[n].Move();
                }
            }

            //If there are no more active IP's left then set us back to edit mode
            if (IPs.Exists(item => item.Active == true) == false)
            {
                _curMode = BoardMode.Edit;
            }

            return Instructions.CommandType.NotImplemented;//TODO - Needs a better idea
        }
    }
}
