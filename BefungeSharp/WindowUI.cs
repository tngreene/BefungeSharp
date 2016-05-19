using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BefungeSharp.UI;
using BefungeSharp.FungeSpace;

namespace BefungeSharp
{
    public class WindowUI
    {
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
        const   int UI_TOP = 26;
        const   int UI_RIGHT = 80;
        private int UI_BOTTOM;

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

        private Queue<string> _outputRep;
        private int _outputRow;

        private Queue<string> _inputRep;
        private int _inputRow;

        private Selection _selection;

        /// <summary>
        /// Returns true if the selection has an area > 0
        /// </summary>
        public bool SelectionActive { 
                                        get
                                        {
                                            FungeSpaceArea area = _selection.GenerateArea();
                                            return (area.left + area.top + area.right + area.bottom) > 0;
                                        }
                                    }
        public WindowUI()
        {
            //All of the rows follow after each other
            _TOSSstackRep = "TS:";
            _TOSSstackRow = UI_TOP;

            _SOSSstackRep = "SS:";
            _SOSSstackRow = -1;//Turn on when we implent stack stack/interpreter currently running in 98 mode//_TOSSstackRow + 1;

            _outputRep = new Queue<string>();
            
            _outputRow = _TOSSstackRow + 1;

            _inputRep = new Queue<string>();
            _inputRow = _outputRow + 1;

            UI_BOTTOM = ConEx.ConEx_Draw.Dimensions.height - 1;

            _selection.Clear();
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
                    if (Program.Interpreter.IPs[0].Stack.Count > 0)
                    {
                        //Insert a pipe bar inbetween every number
                        for (int i = Program.Interpreter.IPs[0].Stack.Count-1; i >= 0; i--)
                        {
                            _TOSSstackRep += Program.Interpreter.IPs[0].Stack.ElementAt(i).ToString() + '|';
                        }

                        //If the size of the stack's representation is more than our screen can handle, present
                        //the condensed version
                        if (_TOSSstackRep.Length > UI_RIGHT)
                        {
                            string prefix = "TS:...";
                            //Copy from the the end (Length - 1) to some characters back (full width) minus[sic] the length of the prefix
                            _TOSSstackRep = "TS:..." + _TOSSstackRep.Substring((_TOSSstackRep.Length - 1) - UI_RIGHT + prefix.Length);
                        }
                    }
                    ConEx.ConEx_Draw.InsertString(_TOSSstackRep, _TOSSstackRow, 0, false);

                    //Color the pipe bars inbetween something nice
                    int colorize_row = _TOSSstackRow;
                    string colorize = _TOSSstackRep;
                    ConsoleColor pipeColor = ConsoleColor.DarkGreen;

                    for (int j = 0; j < colorize.Length; j++)
                    {
                        if (colorize[j] == '|')
                        {
                            ConEx.ConEx_Draw.SetAttributes(colorize_row, j, pipeColor, ConsoleColor.Black);
                        }
                    }
#endregion
#region OUTPUT
                    string outputString = "O:";
                    foreach (var character in _outputRep)
                    {
                        outputString += character;
                    }
                    ConEx.ConEx_Draw.InsertString(outputString, _outputRow, 0, false);                    
#endregion
#region INPUT
                    string inputString = "I:";
                    foreach (var character in _inputRep)
                    {
                        inputString += character;
                    }
                    
                    ConEx.ConEx_Draw.InsertString(inputString, _inputRow, 0, false);
#endregion
                    
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
                    //_TOSSstackRep += text;
                    break;
                case Categories.SOSS:
                   // _SOSSstackRep += text;
                    break;
                case Categories.OUT:
                    if (Program.Interpreter.CurMode == BoardMode.Run_TERMINAL)
                    {
                        Console.Write(text);
                    }
                    _outputRep.Enqueue(text);
                    if (_outputRep.Count > UI_RIGHT - 1 - 2)//-1 so we're not on the edge, -2 for the "O:"
                    {
                        _outputRep.Dequeue();
                    }
                    break;
                case Categories.IN:
                    _inputRep.Enqueue(text);
                    if (_inputRep.Count > UI_RIGHT - 1 - 2)//-1 so we're not on the edge, -2 for the "I:"
                    {
                        _inputRep.Dequeue();
                    }
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

            ConEx.ConEx_Draw.FillArea(' ', UI_TOP, 0, UI_RIGHT, ConEx.ConEx_Draw.Dimensions.height);
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
            IP selectedIP = null;
            if (mode != BoardMode.Edit)
            {
                selectedIP = Program.Interpreter.IPs[0];
            }
            else
            {
                selectedIP = Program.Interpreter.EditIP;
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("Mode: ");
            switch (mode)
            {
                //All strings padded so their right side is all uniform
                case BoardMode.Run_MAX:
                    sb.Append("Max");
                    break;
                case BoardMode.Run_FAST:
                    sb.Append("Fast");
                    break;
                case BoardMode.Run_MEDIUM:
                    sb.Append("Medium");
                    break;
                case BoardMode.Run_SLOW:
                    sb.Append("Slow");
                    break;
                case BoardMode.Run_STEP:
                    sb.Append("Step");
                    break;
                case BoardMode.Edit:
                    sb.Append("Edit");
                    break;
            }
            sb.Append(" ");

            //Based on the IP's delta show it's delta as a unicode arrow/symbol
            if (selectedIP.Delta == Vector2.North)
            {
                sb.Append((char)9516);
            }
            else if (selectedIP.Delta == Vector2.East)
            {
                sb.Append((char)9508);
            }
            else if (selectedIP.Delta == Vector2.South)
            {
                sb.Append((char)9524);
            }
            else if (selectedIP.Delta == Vector2.West)
            {
                sb.Append((char)9500);
            }
            else
            {
                //TODO - Choose a different symbol
                sb.Append('?');
            }

            //Save where in the string our delta representative is
            int deltaRepIndex = sb.Length - 1;

            sb.Append(" ");
            
            //Append
            if (mode == BoardMode.Edit && SelectionActive == true)
            {
                sb.Append(_selection.handle);
            }
            else
            {
                sb.Append((Vector2)selectedIP.Position.Data);
            }

            ConEx.ConEx_Draw.InsertString(sb.ToString(), UI_BOTTOM, 0, false);
            
            for (int c = deltaRepIndex; c < sb.ToString().Length; c++)
            {
                ConEx.ConEx_Draw.SetAttributes(UI_BOTTOM, c, ConsoleColor.Cyan, ConsoleColor.Black);//Color should be the same as movement color    
            }
        }

        private void DrawSelection(BoardMode mode)
        {
            if (this.SelectionActive == false)
            {
                return;
            }
            FungeSpaceArea dimensions = _selection.GenerateArea();
            //Draw selection
            for (int c = dimensions.left; c <= dimensions.right; c++)
            {
                for (int r = dimensions.top; r <= dimensions.bottom; r++)
                {
                    int relative_row = r - Program.Interpreter.ViewScreen.top;
                    int relative_column = c - Program.Interpreter.ViewScreen.left;

                    //Ensures that we will never draw inside the UI or Sidebar Pane
                    if (relative_row < Program.Interpreter.ViewScreen.Height && relative_column < Program.Interpreter.ViewScreen.Width)
                    {
                        ConEx.ConEx_Draw.SetAttributes(relative_row,
                                                       relative_column,
                                                       ConEx.ConEx_Draw.GetForegroundColor(relative_row, relative_column),
                                                       ConsoleColor.DarkGreen);
                    }
                }
            }
        }

        public void Reset()
        {
            _TOSSstackRep = "";
            _outputRep.Clear();
            _inputRep.Clear();
        }

		public void Update(BoardMode mode, IEnumerable<ConsoleKeyInfo> keysHit)
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
                    bool keep_selection_active = false;
                    for (int i = 0; i < keysHit.Count(); i++)
                    {
                        //--Debugging key presses
                        //System.ConsoleKey k = keysHit.ElementAt(i).Key;
                        //var m = keysHit.ElementAt(i).Modifiers;
                        //------------------------
                       
                        switch (keysHit.ElementAt(i).Key)
                        {
                            case ConsoleKey.UpArrow:
                            case ConsoleKey.LeftArrow:
                            case ConsoleKey.DownArrow:
                            case ConsoleKey.RightArrow:
                                //If we are editing the selection
                                if (ConEx.ConEx_Input.ShiftDown == true)
                                {
                                    //If we are starting a new selection
                                    if (this.SelectionActive == false)
                                    {
                                        //Set everything to the cell we are currently in
                                        _selection.origin = _selection.handle = Program.Interpreter.EditIP.Position.Data;
                                    }
									UpdateSelection(keysHit.ElementAt(i).Key);
                                
                                    //Don't clear if we used an arrow key with shift
                                    keep_selection_active = true;
                                }
                                break;
                            case ConsoleKey.Delete:
                                if (this.SelectionActive == true)
                                {
                                    DeleteSelection();
                                    keep_selection_active = true;
                                }                                
                                break;
                           
                            default:
                                //Explicitly say that any other keystroke will clear the selection
                                keep_selection_active = false;
                                break;
                        }
                        if (keep_selection_active == false)
                        {
                            _selection.Clear();
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
            *  Ctrl + X - Cut
            *  Ctrl + C - Copy
            *  Ctrl + V - Paste
            *  XCtrl + A - Select the whole board?
            *  XCtrl + Z - Undo, a planned feature
            *  XCtrl + Y - Redo, also a planned feature
            *  Alt + S - Save
            */
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
                this._selection = ClipboardTools.FromWindowsClipboard(Program.Interpreter.EditIP.Position.Data);
                PutSelectionContents();
                //Emergancy sleep so we don't get a whole bunch of operations at once
                System.Threading.Thread.Sleep(150);
            }

            if (mode == BoardMode.Edit)
            {
                //Resets the board back to its original state
                bool r = ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_R);
                if (r && alt)
                {
                    List<List<int>> originalSource = FileUtils.ReadFile(FileUtils.LastUserOpenedPath, false, true, true);
                    if (originalSource == null)
                    {
                        originalSource = new List<List<int>>();
                    }
                    Program.Interpreter = new Interpreter(originalSource);
                }
            }
            //TODO:FEATURE? What does Select All mean to us in text editor mode? Do we want this?
            /*bool a = ConEx.ConEx_Input.IsKeyPressed(ConEx.ConEx_Input.VK_Code.VK_A);
            if (a && control)
            {
                this._selection.dimensions = FungeSpaceUtils.GetWorldBounds(Program.Interpreter.FungeSpace);
                this._selection.content = this.GetSelectionContents();
                created_selection = true;
            }*/
            return;
        }

#region Selection
        /// <summary>
        /// Gets the contents of the selection box
        /// </summary>
        /// <returns>A list of strings, one for each row of the selection</returns>
        public List<string> GetSelectionContents()
        {
            Vector2[] cropping_bounds = new Vector2[2];

            FungeSpaceArea dimensions = _selection.GenerateArea();
            cropping_bounds[0] = new Vector2(dimensions.left, dimensions.top);
            cropping_bounds[1] = new Vector2(dimensions.right, dimensions.bottom);
 
            List<string> outlines = FungeSpaceUtils.MatrixToStringList(Program.Interpreter.FungeSpace, cropping_bounds);
         
            return outlines;
        }

