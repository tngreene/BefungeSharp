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

        //One day, for when we create our Three-Torus
        //private FungeNode up;
        //private FungeNode down;

		public FungeSparseMatrix ParentMatrix { get; internal set; }

        /// <summary>
        /// Gets and sets the data of this node
        /// </summary>
		public FungeCell Data { get { return data; } internal set { data = value; } }

		public int X { get { return data.x; } }

		public int Y { get { return data.y; } }

		public int Value { get { return data.value; } set { data.value = value; } }

		public Vector2 Location { get { return new Vector2(X, Y); } }
        /// <summary>
        /// Gets and sets the north Node,
        /// set is internal so the matrix cannot be pulled apart
        /// </summary>
        public FungeNode North  { get; internal set; }

        /// <summary>
        /// Gets and sets the east Node,
        /// set is internal so the matrix cannot be pulled apart
        /// </summary>
		public FungeNode East { get; internal set; }

        /// <summary>
        /// Gets and sets the south Node
        /// set is internal so the matrix cannot be pulled apart
        /// </summary>
        public FungeNode South  { get; internal set; }

        /// <summary>
        /// Gets and sets the west Node,
        /// set is internal so the matrix cannot be pulled apart
        /// </summary>
        public FungeNode West  { get; internal set; }
 
        /// <summary>
        /// Creates new Node with data
        /// </summary>
        /// <param name="data">A funge cell to store in node</param>
        public FungeNode(FungeCell data, FungeSparseMatrix parent)
        {
            this.Data = data;
            this.ParentMatrix = parent;
            //All sides wrap around to themselfs unless otherwise set
            North = this;
            East = this;
            South = this;
            West = this;
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }   
}
