using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.FungeSpace
{
    /// <summary>
    /// FungeSpace is a utility class for finding out information about and changing the SparseMatrix
    /// </summary>
    public static class FungeSpaceUtils
    {
        /// <summary>
        /// Get's a matrix upper and lower bounds
        /// </summary>
        /// <param name="matrix">The matrix to query</param>
        /// <returns>A vector where [0] is the upper left and [1] is the bottom right </returns>
        public static Vector2[] GetMatrixBounds(FungeSparseMatrix matrix)
        {
            //Where bounds[0] is the upper left bound and bounds[1] is the bottom right
            Vector2[] bounds = new Vector2[2];
            int left = 0;
            int right = 0;
            int top = 0;
            int bottom = 0;
            FungeNode traverse = matrix.Origin;


            //For every row
            do
            {
                FungeNode columnsStart = traverse;
                //For ever column
                do
                {
                    //If the current x is greater than our greatest score
                    if (traverse.Data.x > right)
                    {
                        //Update our highest right
                        right = traverse.Data.x;
                    }
                    if (traverse.Data.x < left)
                    {
                        left = traverse.Data.x;
                    }
                    traverse = traverse.East;
                }
                while(traverse != columnsStart);

                if (traverse.Data.y > bottom)
                {
                    bottom = traverse.Data.y;
                }
                if (traverse.Data.y < top)
                {
                    top = traverse.Data.y;
                }
                
                //Go to the next row down
                traverse = traverse.South;
            }
            while(traverse != matrix.Origin);
            
            //Assaign the top left corner
            bounds[0].x = left;
            bounds[0].y = top;

            bounds[1].x = right;
            bounds[1].y = bottom;

            return bounds;
        }

        public static void TestMatrix()
        {
            FungeNode traverse = null;
            if (false)
            {
                //Many columns, 1 row
                FungeSparseMatrix matrix1 = new FungeSparseMatrix(4, 4);
                matrix1.InsertCell(-1, 0, ' ');
                matrix1.InsertCell(-2, 0, ' ');
                matrix1.InsertCell(-5, 0, ' ');
                matrix1.InsertCell(-4, 0, ' ');
                matrix1.InsertCell(0, -1, ' ');
                matrix1.InsertCell(0, -2, ' ');
                matrix1.InsertCell(0, -5, ' ');
                matrix1.InsertCell(0, -4, ' ');
                matrix1.PrintFungeSpace();

                Vector2[] bounds = FungeSpaceUtils.GetMatrixBounds(matrix1);
                Console.WriteLine("TL: " + bounds[0] + "BR: " + bounds[1]);
                WalkOnMatrix(matrix1);

                FungeSparseMatrix matrix2 = new FungeSparseMatrix(); ;
                Console.WriteLine("----------------------------------------");
                matrix2.InsertCell(0, 5, 'a');
                matrix2.InsertCell(0, 6, 'b');
                matrix2.InsertCell(0, 4, 'c');
                matrix2.InsertCell(0, 1, 'd');
                matrix2.InsertCell(0, 2, 'e');
                matrix2.InsertCell(0, 3, 'f');
                matrix2.InsertCell(1, 0, 'g');
                matrix2.InsertCell(5, 0, 'h');
                matrix2.InsertCell(5, 3, 'i');
                matrix2.InsertCell(10, 0, 'j');
                matrix2.InsertCell(10, 6, 'k');

                matrix2.PrintFungeSpace();
                Vector2[] bounds2 = FungeSpaceUtils.GetMatrixBounds(matrix2);
                Console.WriteLine("TL: " + bounds2[0] + "BR: " + bounds2[1]);
                WalkOnMatrix(matrix2);
                matrix2.InsertCell(-1, 0, 'l');
                matrix2.InsertCell(-5, 0, 'm');
                matrix2.InsertCell(-3, 0, 'o');
                matrix2.InsertCell(-6, 0, 'p');
                matrix2.InsertCell(-1, 1 + 0, 'l');
                matrix2.InsertCell(-5, 1 + 0, 'm');
                matrix2.InsertCell(-3, 1 + 0, 'o');
                matrix2.InsertCell(-6, 1 + 0, 'p');
                matrix2.PrintFungeSpace();
                WalkOnMatrix(matrix2);

                bounds2 = FungeSpaceUtils.GetMatrixBounds(matrix2);
                Console.WriteLine("TL: " + bounds2[0] + "BR: " + bounds2[1]);
            }
            FungeSparseMatrix matrix3 = new FungeSparseMatrix();
            char v = 'a';
            while (true)
            {
                int x = 0;
                int y = 0;
                char value = v;
                try
                {
                    x = Convert.ToInt32(Console.ReadLine());
                    y = Convert.ToInt32(Console.ReadLine());
                    //value = Convert.ToChar(Console.ReadKey(true).KeyChar);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                    break;
                }

                FungeCell _cell = new FungeCell(x, y, value);
                matrix3.InsertCell(_cell);
                matrix3.PrintFungeSpace();

                Vector2[] bounds3 = FungeSpaceUtils.GetMatrixBounds(matrix3);
                Console.WriteLine("TL: " + bounds3[0] + "BR: " + bounds3[1]);
                v++;
            }

            WalkOnMatrix(matrix3);
        }

        private static void WalkOnMatrix(FungeSparseMatrix matrix)
        {
            FungeNode traverse = matrix.Origin;
            while (traverse != null)
            {
                char delta = Console.ReadKey(true).KeyChar;

                switch (delta)
                {
                    case 'i':
                        traverse = traverse.North;
                        break;
                    case 'l':
                        traverse = traverse.East;
                        break;
                    case 'k':
                        traverse = traverse.South;
                        break;
                    case 'j':
                        traverse = traverse.West;
                        break;
                    case 'q':
                        traverse = null;
                        break;
                    default:
                        break;
                }
                Console.WriteLine(traverse);
            }
        }
    }
}
