using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BefungeSharp.UI;

namespace BefungeSharp
{
    public class WindowUI
    {
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

        private Selection _selection;
        
        public WindowUI(BoardInterpreter interp)
        {
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

            _selection.content = new List<string>();
            _selection.dimensions.Top = _selection.dimensions.Right = _selection.dimensions.Bottom = _selection.dimensions.Left = -1;
            _selection.active = false;
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
            for (int row = 0; row < UI_RIGHT; row++)
            {
                ConEx.ConEx_Draw.InsertCharacter(' ', row, UI_RIGHT);
            }

            string bottom = new string(' ', UI_RIGHT);
            ConEx.ConEx_Draw.InsertString(bottom, UI_RIGHT, 0, false);

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
                    else
                    {
                        //TODO - Choose a different symbol
                        deltaRep = '?';
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
            if (_selection.active == false)
            {
                return;
            }

            //Draw selection
            for (int c = _selection.dimensions.Left; c <= _selection.dimensions.Right; c++)
            {
                for (int r = _selection.dimensions.Top; r <= _selection.dimensions.Bottom; r++)
                {
                    char prevChar = '\0';
                    prevChar = Program.BoardManager.GetCharacter(r,c);

                    if (prevChar != '\0')
                    {
                        ConsoleColor color = ConsoleColor.White;
                        if (prevChar >= ' ' || prevChar <= '~')
                        {
                            color = Instructions.InstructionManager.InstructionSet[prevChar].Color;
                        }

                        ConEx.ConEx_Draw.SetAttributes(r, c, color, ConsoleColor.DarkGreen);
                    }
                }
            }
        }
        
        
        public void Update(BoardMode mode, ConsoleKeyInfo[] keysHit)
        {
            

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
                    HandleModifiers(mode, keysHit);

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
                                if (ConEx.ConEx_Input.ShiftDown == true)
                                {
                                    UpdateSelection(k);
                                }
                                else
                                {
                                    //Clear if we used an arrow key without shift
                                    ClearSelection();
                                }
                                break;
                            case ConsoleKey.Delete:
                                if (_selection.content.Count == 0)
                                {
                                    bool success = Program.BoardManager.PutCharacter(_interpRef.EditIP.Position.y, _interpRef.EditIP.Position.x, ' ');
                                }
                                else
                                {
                                    DeleteSelection();
                                    ClearSelection();
                                }
                                break;
                            case ConsoleKey.Backspace:
                                {
                                    Vector2 old = _interpRef.EditIP.Delta;
                                    Vector2 nVec = _interpRef.EditIP.Delta;
                                    nVec.Negate();
                                    _interpRef.EditIP.Delta = nVec;
                                    _interpRef.EditIP.Move();
                                    bool success = Program.BoardManager.PutCharacter(_interpRef.EditIP.Position.y, _interpRef.EditIP.Position.x, ' ');
                                    _interpRef.EditIP.Delta = old;
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
        /// Handles all keyboard input which involes Shift, Alt, or Control
        /// </summary>
        /// <param name="mode">The mode of the program you wish to conisder</param>
        /// <param name="keysHit">an array of keys hit</param>
        private void HandleModifiers(BoardMode mode, ConsoleKeyInfo[] keysHit)
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
             * Ctrl + C - Copy
            *  Ctrl + X - Cut 
            *  Ctrl + V - Paste
            *  XCtrl + A - Select the whole board?
            *  XCtrl + Z - Undo, a planned feature
            *  XCtrl + Y - Redo, also a planned feature
            *  Ctrl + S - Save
            *  
            * */
            bool x = ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_X);
            if (x && control)
            {
                this._selection.content = GetSelectionContents();
                ClipboardTools.ToWindowsClipboard(this._selection);
                DeleteSelection();
                System.Threading.Thread.Sleep(150);
            }

            bool c = ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_C);
            if (c && control)
            {
                this._selection.content = GetSelectionContents();
                ClipboardTools.ToWindowsClipboard(this._selection);
                //Emergancy sleep so we don't get a whole bunch of operations at once
                System.Threading.Thread.Sleep(150);
            }

            bool v = ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_V);
            if (v && control)
            {
                this._selection = ClipboardTools.FromWindowsClipboard(_interpRef.EditIP.Position);
                PutSelectionContents();
                //Emergancy sleep so we don't get a whole bunch of operations at once
                System.Threading.Thread.Sleep(150);
            }


            
        }
        /// <summary>
        /// Gets the contents of the selection box
        /// </summary>
        /// <returns>A list of strings, one for each row of the selection</returns>
        public List<string> GetSelectionContents()
        {
            List<string> outlines = new List<string>();

            for (int row = _selection.dimensions.Top; row <= _selection.dimensions.Bottom; row++)
            {
                string line = "";
                for (int column = _selection.dimensions.Left; column <= _selection.dimensions.Right; column++)
                {
                    line += Program.BoardManager.GetCharacter(row, column);
                }
                outlines.Add(line);
            }
            return outlines;
        }

