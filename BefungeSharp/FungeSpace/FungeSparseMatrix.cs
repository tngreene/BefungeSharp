﻿using System;
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

        /// <summary>
        /// This prepares MoveNext to make its descision, it also makes sure that _traverse.South is always >= _lower_bound
        /// if it can be
        /// </summary>
        private void SearchForNextSouthernInsideBounds()
        {
            //While we the next isn't search_origin or the next isn't out of (lower) bounds
            while (_traverse.South != _search_origin &&
                  _traverse.South.Data.y < _lower_bound)
            {
                //Go to the next one
                _traverse = _traverse.South;
            }
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
        /// The area to enumerate over, defaults every time to FS_THEORETICAL
        /// </summary>
        private FungeSpaceArea _area;
        
        /// <summary>
        /// The area to enumerate over, if bounds are desired they must be set per enumeration and are
        /// reset to FS_THEORETICAL every time afterward
        /// </summary>
        public FungeSpaceArea EnumerationArea { get { return _area; } internal set { _area = value; } }

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

            _area = FS_THEORETICAL;
            _matrix_bounds = new FungeSpaceArea(0, 0, 0, 0);
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

            for (int y = 0; y <= rows; y++)
            {
                Console.Write(y + " ");
                for (int x = 0; x <= columns; x++)
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
            foreach (var node in this)
            {
                if (node.Data.y == row && node.Data.x == column)
                {
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

        public IEnumerator<FungeNode> GetEnumerator()
        {
            FungeSparseMatrixRowEnumerator _row_enumerator = new FungeSparseMatrixRowEnumerator(m_Origin, _area.top, _area.bottom);
            
            while(_row_enumerator.MoveNext() == true)
            {
                var _column_enumerator = new FungeSparseMatrixColumnEnumerator(_row_enumerator.Current, _area.left, _area.right);
                while (_column_enumerator.MoveNext() == true)
                {
                    yield return _column_enumerator.Current;
                }
            }

            _area = FS_THEORETICAL;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            FungeSparseMatrixRowEnumerator _row_enumerator = new FungeSparseMatrixRowEnumerator(m_Origin, _area.top, _area.bottom);
            
            while(_row_enumerator.MoveNext() == true)
            {
                var _column_enumerator = new FungeSparseMatrixColumnEnumerator(_row_enumerator.Current, _area.left, _area.right);
                while (_column_enumerator.MoveNext() == true)
                {
                    yield return _column_enumerator.Current;
                }
            }

            _area = FS_THEORETICAL;
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