        /// <summary>
        /// Puts the contents of the selection box into the world
        /// </summary>
        private void PutSelectionContents()
        {
            FungeSpaceArea dimensions = _selection.GenerateArea();
            int top = dimensions.top;
            int left = dimensions.left;

            //For the rows of the selection
            for (int s_row = 0; s_row < _selection.content.Count; s_row++)
            {
                //For every letter in each row
                for (int s_column = 0; s_column < _selection.content[s_row].Length; s_column++)
                {
                    //Put the character in the "real" location + the selection offset
                    Program.Interpreter.FungeSpace.InsertCell(new FungeCell(left + s_column, top + s_row, _selection.content[s_row][s_column]));
                }
            }

            Vector2 moveDelta = Program.Interpreter.EditIP.Delta;
            FungeNode currentPosition = Program.Interpreter.EditIP.Position;
            
            if (Program.Interpreter.EditIP.Delta == Vector2.North)
            {
                Program.Interpreter.EditIP.Position = FungeSpaceUtils.MoveTo(currentPosition, currentPosition.Data.y - (dimensions.Height), currentPosition.Data.x);
            }
            else if(Program.Interpreter.EditIP.Delta == Vector2.East)
            {
                Program.Interpreter.EditIP.Position = FungeSpaceUtils.MoveTo(currentPosition, currentPosition.Data.y, currentPosition.Data.x + (dimensions.Width));
            }
            else if(Program.Interpreter.EditIP.Delta == Vector2.South)
            {
                Program.Interpreter.EditIP.Position = FungeSpaceUtils.MoveTo(currentPosition, currentPosition.Data.y + (dimensions.Height), currentPosition.Data.x);
            }
            else if(Program.Interpreter.EditIP.Delta == Vector2.West)
            {
                Program.Interpreter.EditIP.Position = FungeSpaceUtils.MoveTo(currentPosition, currentPosition.Data.y, currentPosition.Data.x - (dimensions.Width));
            }
            
            _selection.Clear();
        }
        