        /*Record an insert
        F1-F4 is changes sidebar content
Super awesome text intro that is a befunge program
BefungeSharp for the logo*/
        private void PutSelectionContents()
        {
            int top = _selection.dimensions.Top;
            int left = _selection.dimensions.Left;

            //For the rows of the selection
            for (int s_row = 0; s_row < _selection.content.Count; s_row++)
            {
                //For every letter in each row
                for (int s_column = 0; s_column < _selection.content[s_row].Length; s_column++)
                {
                    //Put the character in the "real" location + the selection offset
                    Program.BoardManager.PutCharacter(top + s_row, left + s_column, _selection.content[s_row][s_column]);
                }
            }

            
            if (_interpRef.EditIP.Delta == Vector2.North || _interpRef.EditIP.Delta == Vector2.West)
            {
                _interpRef.EditIP.Move();
            }
            else if(_interpRef.EditIP.Delta == Vector2.East)
            {
                _interpRef.EditIP.Move((_selection.dimensions.Right-_selection.dimensions.Left));
            }
            else if(_interpRef.EditIP.Delta == Vector2.South)
            {
                _interpRef.EditIP.Move((_selection.dimensions.Bottom-_selection.dimensions.Top));
            }

        }
        
        private void UpdateSelection(ConsoleKey k)
        {
            //If the selection doesn't exist then we'll start by making a new one
            //Ensure that the selection is unintialized (Top is always > 0),
            //We aren't using the left or up arrow
            //and we are not currently in the middle of a selection
            if (_selection.dimensions.Top == -1 &&
                (k != ConsoleKey.LeftArrow || k != ConsoleKey.UpArrow) &&
                _selection.active == false)
            {

                //The selection origin is set to the IP's X and Y
                _selection.dimensions.Left = (short)_interpRef.EditIP.Position.x;
                _selection.dimensions.Top = (short)_interpRef.EditIP.Position.y;

                //The bottom is also set to the Y position
                _selection.dimensions.Bottom = (short)_interpRef.EditIP.Position.y;

                //To counter act if we are starting off moving down
                //we must account for the y position to be lower than normal
                //and that we are imediantly incrementing the bottom side
                if (k == ConsoleKey.DownArrow)
                {
                    _selection.dimensions.Bottom -= 1;
                    _selection.dimensions.Top -= 1;
                }

                _selection.dimensions.Right = (short)_interpRef.EditIP.Position.x;
                if (k == ConsoleKey.RightArrow)
                {
                    _selection.dimensions.Right -= 1;
                    _selection.dimensions.Left -= 1;
                }
                _selection.active = true;
            }

            //Finally get to the changing of the directions!
            if (k == ConsoleKey.UpArrow)
                _selection.dimensions.Bottom--;
            if (k == ConsoleKey.LeftArrow)
                _selection.dimensions.Right--;
            if (k == ConsoleKey.DownArrow)
                _selection.dimensions.Bottom++;
            if (k == ConsoleKey.RightArrow)
                _selection.dimensions.Right++;

            //Now we do a post check to see if we made a bad selection
            bool error_creating_selection = false;


            int x = _interpRef.EditIP.Position.x;
            int y = _interpRef.EditIP.Position.y;

            //Test if the IP has wrapped around behind itself or
            //Has walked behind itself
            error_creating_selection |= x < _selection.dimensions.Left;
            error_creating_selection |= _selection.dimensions.Right > 79;
            error_creating_selection |= y < _selection.dimensions.Top;
            error_creating_selection |= _selection.dimensions.Bottom > 24;

            if (error_creating_selection == true)
            {
                ClearSelection();
            }
            else
            {
                _selection.content = GetSelectionContents();
            }
        }

        private void DeleteSelection()
        {
            int top = _selection.dimensions.Top;
            int left = _selection.dimensions.Left;

            //For the rows of the selection
            for (int s_row = 0; s_row < _selection.content.Count; s_row++)
            {
                //For every letter in each row
                for (int s_column = 0; s_column < _selection.content[s_row].Length; s_column++)
                {
                    //Put the character in the "real" location + the selection offset
                    Program.BoardManager.PutCharacter(top + s_row, left + s_column, ' ');
                }
            }
        }
        private void ClearSelection()
        {
            _selection.content.Clear();
            _selection.dimensions = new ConEx.ConEx_Draw.SmallRect();
            _selection.dimensions.Bottom = -1;
            _selection.dimensions.Left = -1;
            _selection.dimensions.Right = -1;
            _selection.dimensions.Top = -1;
            _selection.active = false;
        }

       
    }
}
