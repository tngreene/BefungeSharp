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
        /// Creates a new BoardManager with the options to set up its entire intial state and run type
        /// </summary>
        /// <param name="init_board">
        /// Each element in the array represents a row of text.
        /// If you wish a blank board pass in an empty array
        /// </param>
        /// <param name="initGlobalStack">Initialize the input stack with preset numbers</param>
        /// <param name="mode">Chooses what mode you would like to start the board in</param>
        public BoardManager(List<List<int>> init_board,
                            Stack<int> initGlobalStack = null, BoardMode mode = BoardMode.Edit)
        {
            Program.Interpreter = new Interpreter(init_board, initGlobalStack, mode);
            Program.WindowUI = new WindowUI();
            Program.WindowSideBar = new WindowSideBar();
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
                IEnumerable<ConsoleKeyInfo> keysHit = ConEx.ConEx_Input.GetInput();
                
                Instructions.CommandType type =  Program.Interpreter.Update(Program.Interpreter.CurMode, keysHit);
                                                 Program.WindowUI.Update(Program.Interpreter.CurMode, keysHit);
                                                 Program.WindowSideBar.Update(Program.Interpreter.CurMode, keysHit);

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
                        for (int i = 0; i < keysHit.Count(); i++)
                        {
                            //System.ConsoleKey k = keysHit.ElementAt(i).Key;
                            //var m = keysHit.ElementAt(i).Modifiers;

                            switch (keysHit.ElementAt(i).Key)
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
                                        Program.QuitProgram(0);
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
                    Draw(Program.Interpreter.CurMode);
                }
                double mm = watch.ElapsedMilliseconds;
                //Based on the mode sleep the program so it does not scream by
                System.Threading.Thread.Sleep(ClockDelay(Program.Interpreter.CurMode));
            }//while(true)
        }//Update()
        
        /// <summary>
        /// Handles all keyboard input which involes Shift, Alt, or Control
        /// </summary>
        /// <param name="mode">The mode of the program you wish to conisder</param>
        /// <param name="keysHit">an array of keys hit</param>
        private void HandleModifiers(BoardMode mode, IEnumerable<ConsoleKeyInfo> keysHit)
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

            bool n = ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_N);
            if (n && control)
            {
                //Tell the interpreter to reset
                //Program.Interpreter.Reset()
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
                Menus.SaveSimpleMenu save = new Menus.SaveSimpleMenu();
                save.RunLoop();

                //Emergancy sleep so we don't get a whole bunch of operations at once
                System.Threading.Thread.Sleep(150);
            }            
        }

        public void Draw(BoardMode mode)
        {
            if (mode == BoardMode.Run_TERMINAL)
            {
                return;
            }
            //Draw the innocent sidebar
            Program.WindowSideBar.ClearArea(Program.Interpreter.CurMode);
            Program.WindowSideBar.Draw(Program.Interpreter.CurMode);

            //Draw the board and draw the IP ontop of the board
            Program.Interpreter.ClearArea();
            Program.Interpreter.Draw();

            //Draw the UI and selection to override the black
            Program.WindowUI.ClearArea(Program.Interpreter.CurMode);
            Program.WindowUI.Draw(Program.Interpreter.CurMode);

            ConEx.ConEx_Draw.DrawScreen();
        }

        /// <summary>
        /// How much delay between clock ticks there is, in milliseconds
        /// </summary>
        public static int ClockDelay(BoardMode mode)
        {
            //Translate between the current number and desired number of
            //milliseconds. Milliseconds 50,100,200 were chosen because
            //they seemed to work well
            switch (mode)
            {
                case BoardMode.Run_MAX:
                    return 0;
                case BoardMode.Run_TERMINAL:
                    return 0;
                case BoardMode.Run_FAST:
                    return 50;
                case BoardMode.Run_MEDIUM:
                    return 100;
                case BoardMode.Run_SLOW:
                    return 200;
                case BoardMode.Run_STEP:
                    return 100;
                case BoardMode.Edit:
                case BoardMode.Debug:
                default:
                    return 0;
            }
        }
    }//class BoardManager
}//Namespace BefungeSharp