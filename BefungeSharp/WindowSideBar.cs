using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConEx;

namespace BefungeSharp
{
    public class WindowSideBar
    {
        /* The side bar extends from the right side of the board to a location W
         * | [0,81]                 [0,C_Width]
         * |
         * |
         * |
         * |
         * | [C_Height,0]                 [C_Height,C_Width]
         * */
        private const int BAR_TOP = 0;
        private const int BAR_LEFT = 81;
        private int BAR_RIGHT;
        private int BAR_BOTTOM;

        /// <summary>
        /// A collection of pages of content,
        /// where each page is a list of strings.
        /// 
        /// 0 - Keyboard shortcuts
        /// 1 - ASCII table
        /// </summary>
        private List<List<string>> _pages;
        
        /// <summary>
        /// The current page we're on
        /// </summary>
        private int _pageIndex;
          
        private readonly string[] ASCII_TABLE;

        public WindowSideBar()
        {
            BAR_RIGHT = ConEx.ConEx_Draw.Dimensions.width - 1;
            BAR_BOTTOM = ConEx.ConEx_Draw.Dimensions.height - 1;

            _pageIndex = 0;
            _pages = new List<List<string>>();

            //Create Page 1 (the current keyboard shortcuts)
            _pages.Add(new List<string>());

            ASCII_TABLE = new string[] {
                                        "╔══════════════════════════════════╗",
                                        "║           ASCII Table            ║",
                                        "╠═══════╦═══════╦════════╦═════════╣",
                                        "║ 32    ║ 58  : ║ 82   T ║ 111  o  ║",
                                        "║ 33  ! ║ 59  ; ║ 83   U ║ 112  p  ║",
                                        "║ 34  \" ║ 60  < ║ 84   V ║ 113  q  ║",
                                        "║ 35  # ║ 61  = ║ 88   W ║ 114  r  ║",
                                        "║ 36  $ ║ 62  > ║ 89   X ║ 115  s  ║",
                                        "║ 37  % ║ 63  ? ║ 90   Y ║ 116  t  ║",
                                        "║ 38  & ║ 64  @ ║ 91   Z ║ 117  u  ║",
                                        "║ 39  ' ║ 65  A ║ 92   [ ║ 118  v  ║",
                                        "║ 40  ( ║ 66  B ║ 93   \\ ║ 119  w  ║",
                                        "║ 41  ) ║ 67  C ║ 94   ] ║ 120  x  ║",
                                        "║ 42  * ║ 68  D ║ 95   _ ║ 121  y  ║",
                                        "║ 43  + ║ 69  E ║ 96   ` ║ 122  z  ║",
                                        "║ 44  , ║ 70  F ║ 97   a ║ 123  {  ║",
                                        "║ 45  - ║ 71  G ║ 98   b ║ 124  |  ║",
                                        "║ 46  . ║ 72  H ║ 99   c ║ 125  }  ║",
                                        "║ 47  / ║ 73  I ║ 100  d ║ 126  ~  ║",
                                        "║ 48  0 ║ 74  J ║ 101  e ║         ║",
                                        "║ 49  1 ║ 75  K ║ 102  f ║         ║",
                                        "║ 50  2 ║ 76  L ║ 103  g ║         ║",
                                        "║ 51  3 ║ 77  M ║ 104  h ║         ║",
                                        "║ 52  4 ║ 78  N ║ 105  i ║         ║",
                                        "║ 53  5 ║ 79  O ║ 106  j ║         ║",
                                        "║ 54  6 ║ 80  P ║ 107  k ║         ║",
                                        "║ 55  7 ║ 81  Q ║ 108  l ║         ║",
                                        "║ 56  8 ║ 82  R ║ 109  m ║         ║",
                                        "║ 57  9 ║ 83  S ║ 110  n ║         ║",
                                        "╚═══════╩═══════╩════════╩═════════╝"
                                       };
            //Create Page 2 (the ASCII table)
            _pages.Add(new List<string>(ASCII_TABLE));
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
            List<string> content = new List<string>();
            string[] commandContent = {
                                        "╔══════════════════════════════════╗",
                                        "║        Keyboard Shortcuts        ║",
                                        "╠══════════════════════════════════╣"
                                        };
            content.AddRange(commandContent);
            switch (mode)
            {
                case BoardMode.Run_MAX:
                case BoardMode.Run_FAST:
                case BoardMode.Run_MEDIUM:
                case BoardMode.Run_SLOW:
                case BoardMode.Run_STEP:
                    {
                        string stepInstructions = mode == BoardMode.Run_STEP ?
                                              "║Next Tick - Right arrow           ║" : 
                                              "║                                  ║";
                        string[] contentArr = {
                                              "║Select Speed - F1 - F6            ║",
                                              "║Back to Edit Mode - F12           ║",
                                              stepInstructions,
                                              "╚══════════════════════════════════╝",
                                              };
                        content.AddRange(contentArr);
                        _pages[0] = content;
                    }
                    break;
                case BoardMode.Edit:
                    {
                        //We'll only build up the command list
                        //if we'll be showing it
                        if (_pageIndex != 0)
                        {
                            break;
                        }
                        if (Program.WindowUI.SelectionActive == false)
                        {
                            string[] contentArr = {
                                                    //X indicates the feature has not been implemented
                                                    "║New File - Ctrl + N               ║",
                                                    "║Save - Alt + S                    ║",
                                                    "║Reload Source - Alt + R           ║",
                                                    "║Run (Step) - F1                   ║",
                                                    "║Run (Real Time) - F2 - F5         ║",
                                                    "║Run (Terminal Mode) - F6          ║",
                                                    "║Main Menu - Esc                   ║",
                                                    "║Insert Snippet - Insert           ║",
                                                    "║Set IP Delta - Ctrl + Arrow Key   ║",
                                                    "║Select - Shift (hold) + Arrow Key ║",
                                                    //"║XSelect All - Ctrl + A           ║",
                                                    "╚══════════════════════════════════╝"
                                                  };

                            content.AddRange(contentArr);
                        }
                        else
                        {
                            string[] contentArr = {
                                                    "║Adjust Selection Box - Arrow Keys ║",
                                                    "║Copy Section - Ctrl + C           ║",
                                                    "║Cut Section - Ctrl + X            ║",
                                                    "║Paste Section - Ctrl + V          ║",
                                                    "║Clear area - Delete               ║",
                                                    //"║XReverse line - Alt + R          ║",
                                                    "║Cancel Selection - Any Other Key  ║",
                                                    "╚══════════════════════════════════╝"
                                                   };
                            content.AddRange(contentArr);
                        }

                        _pages[0] = content;
                    }
                    break;
                default:
                    break;
            }
            
            for (int i = 0; i < _pages[_pageIndex].Count; i++)
            {
               ConEx.ConEx_Draw.InsertString(_pages[_pageIndex][i], BAR_TOP + i, BAR_LEFT, false);
            }

            if (_pageIndex == 1)
            {
                DrawASCII_Table();
            }
        }

