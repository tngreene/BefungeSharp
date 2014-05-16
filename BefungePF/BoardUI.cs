using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    class BoardUI : BoardComponent
    {
        BoardManager boardRef;
        public const int UI_TOP = 26;

        public BoardUI(BoardManager mgr)
        {
            boardRef = mgr;
        }

        /// <summary>
        /// Draws the User interface of whatever mode the board is in
        /// </summary>
        /// <param name="mode">The mode of the board</param>
        public void Draw(BoardMode mode)
        {
            //Console.ForegroundColor = ConsoleColor.White;
            switch (mode)
            {
                case BoardMode.Run_MAX:
                case BoardMode.Run_FAST:
                case BoardMode.Run_MEDIUM:
                case BoardMode.Run_SLOW:

                case BoardMode.Run_STEP:
                Console.CursorTop = UI_TOP;
                Console.CursorLeft = 0;

                Console.WriteLine("Global Stack");
                Console.Write(' ');
                for (int i = boardRef.GlobalStack.Count - 1; i > 0; i-- )
                {
                    Console.Write("{0}|", boardRef.GlobalStack.ElementAt(i));
                }
                Console.Write('\n');
                for (int i = boardRef.GlobalStack.Count - 1; i > 0; i--)
                {
                    //Charecter At
                    char inCharFormat = (char)boardRef.GlobalStack.ElementAt(i);
                    if (Char.IsLetterOrDigit(inCharFormat) == true)
                    {
                        Console.Write(" {0}|", inCharFormat);
                    }
                    else
                    {
                        Console.Write("  |");
                    }
                }
                break;

                case BoardMode.Edit:
                    Console.CursorTop = UI_TOP;
                    Console.CursorLeft = 0;
                   
                    Console.WriteLine("New File - Ctrl-N  | Save - Ctrl-S | Run (Medium) - F5 | Main Menu - Escape");
                    break;
            }
        }

        public void ClearArea(BoardMode mode)
        {
            Console.SetCursorPosition(0, UI_TOP);
            for (int i = 0; i < Console.BufferHeight; i++)
            {
                for (int j = 0; j < Console.BufferWidth; j++)
                {
                    Console.Write(' ');
                }
            }
        }

        public void Update(BoardMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
