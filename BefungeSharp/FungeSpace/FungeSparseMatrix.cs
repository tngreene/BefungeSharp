using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.FungeSpace
{
    public struct FungeCell
    {
        public int x;
        public int y;
        public int value;

        public FungeCell(int x, int y, int value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }

        public static implicit operator Vector2(FungeCell cell)
        {
            return new Vector2(cell.x, cell.y);
        }

        public override string ToString()
        {
            return "X: " + x + " Y: " + y + " Value: " + value;
        }
    }

    public class FungeNode
    {
        //Attributes
        private FungeCell data;

        private FungeNode north;
        private FungeNode east;
        private FungeNode south;
        private FungeNode west;

        //One day, for when we create our Three-Torus
        //private FungeNode up;
        //private FungeNode down;

        private FungeSparseMatrix parentMatrix;
        public FungeSparseMatrix ParentMatrix
        {
            get
            {
                return parentMatrix;
            }
            internal set 
            {
                parentMatrix = value; 
            }
        }
        /// <summary>
        /// Gets and sets the data of this node
        /// </summary>
        public FungeCell Data
        {
            get { return data; }
            set { data = value; }
        }

        /// <summary>
        /// Gets and sets the north Node,
        /// set is internal so the matrix cannot be pulled apart
        /// </summary>
        public FungeNode North
        {
            get { return north; }
            internal set { north = value; }
        }
        /// <summary>
        /// Gets and sets the east Node,
        /// set is internal so the matrix cannot be pulled apart
        /// </summary>
        public FungeNode East
        {
            get { return east; }
            internal set { east = value; }
        }
        /// <summary>
        /// Gets and sets the south Node
        /// set is internal so the matrix cannot be pulled apart
        /// </summary>
        public FungeNode South
        {
            get { return south; }
            internal set { south = value; }
        }
        /// <summary>
        /// Gets and sets the west Node,
        /// set is internal so the matrix cannot be pulled apart
        /// </summary>
        public FungeNode West
        {
            get { return west; }
            internal set { west = value; }
        }

        /// <summary>
        /// Creates new Node with data
        /// </summary>
        /// <param name="data">A funge cell to store in node</param>
        public FungeNode(FungeCell data, FungeSparseMatrix parent)
        {
            this.data = data;
            this.parentMatrix = parent;
            //All sides wrap around to themselfs unless otherwise set
            north = this;
            east = this;
            south = this;
            west = this;
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }

    public class FungeSparseMatrix
    {
        /// <summary>
        /// The big heap of nodes
        /// </summary>
        private List<FungeNode> m_Nodes;
        
        //Our m_Origin, cached
        private FungeNode m_Origin;

        /// <summary>
        /// The Origin, at 0,0 in the matrix. Is always ensured to exist.
        /// </summary>
        public FungeNode Origin { get { return m_Origin; } private set { m_Origin = value; } }

        /// <summary>
        /// Instantiates a new blank FungeSparseMatrix
        /// </summary>
        public FungeSparseMatrix()
        {
            m_Nodes = new List<FungeNode>();

            FungeCell data = new FungeCell();
            data.x = 0;
            data.y = 0;
            data.value = ' ';

            m_Origin = new FungeNode(data,this);
            m_Origin.ParentMatrix = this;
            m_Nodes.Add(m_Origin);
        }

        /// <summary>
        /// Instantiates and fills, with ' 's, a new FungeSparseMatrix
        /// </summary>
        /// <param name="rows">The number of rows for the new matrix</param>
        /// <param name="columns">The number of columns for the new matrix</param>
        public FungeSparseMatrix(int rows, int columns)
        {
            m_Nodes = new List<FungeNode>();
            FungeCell data = new FungeCell();
            data.x = 0;
            data.y = 0;
            data.value = ' ';

            m_Origin = new FungeNode(data,this);
            
            m_Nodes.Add(m_Origin);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    data.x = x;
                    data.y = y;
                    data.value = ' ';
                    InsertCell(data);
                }
            }
        }

        /// <summary>
        /// The copy constructor for FungeSparseMatrix, preforms a deep copy of the matrix
        /// </summary>
        /// <param name="copy">The matrix to copy from</param>
        public FungeSparseMatrix(FungeSparseMatrix copy)
        {
            m_Nodes = new List<FungeNode>(copy.m_Nodes.Count);
            m_Origin = new FungeNode(new FungeCell(0, 0, copy.m_Origin.Data.value), this);
            m_Nodes.Add(m_Origin);

            for (int i = 1; i < copy.m_Nodes.Count; i++)
            {
                this.InsertCell(copy.m_Nodes[i].Data);
            }
        }

        /// <summary>
        /// Attempts to get a cell at a row and column
        /// </summary>
        /// <param name="row">The row to query</param>
        /// <param name="column">The column to query</param>
        /// <returns>The FungeNode if found or null if not found</returns>
        public FungeNode GetNode(int row, int column)
        {
            //Attemp to get the row
            FungeNode row_node = GetRow(row, m_Origin);

            //If it was not found in a row it cannot exist
            if (row_node == null)
            {
                return null;
            }

            //Now that we have our row node, we'll try to find our column
            FungeNode final_node = GetColumn(column, row_node);

            //If that space in its row and column does not exist at all
            if (final_node == null)
            {
                //Return null
                return null;
            }
            //We found it!
            return final_node;
        }

        /// <summary>
        /// Gets the FungeNode where the row is, if it exists
        /// </summary>
        /// <param name="row">The row to search for</param>
        /// <param name="search_origin">The row to start searching for the row we intend</param>
        /// <returns>The existing node at that row or null for if no row exists at that row</returns>
        private FungeNode GetRow(int row, FungeNode search_origin)
        {
            //Start at the place we're starting from
            FungeNode traverse = search_origin;

            if (row > 0)
            {
                //Keep moving while cell.y > traverse.y and we haven't reached the end of the list yet
                while (row > traverse.Data.y)
                {
                    //If we searched through the whole row and didn't find that row
                    if (traverse.South == search_origin)
                    {
                        //return null
                        return null;
                    }
                    else if (row < traverse.South.Data.y)
                    {
                        //For the cases when we are between rows
                        return null;
                    }
                    //Otherwise lets check the next place
                    traverse = traverse.South;
                }
            }
            else if (row < 0)
            {
                //Keep moving while cell.y > traverse.y and we haven't reached the end of the list yet
                while (row < traverse.Data.y)
                {
                    if (traverse.North == m_Origin)
                    {
                        return null;
                    }
                    else if (row > traverse.North.Data.y)
                    {
                        //For the cases when we are between rows
                        return null;
                    }
                    //Otherwise lets check the next place
                    traverse = traverse.North;
                }
            }
            else if (row == 0)
            {
                return traverse;
            }
            return traverse;
        }

        /// <summary>
        /// Gets the FungeNode where the column is, if it exists
        /// </summary>
        /// <param name="column">The column to search for</param>
        /// <param name="row_to_search">The row to search through</param>
        /// <returns>The existing node at that column or null for if no column exists at that column</returns>
        private FungeNode GetColumn(int column, FungeNode row_to_search)
        {
            //Start at the place we're starting from
            FungeNode traverse = row_to_search;

            if (column > 0)
            {
                //Keep moving while cell.x > traverse.x and we haven't reached the end of the list yet
                while (column > traverse.Data.x)
                {
                    if (traverse.East == row_to_search)
                    {
                        return null;
                    }
                    else if (column < traverse.East.Data.x)
                    {
                        //For the cases when we are between columns
                        return null;
                    }
                    //Otherwise lets check the next place
                    traverse = traverse.East;
                }
            }
            else if (column < 0)
            {
                //Keep moving while cell.x < traverse.x and we haven't reached the end of the list yet
                while (column < traverse.Data.x)
                {
                    if (traverse.West == row_to_search)
                    {
                        return null;
                    }
                    else if (column > traverse.West.Data.x)
                    {
                        //For the cases when we are between columns
                        return null;
                    }
                    //Otherwise lets check the next place
                    traverse = traverse.West;
                }
            }
            else if (column == 0)
            {
                return traverse;
            }
            return traverse;
        }

        /// <summary>
        /// Inserts a cell into the matrix using the seperate components of a funge cell
        /// </summary>
        /// <param name="x">The x position of the cell</param>
        /// <param name="y">The y position of the cell</param>
        /// <param name="value">The value to be placed</param>
        /// <returns>The FungeNode that was placed or altered</returns>
        public FungeNode InsertCell(int x, int y, int value)
        {
            return InsertCell(new FungeCell(x, y, value));
        }

        /// <summary>
        /// Inserts a cell into the matrix using a vector and an int
        /// </summary>
        /// <param name="cell">A position vector of where to insert a cell</param>
        /// <param name="value">The value to be placed in that location</param>
        /// <returns>The FungeNode that was placed or altered</returns>
        public FungeNode InsertCell(Vector2 cell, int value)
        {
            return InsertCell(new FungeCell(cell.x, cell.y, value));
        }
        static int count = 0;
        /// <summary>
        /// Inserts a cell into the matrix
        /// </summary>
        /// <param name="cell">A funge cell to be inserted</param>
        /// <returns>The FungeNode that was placed or altered</returns>
        public FungeNode InsertCell(FungeCell cell)
        {
            //The row node is the node where we will start searching for when we reach the column
            FungeNode row_node = GetRow(cell.y, m_Origin);
            bool needsToAddAnchor = false;
            //If a row at cell.y doesn't exist
            if (row_node == null)
            {
                //We create a row at cell.y with a space as it's value because it technically shouldn't exist
                FungeCell anchor_cell = new FungeCell(0, cell.y, ' ');

                //We must start back at the m_Origin becase the row_node is not connected to anything
                row_node = InsertRow(anchor_cell, m_Origin);
                needsToAddAnchor = true;
            }

            //Now that we have our row node, we'll try to find our column
            FungeNode column_node = GetColumn(cell.x, row_node);

            //If this column turns out to be entirely new
            if (column_node == null)
            {
                //Create our column on our row
                column_node = InsertColumn(cell, row_node);
                bool connectedNoS = AttemptCoupling(column_node);
                column_node.ParentMatrix = this;
                //Now we'll add it to the list only after we know it didn't exist before
                m_Nodes.Add(column_node);
                return column_node;
            }
            else if(needsToAddAnchor == true)
            {
                m_Nodes.Add(column_node);
                return column_node;
            }
            else
            {
                //The row and column exists and it's info must be updated
                column_node.Data = cell;
                return column_node;
            }
        }

        private FungeNode InsertRow(FungeCell cell, FungeNode operation_origin)
        {
            FungeNode traverse = operation_origin;
            FungeNode newRow = new FungeNode(cell,this);

            if (cell.y == traverse.Data.y)
            {
                if (cell.x == traverse.Data.x)
                {
                    //Nothing to insert, return null
                    return null;
                }
            }

            bool negative_column = cell.y < 0 ? true : false;
            if (negative_column == false)
            {
                //Keep moving while cell.y is bigger than the current cell.y
                //and we haven't reached the end of the list yet
                while (cell.y > traverse.South.Data.y/* && traverse.South != operation_origin*/)
                {
                    //If the next thing is actually a negative number
                    if (traverse.South.Data.y <= 0)
                    {
                        //We've found where to place our node
                        break;
                    }
                    //Otherwise lets check the next place
                    traverse = traverse.South;
                }

                newRow.North = traverse;
                newRow.South = traverse.South;

                traverse.South.North = newRow;
                traverse.South = newRow;
            }
            else
            {
                //Keep moving while cell.y is smaller than the current cell.y
                //and we haven't reached the end of the list yet
                while (cell.y < traverse.North.Data.y /*&& traverse.North != operation_origin*/)
                {
                    //If the next thing is actually a positive number
                    if (traverse.North.Data.y >= 0)
                    {
                        //We've found our spot
                        break;
                    }
                    //Otherwise lets check the next place
                    traverse = traverse.North;
                }

                newRow.South = traverse;
                newRow.North = traverse.North;

                traverse.North.South = newRow;
                traverse.North = newRow;
            }
            return newRow;
        }

        private FungeNode InsertColumn(FungeCell cell, FungeNode operation_origin)
        {
            //Start at the place we're starting from
            FungeNode traverse = operation_origin;
            FungeNode newColumn = new FungeNode(cell,this);

            if (cell.y == traverse.Data.y)
            {
                if (cell.x == traverse.Data.x)
                {
                    //Nothing to insert, return null
                    return null;
                }
            }

            bool negative_column = cell.x < 0 ? true : false;
            if (negative_column == false)
            {
                //Keep moving while cell.x is bigger than the current cell.x
                //and we haven't reached the end of the list yet
                while (cell.x > traverse.East.Data.x /*&& traverse.East != operation_origin*/)
                {
                    //If the next thing is actually a negative number
                    if (traverse.East.Data.x <= 0)
                    {
                        //We've found where to place our node
                        break;
                    }
                    //Otherwise lets check the next place
                    traverse = traverse.East;
                }

                newColumn.West = traverse;
                newColumn.East = traverse.East;

                traverse.East.West = newColumn;
                traverse.East = newColumn;
            }
            else
            {
                //Keep moving while cell.x is smaller than the current cell.x
                //and we haven't reached the end of the list yet
                while (cell.x < traverse.West.Data.x /*&& traverse.West != operation_origin*/)
                {
                    //If the next thing is actually a positive number
                    if (traverse.West.Data.x >= 0)
                    {
                        //We've found our spot
                        break;
                    }
                    //Otherwise lets check the next place
                    traverse = traverse.West;
                }

                newColumn.East = traverse;
                newColumn.West = traverse.West;

                traverse.West.East = newColumn;
                traverse.West = newColumn;
            }
            return newColumn;

        }

        /// <summary>
        /// Attempts to hook a new node's North and South to the next
        /// availble node above and below it as needed.
        /// </summary>
        /// <param name="attempt_origin">The new node to attempt this</param>
        /// <returns>Returns true if the any hook, N or S, was made. False if not.</returns>
        private bool AttemptCoupling(FungeNode attempt_origin)
        {
            //Problem: Cells where x != 0, may not have their and N or S hooked up when they could and should be

            //Solution: Every new cell must attempt to attach to their closest possible node to their North or South.
            //The Search: We travel back to the central column (0,y) and travel up each row, searching until we find a place where the x locations match.
            //We hook up applicable nodes until we arrive back at the row we started at.
            
            FungeNode traverse = attempt_origin;

            //Move back to the central column
            while (traverse.Data.x != 0)
            {
                //If this turns out to be very slow
                //We'll add some heuristics
                traverse = traverse.East;
            }
            
            //Attempt to go north one column
            traverse = traverse.North;

            //Our current place in the "spine" of the central column
            FungeNode vertibrae = traverse;

            FungeNode northNode = null;
            FungeNode southNode = null;

            while(vertibrae.Data.y != attempt_origin.Data.y)
            {
                //Start at the vertibrae
                traverse = vertibrae;
                
                do
                {
                    //Travel around the row
                    traverse = traverse.East;
                    //We get find a place where the x's line up
                    if (traverse.Data.x == attempt_origin.Data.x)
                    {
                        //Now we test if we are north or south of the original attempt
                        //Remember, y increases as we go down and number != north or south!
                        if (traverse.Data.y > attempt_origin.Data.y)
                        {
                            if (southNode == null)
                            {
                                //Traverse is our south node
                                southNode = traverse;
                            }
                            break;
                        }
                        else if (traverse.Data.y < attempt_origin.Data.y)
                        {
                            if (northNode == null)
                            {
                                //Traverse is our north node
                                northNode = traverse;
                            }
                            break;
                        }
                    }
                }
                while (traverse.Data.x != vertibrae.Data.x);
                
                //Go up to the next vertibrae
                vertibrae = vertibrae.North;
            }
            
            //now that we (possibly have the north and south nodes found its time to connect them up to the attempt_origin
            if (northNode != null)
            {
                attempt_origin.North = northNode;
                attempt_origin.South = northNode.South;

                northNode.South.North = attempt_origin;
                northNode.South = attempt_origin;
            }

            if (southNode != null)
            {
                attempt_origin.South = southNode;
                attempt_origin.North = southNode.North;

                southNode.North.South = attempt_origin;
                southNode.North = attempt_origin;
            }

            if (northNode == null && southNode == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Attempts to print all the cells in funge space,
        /// it is not aligned or made extra pretty.
        /// </summary>
        public void PrintFungeSpace()
        {
            FungeNode f = m_Nodes[0];

            FungeNode rowsStart = f;
            do
            {
                FungeNode columnsStart = f;
                do
                {
                    Console.Write("[" + (char)f.Data.value + "," + f.Data.x + "," + f.Data.y + "]");
                    f = f.East;
                }
                while (f != columnsStart);
                Console.WriteLine();
                f = f.South;
            }
            while (f != rowsStart);
        }
    }


}
