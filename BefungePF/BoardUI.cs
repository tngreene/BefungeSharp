using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    public class BoardUI
    {
        private BoardManager _boardRef;
        private BoardInterpreter _interpRef;
        /* The board UI area extends from a (currently arbitray/hardcoded area) from row 26 to row 31 and columns 0 through 80
         * except for the space [31,80] which makes it go to a new line
         * 
         * -----------------------------|
         * [26,0]                 [26,80]
         * 
         * 
         * 
         * 
         * [31,0]                 [31,80]
         * */
        const int UI_TOP = 26;
        const int UI_RIGHT = 80;
        int UI_BOTTOM = 0;

        public enum Categories
        {
            TOSS,
            SOSS,
            OUT,
            IN
        }
        //a string to represent the piece of information
        //and the row in which to start drawing it
        private string _TOSSstackRep;
        private int _TOSSstackRow;

        private string _SOSSstackRep;
        private int _SOSSstackRow;

        private string _outputRep;
        private int _outputRow;

        private string _inputRep;
        private int _inputRow;

        private ConEx.ConEx_Draw.SmallRect selection;
        private bool _selecting = false;

        public BoardUI(BoardManager mgr, BoardInterpreter interp)
        {
            _boardRef = mgr;
            _interpRef = interp;

            //All of the rows follow after each other
            _TOSSstackRep = "TS:";
            _TOSSstackRow = UI_TOP;

            _SOSSstackRep = "SS:";
            _SOSSstackRow = -1;//Turn on when we implent stack stack/interpreter currently running in 98 mode//_TOSSstackRow + 1;

            _outputRep = "O:";
            _outputRow = _TOSSstackRow + 1;

            _inputRep = "I:";
            _inputRow = _outputRow + 1;

            UI_BOTTOM = ConEx.ConEx_Draw.Dimensions.height - 1;

            selection.Top = selection.Right = selection.Bottom = selection.Left = -1;
            _selecting = false;
        }

        /// <summary>
        /// Draws the User interface of whatever mode the board is in
        /// </summary>
        /// <param name="mode">The mode of the board</param>
        public void Draw(BoardMode mode)
        {

            DrawField(mode);
            DrawInfo(mode);
            

            switch (mode)
            {
                case BoardMode.Run_MAX:
                case BoardMode.Run_FAST:
                case BoardMode.Run_MEDIUM:
                case BoardMode.Run_SLOW:
                case BoardMode.Run_STEP:
#region TOSS
                    _TOSSstackRep = "TS:";
                    List<int> rStack = _interpRef.IPs[0].Stack.ToList();
                    rStack.Reverse();

                    for (int i = 0; i < rStack.Count; i++)
                    {
                        _TOSSstackRep += rStack.ElementAt(i).ToString() + '|';
                    }
                    
                    for (int i = 0; i < _TOSSstackRep.Length; i++)
                    {
                        if(_TOSSstackRep[i] == '|')
                        {
                            ConsoleColor fore = ConsoleColor.DarkGreen;
                            ConEx.ConEx_Draw.SetAttributes(_TOSSstackRow, i, fore, ConsoleColor.Black);
                        }
                    }
#endregion
#region SOSS
                    if (_SOSSstackRow != -1)
                    {
                        /*_SOSSstackRep = "SS:";
                        List<int> rStack = _boardRef.GlobalStack.ToList();
                        rStack.Reverse();

                        for (int i = 0; i == rStack.Count - 1; i++)
                        {
                            _SOSSstackRep += rStack.ElementAt(i).ToString();
                            _SOSSstackRep += '|';
                        }
                        

                        for (int i = 0; i < _SOSSstackRep.Length; i++)
                        {
                            ConsoleColor fore = ConsoleColor.White;
                            if (_SOSSstackRep[i] == '|')
                            {
                                fore = ConsoleColor.DarkGreen;
                                ConEx.ConEx_Draw.SetAttributes(_SOSSstackRow, i, fore, ConsoleColor.Black);
                            }
                        }*/
                    }
#endregion    
                    //Insert the strings into the world
                    ConEx.ConEx_Draw.InsertString(_TOSSstackRep, _TOSSstackRow, 0, false);
                    //ConEx.ConEx_Draw.InsertString(_SOSSstackRep, _SOSSstackRow, 0, false);
                    ConEx.ConEx_Draw.InsertString(_outputRep, _outputRow, 0, false);
                    ConEx.ConEx_Draw.InsertString(_inputRep, _inputRow, 0, false);

                    //For all strings color the pipe bars
                    for (int i = 0; i < 4; i++)
                    {
                        string colorize = "";
                        ConsoleColor pipeColor = ConsoleColor.White;
                        switch (i)
                        {
                            case 0:
                                colorize = _TOSSstackRep;
                                pipeColor = ConsoleColor.DarkYellow;
                                break;
                            case 1:
                                colorize = _SOSSstackRep;
                                pipeColor = ConsoleColor.DarkMagenta;
                                break;
                            case 2:
                                colorize = _outputRep;
                                pipeColor = ConsoleColor.Gray;
                                break;
                            case 3:
                                colorize = _inputRep;
                                pipeColor = ConsoleColor.DarkGray;
                                break;
                        }

                        for (int j = 0; j < colorize.Length; j++)
                        {
                            if (colorize[j] == '|')
                            {
                                ConEx.ConEx_Draw.SetAttributes(_outputRow, j, pipeColor, ConsoleColor.Black);
                            }
                        }      
                    }
                    break;
                case BoardMode.Edit:
                    DrawSelection(mode);
                    break;
                default:
                    break;
            }
        }

        public void AddText(string text, Categories catagory)
        {
            switch (catagory)
            {
                case Categories.TOSS:
                    _TOSSstackRep += text;
                    break;
                case Categories.SOSS:
                    _SOSSstackRep += text;
                    break;
                case Categories.OUT:
                    _outputRep += text;
                    break;
                case Categories.IN:
                    _inputRep += text + "|";
                    break;
            }
        }

        public void ClearArea(BoardMode mode)
        {
            //De-create the boarder of the playing field
            for (int row = 0; row < _boardRef.BoardArray.Count; row++)
            {
                ConEx.ConEx_Draw.InsertCharacter(' ', row, _boardRef.BoardArray[0].Count);
            }

            string bottom = new string(' ', _boardRef.BoardArray[0].Count);
            ConEx.ConEx_Draw.InsertString(bottom, _boardRef.BoardArray.Count, 0, false);

            ConEx.ConEx_Draw.FillArea(' ', UI_TOP, 0, ConEx.ConEx_Draw.Dimensions.width, ConEx.ConEx_Draw.Dimensions.height);
        }

        /// <summary>
        /// Draws the border of the field which seperates the UI and the field
        /// </summary>
        /// <param name="mode"></param>
        private void DrawField(BoardMode mode)
        {
            //Create the boarder of the playing field
            for (int row = 0; row <= UI_BOTTOM; row++)
            {
                ConEx.ConEx_Draw.InsertCharacter('|', row, UI_RIGHT);
            }

            string bottom = new string('_', UI_RIGHT);
            ConEx.ConEx_Draw.InsertString(bottom, UI_TOP - 1, 0, false);
        }
        
        /// <summary>
        /// Draws the information about the current mode and IP_Position
        /// </summary>
        /// <param name="mode">Mode of the program</param>
        private void DrawInfo(BoardMode mode)
        {
            string modeStr = "Mode: ";
            char deltaRep = ' ';

            switch (mode)
            {
                //All strings padded so their right side is all uniform
                case BoardMode.Run_MAX:
                    modeStr += "Max";
                    break;
                case BoardMode.Run_FAST:
                    modeStr += "Fast";
                    break;
                case BoardMode.Run_MEDIUM:
                    modeStr += "Medium";
                    break;
                case BoardMode.Run_SLOW:
                    modeStr += "Slow";
                    break;
                case BoardMode.Run_STEP:
                    modeStr += "Step";
                    break;
                case BoardMode.Edit:
                    modeStr += "Edit";
                    
                    //Based on the direction of the IP set the delta rep to it
                    //This was the delta representative is only availble in Edit or edit like modes
                    if (_interpRef.EditIP.Delta == Vector2.North)
                    {
                        deltaRep = (char)9516;
                    }
                    else if (_interpRef.EditIP.Delta == Vector2.East)
                    {
                        deltaRep = (char)9508;
                    }
                    else if (_interpRef.EditIP.Delta == Vector2.South)
                    {
                        deltaRep = (char)9524;
                    }
                    else if (_interpRef.EditIP.Delta == Vector2.West)
                    {
                        deltaRep = (char)9500;
                    }
                    break;
            }

            
            
             
            //Generates a strings which is always five chars wide, with the number stuck to the ','
            //Like " 0,8 " , "17,5 " , "10,10", " 8,49"
            string IP_Pos = "";
            Vector2 vec_pos = _interpRef.IPs[0].Position;
            IP_Pos += vec_pos.x.ToString().Length == 1 ? ' ' : vec_pos.x.ToString()[0];
            IP_Pos += vec_pos.x.ToString().Length == 1 ? vec_pos.x.ToString()[0] : vec_pos.x.ToString()[1];
            IP_Pos += ',';
            IP_Pos += vec_pos.y.ToString().Length == 1 ? vec_pos.y.ToString()[0] : vec_pos.y.ToString()[0];
            IP_Pos += vec_pos.y.ToString().Length == 1 ? ' ' : vec_pos.y.ToString()[1];

            
            ConEx.ConEx_Draw.InsertCharacter(deltaRep, UI_BOTTOM, (UI_RIGHT - 1) - IP_Pos.Length - 1, ConsoleColor.Cyan);
            ConEx.ConEx_Draw.InsertString(IP_Pos, UI_BOTTOM, (UI_RIGHT - 1) - IP_Pos.Length, false);
            ConEx.ConEx_Draw.InsertString(modeStr, UI_BOTTOM, (UI_RIGHT - 1) - (IP_Pos.Length) - (1) - (12/*Maximum Possible Length for modeStr*/), false);
            

            for (int i = 0; i < IP_Pos.Length; i++)
            {
                int col = (UI_RIGHT - 1) - (IP_Pos.Length + i);
                ConEx.ConEx_Draw.SetAttributes(UI_BOTTOM, (UI_RIGHT - 1) - (IP_Pos.Length - i), ConsoleColor.Cyan, ConsoleColor.Black);//Color should be the same as movement color    
            }
        }

        private void DrawSelection(BoardMode mode)
        {
            //Fix the perminate 1 cell in [0,0] bug
            if (_selecting == false)
            {
                return;
            }

            //Draw selection
            for (int c = selection.Left; c <= selection.Right; c++)
            {
                for (int r = selection.Top; r <= selection.Bottom; r++)
                {
                    char prevChar = '\0';
                    prevChar = _boardRef.GetCharacter(r,c);

                    if (prevChar != '\0')
                    {
                        ConEx.ConEx_Draw.SetAttributes(r, c, BoardManager.LookupInfo(prevChar).color, ConsoleColor.DarkGreen);
                    }
                }
            }
        }
        
        
        public void Update(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
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
                    break;
                case BoardMode.Edit:
                    bool c = ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_C);
                    if (c && control)
                    {
                        List<string> contents = GetSelectionContents();
                    }
                    for (int i = 0; i < keysHit.Length; i++)
                    {
                        //--Debugging key presses
                        System.ConsoleKey k = keysHit[i].Key;
                        var m = keysHit[i].Modifiers;
                        //------------------------
                       
                        switch (keysHit[i].Key)
                        {
                            case ConsoleKey.UpArrow:
                            case ConsoleKey.LeftArrow:
                            case ConsoleKey.DownArrow:
                            case ConsoleKey.RightArrow:
                                //If we are editing the selection
                                if (shift == true)
                                {
                                    UpdateSelection(k);
                                }
                                else
                                {
                                    //Clear if we used an arrow key without shift
                                    ClearSelection();
                                }
                                break;
                            case ConsoleKey.C:
                                if (control == true)
                                {
                                    List<string> contents = GetSelectionContents();
                                }
                                break;
                            default:
                                //Clear if we pressed any other key possible
                                ClearSelection();
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the contents of the selection box
        /// </summary>
        /// <returns>A list of strings, one for each row of the selection</returns>
        public List<string> GetSelectionContents()
        {
            List<string> outlines = new List<string>();

            for (int row = selection.Top; row <= selection.Bottom; row++)
            {
                string line = "";
                for (int column = 0; column <= selection.Right; column++)
                {
                    line += _boardRef.GetCharacter(row, column);
                }
                outlines.Add(line);
            }
            return outlines;
        }

        private void SetSetelectionContents()
        {

        }
        
        private void UpdateSelection(ConsoleKey k)
        {
            //If the selection doesn't exist then we'll start by making a new one
            //Ensure that the selection is unintialized (Top is always > 0),
            //We aren't using the left or up arrow
            //and we are not currently in the middle of a selection
            if (selection.Top == -1 &&
                (k != ConsoleKey.LeftArrow || k != ConsoleKey.UpArrow) &&
                _selecting == false)
            {

                //The selection origin is set to the IP's X and Y
                selection.Left = (short)_interpRef.EditIP.Position.x;
                selection.Top = (short)_interpRef.EditIP.Position.y;

                //The bottom is also set to the Y position
                selection.Bottom = (short)_interpRef.EditIP.Position.y;

                //To counter act if we are starting off moving down
                //we must account for the y position to be lower than normal
                //and that we are imediantly incrementing the bottom side
                if (k == ConsoleKey.DownArrow)
                {
                    selection.Bottom -= 1;
                    selection.Top -= 1;
                }

                selection.Right = (short)_interpRef.EditIP.Position.x;
                if (k == ConsoleKey.RightArrow)
                {
                    selection.Right -= 1;
                    selection.Left -= 1;
                }
                _selecting = true;
            }

            //Finally get to the changing of the directions!
            if (k == ConsoleKey.UpArrow)
                selection.Bottom--;
            if (k == ConsoleKey.LeftArrow)
                selection.Right--;
            if (k == ConsoleKey.DownArrow)
                selection.Bottom++;
            if (k == ConsoleKey.RightArrow)
                selection.Right++;

            //Now we do a post check to see if we made a bad selection
            bool error_creating_selection = false;


            int x = _interpRef.EditIP.Position.x;
            int y = _interpRef.EditIP.Position.y;

            //Test if the IP has wrapped around behind itself or
            //Has walked behind itself
            error_creating_selection |= x < selection.Left;
            error_creating_selection |= selection.Right > 79;
            error_creating_selection |= y < selection.Top;
            error_creating_selection |= selection.Bottom > 24;

            if (error_creating_selection == true)
            {
                ClearSelection();
            }
        }
        
        private void ClearSelection()
        {
            selection = new ConEx.ConEx_Draw.SmallRect();
            selection.Bottom = -1;
            selection.Left = -1;
            selection.Right = -1;
            selection.Top = -1;
            _selecting = false;
        }
    }
}
