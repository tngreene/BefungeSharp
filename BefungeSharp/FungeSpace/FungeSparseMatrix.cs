using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.FungeSpace
{
    /// <summary>
    /// A FungeSparseMatrix enumerator to enumerate just over a single row of a matrix,
    /// a range of [x,y] where x is unchanging and y is between int.Min and int.Max
    /// </summary>
    public class FungeSparseMatrixRowEnumerator : IEnumerator<FungeNode>
    {
        private FungeNode _traverse;
        private FungeNode _search_origin;

        private int _lower_bound;
        private int _higher_bound;

        /// <summary>
        /// Enumerates over a row of the FungeSparseMatrix ranging from rows int.Min to int.Max
        /// </summary>
        /// <param name="node">The node to start at</param>
        public FungeSparseMatrixRowEnumerator(FungeNode node)
        {
            _search_origin = node;
            _traverse = null;

            _lower_bound = int.MinValue;
            _higher_bound = int.MaxValue;
        }

        /// <summary>
        /// Enumerates over a row of the FungeSparseMatrix ranging from rows int.Min to int.Max
        /// </summary>
        /// <param name="node">The node to start at</param>
        /// <param name="lower_bound">The lowest row to examine</param>
        /// <param name="higher_bound">The highest row to examine</param>
        public FungeSparseMatrixRowEnumerator(FungeNode node, int lower_bound, int higher_bound)
        {
            _search_origin = node;
            _traverse = null;

            _lower_bound = lower_bound;
            _higher_bound = higher_bound;
        }

        public FungeNode Current
        {
            get { return _traverse; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _traverse; }
        }

        public void Dispose()
        {
            //Nothing needed
        }

        public bool MoveNext()
        {
            bool moved = false;

            //Cases
            //0.) We are starting the enumeration
            //1.) If we find the "next one" is m_Origin for any reason, we are done
            //2.) If you are are 0,0 and the bounds are 0,0+y immediantly jump to next inside 0+y
            //3.) If the next one is smaller than our lower bounds, search for the next one inside our lower bound (and not m_Origin)
                //4.) if(_traverse.South >= higher_bound) = false
            //5.) We have finished the enumeration

            //Case 0.)
            if (_traverse == null)
            {
                _traverse = _search_origin;
                
                while (_traverse.Data.y < _lower_bound)
                {
                    _traverse = _traverse.South;

                    if (_traverse == _search_origin)
                    {
                        //If we have tried as hard as we can to find a place to start
                        //yet we found ourselfs back at the beginning
                        //We say there is no search!
                        return false;
                    }
                }
                _search_origin = _traverse;

                moved = true;
                return moved;
            }

            //Takes care of cases 1.) (implicitly), 2.), 3.), 4.) results cleaned up and used next
            while (_traverse.South != _search_origin &&
                  (_traverse.South.Data.y < _lower_bound || _traverse.South.Data.y > _higher_bound))
            {
                //Go to the next one
                _traverse = _traverse.South;
            }
  
            //If you won't be back at the origin
            if (_traverse.South != _search_origin)
            {
                _traverse = _traverse.South;
                //When we move past 
                moved = true;
            }
            else
            {
                //Case 5.)
                _traverse = null;
                moved = false;
            }
            return moved;
        }

        public void Reset()
        {
            _traverse = null;
        }
    }

    /// <summary>
    /// A FungeSparseMatrix enumerator to enumerate just over a column of a matrix,
    /// a range of [x,y] where x is between int.Min and int.Max and y is unchanging
    /// </summary>
    public class FungeSparseMatrixColumnEnumerator : IEnumerator<FungeNode>
    {
        private FungeNode _traverse;
        private FungeNode _search_origin;

        private int _lower_bound;
        private int _higher_bound;

        /// <summary>
        /// Enumerates over a column of the FungeSparseMatrix ranging from rows int.Min to int.Max
        /// </summary>
        /// <param name="node">The node to start at</param>
        public FungeSparseMatrixColumnEnumerator(FungeNode node)
        {
            _search_origin = node;
            _traverse = null;

            _lower_bound = int.MinValue;
            _higher_bound = int.MaxValue;
        }

        /// <summary>
        /// Enumerates over a column of the FungeSparseMatrix ranging from rows int.Min to int.Max
        /// </summary>
        /// <param name="node">The node to start at</param>
        /// <param name="lower_bound">The lowest row to examine</param>
        /// <param name="higher_bound">The highest row to examine</param>
        public FungeSparseMatrixColumnEnumerator(FungeNode node, int lower_bound, int higher_bound)
        {
            _search_origin = _traverse = node;
            _traverse = null;

            _lower_bound = lower_bound;
            _higher_bound = higher_bound;
        }

        public FungeNode Current
        {
            get { return _traverse; }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _traverse; }
        }

        public void Dispose()
        {
            //Nothing needed
        }

        public bool MoveNext()
        {
            bool moved = false;

            //Cases
            //0.) We are starting the enumeration
            //1.) If we find the "next one" is search_origin for any reason, we are done
            //2.) If you are are 0,0 and the bounds are 0,0+x immediantly jump to next inside 0+x
            //3.) If the next one is smaller than our lower bounds, search for the next one inside our lower bound (and not m_Origin)
                //4.) if(_traverse.South >= higher_bound) = false
            //5.) We have finished the enumeration
            if (_traverse == null)
            {
                _traverse = _search_origin;
                
                while (_traverse.Data.x < _lower_bound)
                {
                    _traverse = _traverse.East;
                    
                    if (_traverse == _search_origin)
                    {
                        //If we have tried as hard as we can to find a place to start
                        //yet we found ourselfs back at the beginning
                        //We say there is no search!
                        return false;
                    }
                }
                _search_origin = _traverse;

                moved = true;
                return moved;
            }
            
            //Takes care of cases 1.) (implicitly), 2.), 3.), results cleaned up and used next
            //While we the next isn't _seach_origin or the next isn't out of (lower or higher) bounds
            while (_traverse.East != _search_origin &&
                  (_traverse.East.Data.x < _lower_bound || _traverse.East.Data.x > _higher_bound))
            {
                //Go to the next one
                _traverse = _traverse.East;
            }

            //If you won't be back at the origin
            if (_traverse.East != _search_origin)
            {    
                _traverse = _traverse.East;
                //When we move past 
                moved = true;
            }
            else
            {
                //Case 5.)
                _traverse = null;
                moved = false;
            }
            return moved;
        }

        public void Reset()
        {
            _traverse = null;
        }
    }

    public class FungeSparseMatrix : IEnumerable<FungeNode>
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
        public FungeNode Origin { get { return m_Origin; } set { m_Origin = value; } }

        /// <summary>
        /// The area to enumerate over, if bounds are desired they must be set per enumeration and are
        /// reset to FS_THEORETICAL every time afterward
        /// </summary>
        public FungeSpaceArea EnumerationArea { get; internal set; }

        private FungeSpaceArea _matrix_bounds;
        public FungeSpaceArea MatrixBounds { get { return _matrix_bounds; } }
        /// <summary>
        /// Theoretical FungeSpace is the FungeSpace described in the language specification
        /// </summary>
        public static readonly FungeSpaceArea FS_THEORETICAL = new FungeSpaceArea(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);

        /// <summary>
        /// Instantiates a new blank FungeSparseMatrix
        /// </summary>
        public FungeSparseMatrix()
        {
            m_Nodes = new List<FungeNode>();

            FungeCell data = new FungeCell(0, 0, ' ');
            data.x = 0;
            data.y = 0;
            data.value = ' ';

            m_Origin = new FungeNode(data,this);
            m_Origin.ParentMatrix = this;
            m_Nodes.Add(m_Origin);

            EnumerationArea = FS_THEORETICAL;
            _matrix_bounds = new FungeSpaceArea(0, 0, 0, 0);
        }

        /// <summary>
        /// Instantiates and fills with content, a new FungeSparseMatrix
        /// </summary>
        /// <param name="rows">The number of rows for the new matrix, 1 based</param>
        /// <param name="columns">The number of columns for the new matrix, 1 based</param>
        /// <param name="initial_content">The initial content (or ' ' if left null) of the matrix</param>
        public FungeSparseMatrix(List<List<int>> initial_content)
        {
            //Set up the origin and the node list
            m_Nodes = new List<FungeNode>();
            m_Origin = new FungeNode(new FungeCell(0,0,' '),this);
            //If we have not been given any initial content add the origin
            if (initial_content.Count == 0)
            {
                m_Nodes.Add(m_Origin);
                return;
            }

            //Find the maximum row and column
            int rows = initial_content.Count;
            int columns = 0;
            foreach (var row in initial_content)
            {
                if (row.Count > columns)
                {
                    columns = row.Count;
                }
            }

            //Fill in the initial_content row's so it becomes 
            //this big uniform rectangle
            for (int i = 0; i < rows; i++)
            {
                while(initial_content[i].Count < columns)
                {
                    initial_content[i].Add(' ');
                }
            }
            
            //Attempt to create a matrix holder of a sufficient size
            FungeNode [,] matrixHolder = null;
            try
            {
                matrixHolder = new FungeNode[rows, columns];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //The last placed node
            FungeNode cached = m_Origin;
            
            //For all the initial content, put it into rows
            //All connected east and west
            for (int y = 0; y < rows; y++)
            { 
                FungeCell data;
                data.x = 0;
                data.y = y;
                data.value = initial_content[data.y][data.x];
                cached = InsertRow(data, cached);
                m_Nodes.Add(cached);
                matrixHolder[data.y, data.x] = cached;

                for (int x = 1; x < columns; x++)
                {
                    data.x = x;
                    data.y = y;
                    data.value = initial_content[data.y][data.x];
                    cached = InsertColumn(data, cached);
                    m_Nodes.Add(cached);
                    matrixHolder[data.y, data.x] = cached;
                }
                cached = cached.East;//Wrap around to the spine
            }

            //Loop through every one hooking up the north and south
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    //Connect the top row's north
                    if (r == 0)
                    {
                        matrixHolder[0, c].North = matrixHolder[rows - 1, c];
                    }
                    else
                    {
                        matrixHolder[r, c].North = matrixHolder[r - 1, c];
                    }

                    //Connect the bottom row's south
                    if (r == rows - 1)
                    {
                        matrixHolder[rows - 1, c].South = matrixHolder[0, c];
                    }
                    else
                    {
                        matrixHolder[r, c].South = matrixHolder[r + 1, c];
                    }
                }
            }

            EnumerationArea = FS_THEORETICAL;
            _matrix_bounds = new FungeSpaceArea(0, 0, rows-1, columns-1);
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
            //System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            EnumerationArea = new FungeSpaceArea(row, column, int.MaxValue, int.MaxValue);
            foreach (var node in this)
            {
                if (node.Data.y == row && node.Data.x == column)
                {
              //      TimeSpan span = watch.Elapsed;
                    return node;
                }
            }
            return null;   
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
        
        /// <summary>
        /// Inserts a cell into the matrix
        /// </summary>
        /// <param name="cell">A funge cell to be inserted</param>
        /// <returns>The FungeNode that was placed or altered</returns>
        public FungeNode InsertCell(FungeCell cell)
        {
            //The row node is the node where we will start searching for when we reach the column
            FungeNode spine_node = GetNode(cell.y, 0);
            
            //If a row at cell.y doesn't exist
            if (spine_node == null)
            {
                //We create a row at cell.y with a space as it's value because it technically shouldn't exist
                FungeCell anchor_cell = new FungeCell(0, cell.y, ' ');

                //We must start back at the m_Origin becase the row_node is not connected to anything
                spine_node = InsertRow(anchor_cell, m_Origin);
                m_Nodes.Add(spine_node);

                if (cell.y < _matrix_bounds.top)
                {
                    _matrix_bounds.top = cell.y;
                }
                if (cell.y > _matrix_bounds.bottom)
                {
                    _matrix_bounds.bottom = cell.y;
                }
            }

            //Now that we have our row node, we'll try to find our column
            FungeNode column_node = GetNode(cell.y, cell.x);

            //If this column turns out to be entirely new
            if (column_node == null)
            {
                //Create our column on our row
                column_node = InsertColumn(cell, spine_node);
                bool connectedNoS = AttemptCoupling(column_node);
                column_node.ParentMatrix = this;
                //Now we'll add it to the list only after we know it didn't exist before
                m_Nodes.Add(column_node);
                //Adjust bounds
                {
                    if (cell.x < _matrix_bounds.left)
                    {
                        _matrix_bounds.left = cell.x;
                    }
                    if (cell.x > _matrix_bounds.right)
                    {
                        _matrix_bounds.right = cell.x;
                    }

                    
                    
                }
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
                    //Change the contents, return the origin
                    operation_origin.Data = cell;
                    return operation_origin;
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
                    //Change the contents, return the origin
                    operation_origin.Data = cell;
                    
                    return operation_origin;
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
            
            //Our current place in the "spine" of the central column
            FungeNode vertibrae = this.GetNode(attempt_origin.Data.y, 0).North;
            FungeNode traverse = vertibrae;

            //Non-wrap north node/south node, not "real" north and south
            FungeNode northNode = null;
            FungeNode southNode = null;
            FungeNode southernEdge = null;
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
                            //Traverse is our south node
                            //Because our search direction is North East 
                            //we may find another node a row north that is closer to the attempt origin
                            southNode = traverse;

                            //In the case of the north node being null just
                            //having the closest southern node won't cut it
                            //Therefore we save what is the most southernEdge node of this column
                            //For case 2
                            if (southernEdge == null)
                            {
                                southernEdge = traverse;
                            }
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

            #region Explination of our test cases
            //A node that is above or bellow, without wrapping around

            //1.)Case N,S = NULL:
            //[][?]
            //N = NULL, S = NULL

            //2.)Case N = NULL
            //[][?]
            //[][S]

            //3.)Case S = NULL
            //[][N]
            //[][?]

            //4.)Case X
            //[][N]
            //[][?]
            //[][S]

            //     North  | South
            //NULL| 1,2   |  1,3
            //----|       |
            //Real| 3,4   |  2,4
            
            //We work through reducing possibilities
            //Are both are real?
            //  Yes:We have case 4
            //else, is North real?
            //  Yes:We have case 3, 4 is no longer possible
            //else, is South real?
            //  Yes:We have case 2, 3,4 are no longer possible
            //else,
            //  We are in 1 and nothing is needed
            #endregion
            
            //Test Case 4
            if (northNode != null && southNode != null)
            {
                attempt_origin.North = northNode;
                attempt_origin.South = northNode.South;

                northNode.South.North = attempt_origin;
                northNode.South = attempt_origin;
            }
            else if(northNode != null && southNode == null)//Test Case 3
            {
                //Implicityly a duplicate of Case 4,
                //included for thoroughness 
                attempt_origin.North = northNode;
                attempt_origin.South = northNode.South;

                northNode.South.North = attempt_origin;
                northNode.South = attempt_origin;
            }
            else if (southNode != null && northNode == null)//Test Case 2
            {
                attempt_origin.North = southernEdge;
                attempt_origin.South = southNode;

                southNode.North.South = attempt_origin;
                southNode.North = attempt_origin;
            }
            else
            {
                //We are in Case 1, implicitly taken care of in
                //the FungeNode constructor
                return false;
            }

            return true;
        }

        public IEnumerator<FungeNode> GetEnumerator()
        {
            FungeSparseMatrixRowEnumerator _row_enumerator = new FungeSparseMatrixRowEnumerator(m_Origin, EnumerationArea.top, EnumerationArea.bottom);
            
            while(_row_enumerator.MoveNext() == true)
            {
                var _column_enumerator = new FungeSparseMatrixColumnEnumerator(_row_enumerator.Current, EnumerationArea.left, EnumerationArea.right);
                while (_column_enumerator.MoveNext() == true)
                {
                    yield return _column_enumerator.Current;
                }
            }

            EnumerationArea = FS_THEORETICAL;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            FungeSparseMatrixRowEnumerator _row_enumerator = new FungeSparseMatrixRowEnumerator(m_Origin, EnumerationArea.top, EnumerationArea.bottom);
            
            while(_row_enumerator.MoveNext() == true)
            {
                var _column_enumerator = new FungeSparseMatrixColumnEnumerator(_row_enumerator.Current, EnumerationArea.left, EnumerationArea.right);
                while (_column_enumerator.MoveNext() == true)
                {
                    yield return _column_enumerator.Current;
                }
            }

            EnumerationArea = FS_THEORETICAL;
        }

        /// <summary>
        /// Attempts to print all the cells in funge space,
        /// it is not aligned or made extra pretty.
        /// </summary>
        public void PrintFungeSpace()
        {
            Console.WriteLine();
            FungeNode f = m_Nodes[0];
            //m_Origin.ParentMatrix.EnumerationArea = new FungeSpaceArea(2, 2, 4, 4);
            //FungeSparseMatrixEnumerator enumerator = new FungeSparseMatrixEnumerator(m_Origin);
            //Console.Write("[" + (char)enumerator.Current.Data.value + "," + enumerator.Current.Data.x + "," + enumerator.Current.Data.y + "]");
            foreach (var i in m_Origin.ParentMatrix)
            {
                if (i.Data.x == 0)
                {
                    Console.WriteLine();
                }
                Console.Write("[" + (char)i.Data.value + "," + i.Data.x + "," + i.Data.y + "]");
            }            
        }
    }
}
