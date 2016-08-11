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
		public static readonly FungeSpaceArea FS_93 = new FungeSpaceArea(0, 0, 24, 79);
		/// <summary>
		/// Theoretical FungeSpace is the FungeSpace described in the language specification
		/// </summary>
		public static readonly FungeSpaceArea FS_THEORETICAL = new FungeSpaceArea(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);

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

		/// <summary>
		/// Constructs the FungeSpaceArea based on an array of bounds
		/// </summary>
		/// <param name="bounds">Where [0] is top-left and [1] is bottom-right</param>
		public FungeSpaceArea(Vector2[] bounds)
		{
			this.left   = bounds[0].x;
			this.top    = bounds[0].y;
			this.right  = bounds[1].x;
			this.bottom = bounds[1].y;
		}

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
		/// <summary>
		/// Imports and overwrites existing nodes, creating nodes as necessary
		/// </summary>
		/// <param name="matrix">The matrix to import data into</param>
		/// <param name="data">The data to import</param>
		/// <param name="origin">The origin point to start</param>
		public static FungeNode ImportData(FungeSparseMatrix matrix, Vector2 origin, List<int> data)
		{
			return ImportData(matrix, new FungeSpaceArea(origin.y, origin.x, origin.y+1, origin.x + data.Count()), new List<List<int>>(){data});
		}

		/// <summary>
		/// Imports and overwrites existing nodes, creating nodes as necessary
		/// </summary>
		/// <param name="matrix">The matrix to import data into</param>
		/// <param name="data">The data to import</param>
		/// <param name="origin">The origin point to start</param>
		/// <returns>The last node imported</returns>
		public static FungeNode ImportData(FungeSparseMatrix matrix, Vector2 origin, List<List<int>> data)
		{
			return ImportData(matrix, new FungeSpaceArea(origin.y,origin.x,FungeSpaceArea.FS_THEORETICAL.bottom, FungeSpaceArea.FS_THEORETICAL.right), data);
		}

		/// <summary>
		/// Imports and overwrites existing nodes, creating nodes as necessary
		/// </summary>
		/// <param name="matrix">The matrix to import data into</param>
		/// <param name="data">The data to import</param>
		/// <param name="origin">The origin point to start</param>
		public static FungeNode ImportData(FungeSparseMatrix matrix, FungeSpaceArea import_bounds, List<List<int>> data)
		{
			FungeNode traverse = matrix.Origin;
			for (int row = 0; row < data.Count() && row < import_bounds.bottom; row++)
			{
				for (int col = 0; col < data.ElementAt(row).Count() && col < import_bounds.right; col++)
				{
					traverse = matrix.InsertCell(col, row, data.ElementAt(row).ElementAt(col));
				}
			}

			return traverse;
		}

		public static List<List<int>> ExportData(FungeSparseMatrix matrix, Vector2 origin)
		{
			return ExportData(matrix, new FungeSpaceArea(origin.y, origin.x, FungeSpaceArea.FS_THEORETICAL.bottom, FungeSpaceArea.FS_THEORETICAL.right));
		}

		public static List<List<int>> ExportData(FungeSparseMatrix matrix, FungeSpaceArea export_bounds)
		{
			FungeSparseMatrix filled_matrix = FillMatrix(matrix, export_bounds);
			List<List<int>> data = new List<List<int>>();
			
			foreach(var row in new FungeSparseMatrixRowEnumerator(filled_matrix.GetOrCreateNode(export_bounds.top, export_bounds.left), export_bounds.top, export_bounds.bottom))
			{
				data.Add(new List<int>());
				foreach (var column in new FungeSparseMatrixColumnEnumerator(row, export_bounds.left, export_bounds.right))
				{
					data.Last().Add(column.Data.value);
				}
			}

			return data;
		}
		
        public static void OverlayMatrix(FungeSparseMatrix base_matrix, FungeSparseMatrix overlay_matrix, Vector2 overlay_start)
        {
            foreach (var node in overlay_matrix)
            {
                if (node.Value == ' ')
                {
                    continue;
                }

                FungeCell overlay_cell = node.Data;
                overlay_cell.x += overlay_start.x;
                overlay_cell.y += overlay_start.y;
                base_matrix.InsertCell(overlay_cell);
            }
        }

		public static FungeSparseMatrix FillMatrix(FungeSparseMatrix original_matrix, FungeSpaceArea bounds, int value = ' ')
		{
			return FillMatrix(original_matrix, new Vector2(bounds.left, bounds.top), new Vector2(bounds.right, bounds.bottom), value);
		}

        /// <summary>
        /// Fills the matrix's empty spaces with a certain value
        /// </summary>
        /// <param name="original_matrix">The original_matrix, it will not be altered</param>
        /// <param name="value">The value to be inserted into every cell, by default ' '</param>
        /// <returns>The filled matrix</returns>
        public static FungeSparseMatrix FillMatrix(FungeSparseMatrix original_matrix, Vector2 top_left, Vector2 bottom_right, int value = ' ')
        {
            FungeSparseMatrix outMatrix = new FungeSparseMatrix();

            for (int y = top_left.y; y != bottom_right.y + 1; y++)
            {
                for (int x = top_left.x; x != bottom_right.x + 1; x++)
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
                                                     ConsoleColor.DarkGray);//Change if you want to debug where FungeSpaceCells have been inserted
                }
            }
        }
        
		/// <summary>
		/// A testing utility that lets you "walk" on the nodes,ijkl to move, q to quit
		/// </summary>
		/// <param name="matrix">The matrix to "walk" through</param>
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
