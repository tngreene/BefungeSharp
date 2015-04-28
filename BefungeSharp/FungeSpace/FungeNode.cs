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
            return "X: " + x + " Y: " + y + " Value: " + value + " (" + (char)value + ")";
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
}
