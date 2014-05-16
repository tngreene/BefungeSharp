using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    class BoardField
    {
        BoardManager boardRef;

        public BoardField(BoardManager mgr)
        {
            boardRef = mgr;
        }

        /// <summary>
        /// Draws the game field of whatever mode the board
        /// </summary>
        /// <param name="mode">The mode of the board</param>
        public void Draw(BoardMode mode)
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.White;

            //Create the boarder of the playing field
            for (int row = 0; row < boardRef.BoardArray.Count; row++)
            {
                Console.CursorLeft = boardRef.BoardArray[row].Count;
                Console.Write('|');
            }
            Console.CursorTop = boardRef.BoardArray.Count;
            Console.CursorLeft = 0;
            for (int column = 0; column < boardRef.BoardArray[0].Count - 1; column++)
            {
                Console.Write('_');
            }
        }

        public void ClearArea(BoardMode mode)
        {
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
