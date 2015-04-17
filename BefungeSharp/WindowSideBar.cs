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

        private int _pageIndex;
        private List<List<string>> _pages;

        public WindowSideBar(BoardManager mgr, Interpreter interp)
        {
            BAR_RIGHT = ConEx.ConEx_Draw.Dimensions.width - 1;
            BAR_BOTTOM = ConEx.ConEx_Draw.Dimensions.height - 1;
            _pageIndex = 0;
            _pages = new List<List<string>>();

            //Create Page 1
            _pages.Add(new List<string>());


            //Create Page 2
            _pages.Add(new List<string>());
            string[] asciiTableContent = { 
                                            "╔═════════════════════╗",
                                            "║     ASCII Table     ║",
                                            "╠════╦════╦═════╦═════╣",
                                            "║32  ║57 9║87  W║112 p║",
                                            "║33 !║58 :║88  X║113 q║",
                                            "║34 \"║59 ;║89  Y║114 r║",
                                            "║35 #║60 <║90  Z║115 s║",
                                            "║36 $║61 =║91  [║116 t║",
                                            "║37 %║62 >║92  \\║117 u║",
                                            "║38 &║63  ║93  ]║118 v║",
                                            "║39 '║69 E║94  ^║119 w║",
                                            "║40 (║70 F║95  _║120 x║",
                                            "║41 )║71 G║96  `║121 y║",
                                            "║42 *║72 H║97  a║123 z║",
                                            "║43 +║73 I║98  b║124 {║",
                                            "║44 ,║74 J║99  c║125 |║",
                                            "║45 -║75 K║100 d║126 }║",
                                            "║46 .║76 L║101 e║127 ~║",
                                            "║47 /║77 M║102 f║     ║",
                                            "║48 0║78 N║103 g║     ║",
                                            "║49 1║79 O║104 h║     ║",
                                            "║50 2║80 P║105 i║     ║",
                                            "║51 3║81 Q║106 j║     ║",
                                            "║52 4║82 R║107 k║     ║",
                                            "║53 5║83 S║108 l║     ║",
                                            "║54 6║84 T║109 m║     ║",
                                            "║55 7║85 U║110 n║     ║",
                                            "║56 8║86 V║111 o║     ║",
                                            "╚════╩════╩═════╩═════╝"
                                         };
            _pages[1].AddRange(asciiTableContent);
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
                                        "╔═════════════════════════════════╗",
                                        "║        Keyboard Shortcuts       ║",
                                        "╠═════════════════════════════════╣"
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
                                              "║Next Tick - Right arrow          ║" : "";
                        string[] contentArr = {
                                              "║Select Speed - F1 - F6           ║",
                                              "║Back to Edit Mode - F12          ║",
                                              stepInstructions,
                                              "╚═════════════════════════════════╝",
                                              };
                        content.AddRange(contentArr);
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
                                                    //TODO Organize these
                                                    //X indicates the feature has not been implemented
                                                    "║New File - Ctrl+N                ║",
                                                    "║Save - Alt+S                     ║",
                                                    "║Run (Step) - F1                  ║",
                                                    "║Run (Default Speed) - F5         ║",//TODO:OPTION["DefaultSpeed"]
                                                    "║Run (Terminal Mode) - F6         ║",
                                                    "║XReload source - Ctrl + R        ║",
                                                    "║Main Menu - Esc                  ║",
                                                    "║Set IP Delta - Ctrl + Arrow Key  ║",
                                                    "║XInsert Snippet - Insert         ║",
                                                    "║Start Selection - Shift (hold)   ║",
                                                    "║XSelect All - Ctrl + A           ║",
                                                    "╚═════════════════════════════════╝"
                                                  };

                            content.AddRange(contentArr);
                        }
                        else
                        {
                            string[] contentArr = {
                                                    "║Adjust Selection Box - Arrow Keys║",
                                                    "║Copy Section - Ctrl + C          ║",
                                                    "║Cut Section - Ctrl + X           ║",
                                                    "║Paste Section - Ctrl + V         ║",
                                                    "║Clear area - Delete              ║",
                                                    "║Reverse line - Alt + F4          ║",
                                                    "║Cancel Selection - Any Other Key ║",
                                                    "╚═════════════════════════════════╝"
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