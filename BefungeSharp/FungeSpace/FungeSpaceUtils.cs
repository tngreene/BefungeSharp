using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.FungeSpace
{
    /// <summary>
    /// FungeSpace is a utility class for finding out information about and changing the SparseMatrix and/or FungeNodes
    /// </summary>
    public static class FungeSpaceUtils
    {
        /// <summary>
        /// Converts a DynamicArray, such as a List of Lists of chars
        /// </summary>
        /// <param name="matrix">The matrix to affect</param>
        /// <param name="dynamic_array_representation">The dynamic array source</param>
        public static void DynamicArrayToMatrix(FungeSparseMatrix matrix, List<List<int>> dynamic_array_representation)
        {
            for (int r = 0; r < dynamic_array_representation.Count; r++)
            {
                for (int c = 0; c < dynamic_array_representation[r].Count; c++)
                {
                    matrix.InsertCell(c,r, dynamic_array_representation[r][c]);
                }
            }
        }

        /// <summary>
        /// Gets the dynamic array version of a funge sparse matrix, where the holes are "filled in" with ' ''s
        /// </summary>
        /// <param name="matrix">The matrix to copy from</param>
        /// <param name="cropping_bounds">The bounds to copy, where cropping_bounds[0] is the top left and [1] is the bottom right.
        /// Warning: Negative nodes will cause the whole dynamic array representation to lose 1:1 translation from matrix form</param>
        /// <returns>The DynamicArray containing the matrix data</returns>
        public static List<List<int>> MatrixToDynamicArray(FungeSparseMatrix matrix, Vector2 [] cropping_bounds)
        {
            //Create a filled subsection of our matrix
            FungeSparseMatrix filled_matrix = FungeSpaceUtils.FillMatrix(matrix, cropping_bounds[0], cropping_bounds[1]);
            List<List<int>> outArray = new List<List<int>>();
            
            //Starting at the top
            int row = cropping_bounds[0].y;
            int column = cropping_bounds[0].x;
            
            //Find out if the place we want to start exists
            FungeNode traverse = matrix.GetNode(row, column);

            FungeNode f = filled_matrix.Origin;

            FungeNode rowsStart = f;
            do
            {
                FungeNode columnsStart = f;
                outArray.Add(new List<int>());
                do
                {
                    outArray.Last().Add(f.Data.value);
                    f = f.East;
                }
                while (f != columnsStart);
                
                f = f.South;
            }
            while (f != rowsStart);

            return outArray;
        }
        
        /// <summary>
        /// Fills the matrix's empty spaces with a certain value
        /// </summary>
        /// <param name="original_matrix">The original_matrix, it will not be altered</param>
        /// <param name="value">The value to be inserted into every cell, by default ' '</param>
        /// <returns>The filled matrix</returns>
        public static FungeSparseMatrix FillMatrix(FungeSparseMatrix original_matrix, Vector2 top_left, Vector2 bottom_right, int value = ' ')
        {
            //Copy the matrix over
            FungeSparseMatrix outMatrix = new FungeSparseMatrix(original_matrix);

            for (int y = top_left.y; y != bottom_right.y + 1; y++)
            {
                for ( int x = top_left.x; x != bottom_right.x + 1; x++)
                {
                    FungeNode lookup = outMatrix.GetNode(y, x);
                    if (lookup == null)
                    {
                        outMatrix.InsertCell(x, y, ' ');
                    }
                }
            }
            return outMatrix;
        }
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

        public static void ChangeData(FungeNode node, int new_value)
        {
            node.Data = new FungeCell(node.Data.x, node.Data.y, new_value);
        }

        /// <summary>
        /// Move's an object's position node to a specific place, creating a node there if need be
        /// </summary>
        /// <param name="position">The position node of an object</param>
        /// <param name="matrix">The matrix in which the FungeNode lives</param>
        /// <param name="row">The intented row to go to</param>
        /// <param name="column">The intended column to go to</param>
        /// <return>The new position of the FungeNode</return>
        public static FungeNode MoveTo(FungeNode position, int row, int column)
        {
            FungeNode traverse = position.ParentMatrix.GetNode(row, column);
            if (traverse == null)
            {
                position = position.ParentMatrix.InsertCell(column, row, ' ');
            }
            else
            {
                position = traverse;
            }
            return position;
        }

        /// <summary>
        /// MoveBy is an IP's move and wrap function
        /// </summary>
        /// <param name="position">The start position of the object</param>
        /// <param name="delta">The delta with which to move it</param>
        /// <return>The new position of the FungeNode</return>
        public static FungeNode MoveBy(FungeNode position, Vector2 delta)
        {
            //If we are traveling one of the "easy" directions
            if (delta.x == 0 || delta.y == 0)
            {
                //Attempt to move |x| positions
                for (int x = Math.Abs(delta.x); x > 0; x--)
			    {
                    //If we are moving in the positive (East) direction
			        if(delta.x > 0)
                    {
                        position = position.East;
                    }
                    else if(delta.x < 0)
                    {
                        position = position.West;
                    }
			    }

                //Attempt to move |y| positions
                for (int y = Math.Abs(delta.y); y > 0; y--)
			    {
                    //If we are moving in the positive (South) direction
			        if(delta.y > 0)
                    {
                        position = position.South;
                    }
                    else if(delta.y < 0)
                    {
                        position = position.North;
                    }
			    }
            }
            else
            {
                //If we have a complicated delta where we are moving in two directions it means 
                //we have to attempt to move along it until we've reached the matrix bounds.
                //If we still haven't found another non-empty spot at that point we'll need to

                Vector2 origin = new Vector2(position.Data.x,position.Data.y);
                Vector2 traverse = origin;
                 
                Vector2[] bounds = GetMatrixBounds(position.ParentMatrix);
                do
                {
                    //Test if our next move will actually be out of bounds
                    int nextX = traverse.x + (delta.x);
                    int nextY = traverse.y + (delta.y);

                    //If the next X is less than the least X OR greater than the most X
                    if(nextX <= bounds[0].x)
                    {
                        nextX = bounds[1].x + (nextX);   
                    }
                    else if(nextX >= bounds[1].x)
                    {
                        nextX = nextX - bounds[1].x;
                    }

                     //If the next Y is less than the least Y OR greater than the most Y
                    if(nextY <= bounds[0].y)
                    {
                        nextY = bounds[1].y + (nextY);   
                    }
                    else if(nextY >= bounds[1].y)
                    {
                        nextY = nextY - bounds[1].y;
                    }

                    //Attempt to get the next node at the next delta
                    FungeNode lookup = position.ParentMatrix.GetNode(nextY, nextX);

                    //If we finally found something
                    if (lookup != null)
                    {
                        if (lookup.Data.value != ' ')
                        {
                            position = lookup;
                            return position;
                        }
                    }

                    //Insert and travel to it
                    position = position.ParentMatrix.InsertCell(new FungeCell(nextX, nextY, ' '));
                    traverse = position.Data;
                }
                while (traverse != origin);
            }
            return position;
        }

        public static void DrawFungeSpace(FungeNode draw_origin)
        {
            FungeNode traverse = draw_origin;
            
            //For every row
            do
            {
                FungeNode columnsStart = traverse;
                //For ever column
                do
                {
                    ConsoleColor color = Instructions.InstructionManager.InstructionSet[traverse.Data.value].Color;
                   
                    char character = (char)traverse.Data.value;
                    ConEx.ConEx_Draw.InsertCharacter(character, traverse.Data.y, traverse.Data.x, color, ConsoleColor.Black);

                    traverse = traverse.East;
                }
                while(traverse != columnsStart);

                //Go to the next row down
                traverse = traverse.South;
            }
            while(traverse != draw_origin);
        }

        public static void TestMatrix()
        {
            FungeNode traverse = null;
            if (true)
            {
                //Many columns, 1 row
                FungeSparseMatrix matrix1 = new FungeSparseMatrix(10, 10);
                while (true) { }
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
