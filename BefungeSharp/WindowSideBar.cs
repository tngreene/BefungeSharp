using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp
{
    public class WindowSideBar
    {
        /* The side bar extends from the right side of the board to a location W
         * | [0,82]                 [0,C_Width]
         * |
         * |
         * |
         * |
         * | [C_Height,0]                 [C_Height,C_Width]
         * */
        private const int BAR_TOP = 0;
        private const int BAR_LEFT = 82;
        private int BAR_RIGHT;
        private int BAR_BOTTOM = 0;

        private bool _barShowing;

        private List<string> _content;
        
        public WindowSideBar(BoardManager mgr, Interpreter interp)
        {
            BAR_RIGHT = ConEx.ConEx_Draw.Dimensions.width - 1;
            BAR_BOTTOM = ConEx.ConEx_Draw.Dimensions.height - 1;
            _content = new List<string>();
            
            //TODO - _barShowing = OptionsMenu.GetOption("Show help bar at start")
            _barShowing = true;
        }

        /// <summary>
        /// Draws the User interface of whatever mode the board is in
        /// </summary>
        /// <param name="mode">The mode of the board</param>
        public void Draw(BoardMode mode)
        {
            if(_barShowing == false)
            {
                return;
            }
            /*Template
             * Commands
             * --------
             * Command - Keyboard short cut, notes
            */
            _content.Add(" Commands ");
            _content.Add("----------");

            switch (mode)
            {
                case BoardMode.Run_MAX:
                case BoardMode.Run_FAST:
                case BoardMode.Run_MEDIUM:
                case BoardMode.Run_SLOW:
                case BoardMode.Run_STEP:
                    {
                        string stepInstructions = mode == BoardMode.Run_STEP ? "Next Tick - Right arrow" : "";
                        string[] contentArr = {
                                              "Select Speed - 1 - 5",
                                              "Back to Edit Mode - F12",
                                              stepInstructions
                                              };
                        _content.AddRange(contentArr);
                    }
                    break;
                case BoardMode.Edit:
                    {
                        if(true)//_bUI.IsSelecting == false)
                        {
                        string[] contentArr =   {
                                                    //TODO Organize these
                                                    //X indicates the feature has not been implemented
                                                "New File - Ctrl+N",
                                                "Save - Alt+S",
                                                //"Run " + GetDefaultSpeed() + " - F5");
                                                "Run (Step) - F1",
                                                "Run (Default Speed) - F5",
                                                "Run (Terminal Mode) - F6",
                                                "XReload source - Home",
                                                "Main Menu - Esc",
                                                "Show/Hide Sidebar - F1",
                                                "Set IP Delta - Control + Arrow Key",
                                                "XInsert Snippet - Insert",
                                                "Start Selection Mode - Shift (hold)"
                                                };
                        _content.AddRange(contentArr);
                            }
                        //if (_bUI.IsSelecting)
                        {
                        }
                        //"Adjust Selection Box - Arrow Keys",
                        //"Copy Section - Ctrl + C",
                        //"Cut Section - Ctrl + X",
                        //"Paste Section - Ctrl + V",
                        //"Clear area - Delete",
                        //"Reverse line - Alt + F4"
                        //"Cancel Selection - Any Other Key"
                    }
                    break;
                default:
                    break;
            }

            for (int i = 0; i < _content.Count; i++)
            {
                ConEx.ConEx_Draw.InsertString(_content[i], BAR_TOP + i, BAR_LEFT, false);
            }
            _content.Clear();
        }

        public void ClearArea(BoardMode mode)
        {
            ConEx.ConEx_Draw.FillArea(' ', BAR_TOP, BAR_LEFT, ConEx.ConEx_Draw.Dimensions.width, ConEx.ConEx_Draw.Dimensions.height);
        }  
        
        public void Update(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
            switch(mode)
            {
                default:
                #region --HandleInput-------------
                for (int i = 0; i < keysHit.Length; i++)
                {
                    //--Debugging key presses
                    System.ConsoleKey k = keysHit[i].Key;
                    var m = keysHit[i].Modifiers;
                    //------------------------

                    switch (keysHit[i].Key)
                    {
                        //F1 Shows/Hides the sidebar
                        case ConsoleKey.F1:
                        {
                            _barShowing = !_barShowing;
                            ClearArea(mode);
                            
                            //By adjusting the WindowWidth we save from messing with the buffer
                            //And if someone wants to they can still scroll over.
                            if (_barShowing != true)
                            {
                                Console.WindowWidth -= BAR_RIGHT - BAR_LEFT + 2;
                            }
                            else
                            {
                                Console.WindowWidth += BAR_RIGHT - BAR_LEFT + 2;
                            }
                        }
                        break;
                    }
                }
                break;
            }
            #endregion
        }
    }
}