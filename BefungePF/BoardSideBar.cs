using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    public class BoardSideBar
    {
        private BoardManager _boardRef;
        private BoardInterpreter _interpRef;
        
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
        private List<string> _content;
        
        public BoardSideBar(BoardManager mgr, BoardInterpreter interp)
        {
            _boardRef = mgr;
            _interpRef = interp;
            BAR_RIGHT = ConEx.ConEx_Draw.Dimensions.width - 1;
            BAR_BOTTOM = ConEx.ConEx_Draw.Dimensions.height - 1;
            _content = new List<string>();
        }

        /// <summary>
        /// Draws the User interface of whatever mode the board is in
        /// </summary>
        /// <param name="mode">The mode of the board</param>
        public void Draw(BoardMode mode)
        {
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
                    break;
                case BoardMode.Edit:
                    {
                        string[] contentArr =   {
                                                "New File - Ctrl+N",
                                                "Save - Alt+S",
                                                //"Run " + GetDefaultSpeed() + " - F5");
                                                "Run (Default Speed) - F5",
                                                "Run (Step) - F6",
                                                "XReload source - Home",
                                                "Main Menu - Esc",
                                                "Set IP Delta - Shift + Arrow Key"
                                                };
                        _content.AddRange(contentArr);
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
            throw new NotImplementedException();
        }
    }
}