        /// <summary>
        /// Update the selection bounds based on 
        /// </summary>
        /// <param name="k"></param>
        private void UpdateSelection(ConsoleKey k)
        {
            //Finally get to the changing of the directions!
            if (k == ConsoleKey.UpArrow)
                _selection.handle.y--;
            if (k == ConsoleKey.LeftArrow)
                _selection.handle.x--;
            if (k == ConsoleKey.DownArrow)
                _selection.handle.y++;
            if (k == ConsoleKey.RightArrow)
                _selection.handle.x++;

            //If the selection is bigger than the screen will hold move the view screen
            if (_selection.handle.y < Program.Interpreter.ViewScreen.top)
            {
                Program.Interpreter.MoveViewScreen(Vector2.North);
            }
            if (_selection.handle.x < Program.Interpreter.ViewScreen.left)
            {
                Program.Interpreter.MoveViewScreen(Vector2.West);
            }
            if (_selection.handle.y > Program.Interpreter.ViewScreen.bottom)
            {
                Program.Interpreter.MoveViewScreen(Vector2.South);
            }
            if (_selection.handle.x > Program.Interpreter.ViewScreen.right)
            {
                Program.Interpreter.MoveViewScreen(Vector2.East);
            }
        }
        
        private void DeleteSelection()
        {
            Program.Interpreter.FungeSpace.EnumerationArea = _selection.GenerateArea();
            foreach (var node in Program.Interpreter.FungeSpace)
            {
                node.Data = new FungeCell(node.Data.x, node.Data.y, ' ');
            }
        }
#endregion Selection

        public char GetCharacter()
        {
            if (Program.Interpreter.CurMode != BoardMode.Run_TERMINAL)
            {
                Console.SetCursorPosition(0, _inputRow + 1);
                Console.CursorVisible = true;
            }
            char input = '\0';
            do
            {
                input = Console.ReadKey(true).KeyChar;
            }
            while (input < ' ' || input > '~');

            AddText(input.ToString(), WindowUI.Categories.IN);
            Console.CursorVisible = false;
            return input;
        }

        public int GetDecimal()
        {
            if (Program.Interpreter.CurMode != BoardMode.Run_TERMINAL)
            {
                Console.SetCursorPosition(0, _inputRow + 1);
                Console.CursorVisible = true;
            }
            string input = Console.ReadLine();
            Console.CursorVisible = false;
            int outResult = 0;
            bool succeded = int.TryParse(input, out outResult);
            if (succeded == true)
            {
                AddText(input, WindowUI.Categories.IN);
                return outResult;
            }
            else
            {
                AddText("0", WindowUI.Categories.IN);
                return 0;
            }
        }
    }
}
