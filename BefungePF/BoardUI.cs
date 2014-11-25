using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    class BoardUI
    {
        private BoardManager _boardRef;

        /* The board UI area extends from a (currently arbitray/hardcoded area) from row 26 to row 31 and columns 0 through 80
         * except for the space [31,80] which makes it go to a new line
         * 
         * -----------------------------|
         * [26,0]                 [26,80]
         * 
         * 
         * 
         * 
         * [31,0]                 [31,79]
         * */
        const int UI_TOP = 26;
        private string _TOSSstackRep;
        private string _outputRep;
        private string _inputRep;

        private List<string> outputList;
        public List<string> OutputList { get { return outputList; } }

        public BoardUI(BoardManager mgr)
        {
            _boardRef = mgr;
            outputList = new List<string>();
            outputList.Add("");
        }

        /// <summary>
        /// Draws the User interface of whatever mode the board is in
        /// </summary>
        /// <param name="mode">The mode of the board</param>
        public void Draw(BoardMode mode)
        {
            #region Draw Field
            //Create the boarder of the playing field
            for (int row = 0; row < _boardRef.BoardArray.Count; row++)
            {
                ConEx.ConEx_Draw.InsertCharacter('|', row, _boardRef.BoardArray[0].Count);
            }
            
            string bottom = new string('_', _boardRef.BoardArray[0].Count);
            ConEx.ConEx_Draw.InsertString(bottom, _boardRef.BoardArray.Count, 0, false);
            #endregion

            #region DrawCursorPosition
            //Clears off the previous thing
            ConEx.ConEx_Draw.InsertString("     ", ConEx.ConEx_Draw.Dimensions.height - 1, ConEx.ConEx_Draw.Dimensions.width - 1 - 5, false);
            
            string IP_Pos = Console.CursorTop + "," + Console.CursorLeft;
            ConEx.ConEx_Draw.InsertString(IP_Pos, ConEx.ConEx_Draw.Dimensions.height-1, ConEx.ConEx_Draw.Dimensions.width - 1 - IP_Pos.Length, false, ConsoleColor.Cyan);//Color should be the same as movement color
            #endregion

            switch (mode)
            {
                case BoardMode.Run_MAX:
                case BoardMode.Run_FAST:
                case BoardMode.Run_MEDIUM:
                case BoardMode.Run_SLOW:
                case BoardMode.Run_STEP:

                    string stackString = "S:";
                    for (int i = 0; i < _boardRef.GlobalStack.Count; i++)
			        {
                        stackString += _boardRef.GlobalStack.ElementAt(i).ToString();
                        stackString += '|';

                        //ConEx.ConEx_Draw.InsertCharacter('|', UI_TOP, i+1, ConsoleColor.DarkGreen);
                        //ConEx.ConEx_Draw.InsertCharacter(_boardRef.GlobalStack.ElementAt(i).ToString()[0], UI_TOP, j);
			        }

                    for (int i = 0; i < stackString.Length; i++)
                    {
                        ConsoleColor fore = ConsoleColor.White;
                        if(stackString[i] == '|')
                        {
                            fore = ConsoleColor.DarkGreen;
                        }
                        ConEx.ConEx_Draw.InsertCharacter(stackString[i], UI_TOP, i, fore);
                    }

                    if (this.outputList.Count > 0)
                    {
                        string outstring = "O:" + this.outputList[0];

                        for (int i = 0; i < outstring.Length; i++)
                        {
                            ConsoleColor fore = ConsoleColor.White;
                            if (outstring[i] == '|')
                            {
                                fore = ConsoleColor.DarkBlue;
                            }
                            ConEx.ConEx_Draw.InsertCharacter(outstring[i], UI_TOP + 1, i, fore);
                        }
                    }
                    break;
                case BoardMode.Edit:
                    ConEx.ConEx_Draw.InsertString("New File - Ctrl-N  | Save - Alt - S | Run (Medium) - F5 | Main Menu - Escape", UI_TOP,0,false);
                    
                    
                    break;
                default:
                    break;
            }
        }
            
            /*case BoardMode.Run_MAX:
            case BoardMode.Run_FAST:
            case BoardMode.Run_MEDIUM:
            case BoardMode.Run_SLOW:

            case BoardMode.Run_STEP:
            */
                
            //Console.Write("({0},{1})", editCursorL, editCursorT);


            /*    
            Console.Write("             Global Stack");
            Console.Write(' ');

            for (int i = boardRef.GlobalStack.Count - 1; i >= 0; i-- )
            {
                bool displayStackAsASCII = false;
                if (boardRef.GlobalStack.ElementAt(i) > 32 && boardRef.GlobalStack.ElementAt(i) < 127 && displayStackAsASCII == true )
                {
                    Console.Write("{0}|", (char)boardRef.GlobalStack.ElementAt(i));
                }
                else
                {
                    Console.Write("{0}|", boardRef.GlobalStack.ElementAt(i));
                }
            }
            Console.Write('\n');
            Console.Write("Output: ");
            for (int i = 0; i < outputList.Count; i++)
            {
                Console.Write(outputList[i]);
            }*/
        
        public void ClearArea(BoardMode mode)
        {
            //De-create the boarder of the playing field
            for (int row = 0; row < _boardRef.BoardArray.Count; row++)
            {
                ConEx.ConEx_Draw.InsertCharacter(' ', row, _boardRef.BoardArray[0].Count);
            }

            string bottom = new string(' ', _boardRef.BoardArray[0].Count);
            ConEx.ConEx_Draw.InsertString(bottom, _boardRef.BoardArray.Count, 0, false);

            ConEx.ConEx_Draw.FillArea('\0', UI_TOP, 0, ConEx.ConEx_Draw.Dimensions.width, ConEx.ConEx_Draw.Dimensions.height);
        }

        public void Update(BoardMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
