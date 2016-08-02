using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.FungeSpace
{
    /// <summary>
    /// The area of a FungeSpace, real, inuse, or simply conceptually
    /// </summary>
    public struct FungeSpaceArea
    {
        public int top;
        public int left;
        public int bottom;
        public int right;
        /// <summary>
        /// The width of the area (in number of cells, including spaces)
        /// </summary>
        public int Width { get { return (right - left) + 1; } }
        
        /// <summary>
        /// The height of the area (in number of cells, including spaces)
        /// </summary>
        public int Height { get { return (bottom - top) + 1; } }

        public FungeSpaceArea(Vector2 top_left, Vector2 bottom_right)
        {
            this.left = top_left.x;
            this.top = top_left.y;
            this.right = bottom_right.x;
            this.bottom = bottom_right.y;
        }

        public FungeSpaceArea(int top, int left, int bottom, int right)
        {
           this.left = left;
           this.top = top;

           this.bottom = bottom;
           this.right = right;
        }

        public bool Contains(int x, int y)
        {
            if (x >= left && x <= right)
            {
                if (y >= top && y <= bottom)
                {
                    return true;
                }
            }
            return false;
        }
    }
    /// <summary>
    /// FungeSpace is a utility class for finding out information about and changing the SparseMatrix and/or FungeNodes
    /// </summary>
    public static class FungeSpaceUtils
    {
        
        //We'll probably need this one day
        //public static void StringListToMatrix(FungeSparseMatrix matrix, List<string> string_list_representation)
        
        /// <summary>
        /// Converts a DynamicArray, such as a List of Lists of chars
        /// </summary>
        /// <param name="matrix">The matrix to affect</param>
        /// <param name="dynamic_array_representation">The dynamic array source</param>
        public static void DynamicArrayToMatrix(FungeSparseMatrix matrix, List<List<int>> dynamic_array_representation)
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            for (int r = 0; r < dynamic_array_representation.Count; r++)
            {
                for (int c = 0; c < dynamic_array_representation[r].Count; c++)
                {
                    matrix.InsertCell(c,r, dynamic_array_representation[r][c]);
                }
            }
            
            TimeSpan time = watch.Elapsed;
            Console.WriteLine(time);
            Console.ReadKey(true);
        }

        public static List<string> MatrixToStringList(FungeSparseMatrix matrix, Vector2[] cropping_bounds)
        {
            List<List<int>> dynm_arr = FungeSpace.FungeSpaceUtils.MatrixToDynamicArray(matrix, cropping_bounds);
            List<string> outlines =  new List<string>();
            
            for (int row = 0; row < dynm_arr.Count; row++)
            {
                string line = "";
                for (int column = 0; column < dynm_arr[row].Count; column++)
                {
                    line += (char)dynm_arr[row][column];
                }
                outlines.Add(line);
            }
            return outlines;
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
            FungeNode f = filled_matrix.GetNode(row, column);
            
            FungeSparseMatrixRowEnumerator rowEnumerator = new FungeSparseMatrixRowEnumerator(f,cropping_bounds[0].y,cropping_bounds[1].y);
            while(rowEnumerator.MoveNext())
            {
                FungeSparseMatrixColumnEnumerator colEnumerator = new FungeSparseMatrixColumnEnumerator(rowEnumerator.Current,cropping_bounds[0].x,cropping_bounds[1].x);

                outArray.Add(new List<int>());
                while (colEnumerator.MoveNext())
                {
                    outArray.Last().Add(colEnumerator.Current.Data.value);
                }
            }
            
            return outArray;
        }

        public static void OverlayMatrix(FungeSparseMatrix base_matrix, FungeSparseMatrix overlay_matrix, Vector2 overlay_start)
        {
            foreach (var node in overlay_matrix)
            {
                if (node.Data.value == ' ')
                {
                    continue;
                }

                FungeCell overlay_cell = node.Data;
                overlay_cell.x += overlay_start.x;
                overlay_cell.y += overlay_start.y;
                base_matrix.InsertCell(overlay_cell);
            }
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
            FungeSparseMatrix outMatrix = new FungeSparseMatrix();

            for (int y = top_left.y; y != bottom_right.y + 1; y++)
            {
                for ( int x = top_left.x; x != bottom_right.x + 1; x++)
                {
                    FungeNode lookup = original_matrix.GetNode(y, x);
                    if (lookup == null)
                    {
                        outMatrix.InsertCell(x, y, ' ');
                    }
                    else
                    {
                        outMatrix.InsertCell(lookup.Data.x, lookup.Data.y, lookup.Data.value);
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

            bounds[0].x = matrix.MatrixBounds.left;
            bounds[0].y = matrix.MatrixBounds.top;
            bounds[1].x = matrix.MatrixBounds.right;
            bounds[1].y = matrix.MatrixBounds.bottom;
            return bounds;
        }

        /// <summary>
        /// Gets the bounds of the world without the void
        /// </summary>
        /// <param name="matrix">The matrix to inspect</param>
        /// <returns></returns>
        public static Vector2[] GetRealWorldBounds(FungeSparseMatrix matrix)
        {
            //Where bounds[0] is the top left bound and bounds[1] is the bottom right
            Vector2[] bounds = new Vector2[2];

            FungeNode start = null;
            FungeSparseMatrixRowEnumerator rowEnumerator = new FungeSparseMatrixRowEnumerator(matrix.Origin);
            FungeSparseMatrixColumnEnumerator colEnumerator = null;
            while(rowEnumerator.MoveNext() && start == null)
            {
                colEnumerator= new FungeSparseMatrixColumnEnumerator(rowEnumerator.Current);
                while(colEnumerator.MoveNext() && start == null)
                {
                    if(colEnumerator.Current.Data.value != ' ')
                    {
                        start = colEnumerator.Current;
                    }
                }
            }

            //The bounds start off as the start position's
            bounds[0].x = start.Data.x;
            bounds[0].y = start.Data.y;
            bounds[1].x = start.Data.x;
            bounds[1].y = start.Data.y;

            //This node will always exist
            FungeNode rowStart = matrix.GetNode(start.Data.y, 0);
            //Start the row off on the spine
            rowEnumerator = new FungeSparseMatrixRowEnumerator(rowStart);
            colEnumerator = null;
            while (rowEnumerator.MoveNext())
            {
                //if this is our first time starting on the start
                if (colEnumerator == null)
                {
                    //Start the column on the actual "start"
                    colEnumerator = new FungeSparseMatrixColumnEnumerator(start);
                }
                else
                {
                    colEnumerator = new FungeSparseMatrixColumnEnumerator(rowEnumerator.Current);
                }
                while (colEnumerator.MoveNext())
                {
                    FungeCell cell = colEnumerator.Current.Data;
                    if (cell.value != ' ')
                    {
                        if (cell.y < bounds[0].y)
                        {
                            bounds[0].y = cell.y;
                        }
                        if (cell.y > bounds[1].y)
                        {
                            bounds[1].y = cell.y;
                        }
                        if (cell.x < bounds[0].x)
                        {
                            bounds[0].x = cell.x;
                        }
                        if (cell.x > bounds[1].x)
                        {
                            bounds[1].x = cell.x;
                        }
                    }
                }
            }
            return bounds;
        }

        public static void ChangeData(FungeNode node, int new_value)
        {
            node.Data = new FungeCell(node.Data.x, node.Data.y, new_value);
        }

        /// <summary>
        /// Changes a range of Fungespace
        /// </summary>
        /// <param name="position">The position to start at</param>
        /// <param name="string">The values to place</param>
        /// <param name="delta">The direction to lay them down in</param>
        /// <returns>The final position where the last value was placed</returns>
        public static FungeNode ChangeDataRange(FungeNode position, string new_values, Vector2 delta)
        {
            //For every new value to insert, insert it, then move
            for (int i = 0; i < new_values.Length; i++)
            {
                position = position.ParentMatrix.InsertCell(position.Data.x, position.Data.y, new_values[i]);
                position = FungeSpaceUtils.MoveTo(position, delta);
            }
            return position;
        }

        /// <summary>
        /// Changes a range of Fungespace
        /// </summary>
        /// <param name="position">The position to start at</param>
        /// <param name="new_values">The values to place</param>
        /// <param name="delta">The direction to lay them down in</param>
        /// <returns>The final position where the last value was placed</returns>
        public static FungeNode ChangeDataRange(FungeNode position, List<int> new_values, Vector2 delta)
        {
            for (int i = 0; i < new_values.Count; i++)
            {
                position = position.ParentMatrix.InsertCell(position.Data.x, position.Data.y, new_values[i]);
                position = FungeSpaceUtils.MoveTo(position, delta);
            }
            return position;
        }
        /// <summary>
        /// Move's an object's position node to a specific place, creating a node there if need be
        /// </summary>
        /// <param name="position">The position node of an object</param>
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

        public static FungeNode MoveTo(FungeNode position, Vector2 delta)
        {
            return MoveTo(position, position.Data.y + delta.y, position.Data.x + delta.x);
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

        public static void DrawFungeSpace(FungeNode draw_origin, FungeSpaceArea drawable_bounds)
        {
            draw_origin.ParentMatrix.EnumerationArea = drawable_bounds;
            foreach (var traverse in draw_origin.ParentMatrix)
            {
                ConsoleColor color = ConsoleColor.White;

                int value = traverse.Data.value;
                if (value >= ' ' && value <= '~'
                    && OptionsManager.Get<bool>("V","COLOR_SYNTAX_HIGHLIGHTING"))
                {
                    color = Instructions.InstructionManager.InstructionSet[value].Color;
                }

                char character = (char)traverse.Data.value;

                int row    = traverse.Data.y - drawable_bounds.top;
                int column = traverse.Data.x - drawable_bounds.left;
                
                //Ensures that we'll never draw inside the sidebar
                if (row < drawable_bounds.Height && column < drawable_bounds.Width)
                {
                    ConEx.ConEx_Draw.InsertCharacter(character,
                                                     row,
                                                     column,
                                                     color,
                                                     ConsoleColor.Black);//ConsoleColor.DarkGray);//Change if you want to debug where FungeSpaceCells have been inserted
                }
            }
        }
        
        public static void TestMatrix()
        {
            FungeNode traverse = null;
            if (true)
            {
                //Many columns, 1 row
                FungeSparseMatrix matrix1 = new FungeSparseMatrix();//10, 10);
                
                matrix1.InsertCell(-1, 0, 'a');
                matrix1.InsertCell(-2, 0, 'b');
                matrix1.InsertCell(-5, 0, 'c');
                matrix1.InsertCell(-4, 0, 'd');
                matrix1.InsertCell(0, -1, 'e');
                matrix1.InsertCell(0, -2, 'f');
                matrix1.InsertCell(0, -5, 'g');
                matrix1.InsertCell(0, -4, 'h');
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

        public static void WalkOnMatrix(FungeSparseMatrix matrix)
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