        public void DrawASCII_Table()
        {
            //For the whole ASCII table diagram (skipping the first 3 lines)
            for(int row = 3; row < ASCII_TABLE.Length; row++)
	        {
                for(int column = 0; column < ASCII_TABLE[row].Length; column++)
	            {
                    char c = ASCII_TABLE[row][column];

                    //Only colorize the right ASCII characters (excluding space)
                    if (c > ' ' && c <= '~')
                    {
                        //Ignore all numbers except if the row and column are right
                        //6 comes from the hardcoded ASCII art table
                        if ((c < '0' || c > '9') || (column == 6))
                        {
                            ConEx_Draw.InsertCharacter(c,
                                                       row,
                                                       column + BAR_LEFT,
                                                       Instructions.InstructionManager.InstructionSet[c].Color);
                        }
                    }
	            }
	        }
        }

        public void ClearArea(BoardMode mode)
        {
            ConEx.ConEx_Draw.FillArea(' ', BAR_TOP, BAR_LEFT, ConEx.ConEx_Draw.Dimensions.width, ConEx.ConEx_Draw.Dimensions.height);
        }

        public void Update(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
            switch (mode)
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
                            //Cycle through the pages
                            case ConsoleKey.Home:
                                _pageIndex = _pageIndex > 0 ? _pageIndex -= 1 : _pageIndex = _pages.Count - 1;
                                break;
                            case ConsoleKey.End:
                                _pageIndex = _pageIndex < _pages.Count - 1 ? _pageIndex += 1 : _pageIndex = 0;
                                break;
                        }
                    }
                    break;
            }
                    #endregion
        }
    }
}