using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp
{
    public class IP
    {
        //The position of the IP in funge space
        private Vector2 _position;

        /// <summary>
        /// The position of the IP in funge space
        /// </summary>
        public Vector2 Position { get { return _position; } set { _position = value; } }

        /// <summary>
        /// The velocity vector d of the IP
        /// </summary>
        private Vector2 _delta;

        /// <summary>
        /// The velocity vector d of the IP
        /// </summary>
        public Vector2 Delta { get { return _delta; } set { _delta = value; } }

        private Vector2 _storageOffset;
        public Vector2 StorageOffset { get { return _storageOffset; } set { _storageOffset = value; } }

        //Needed for concurrent-98, TODO - Implement the stack stack system
        private Stack<int> _stack;
        public Stack<int> Stack { get { return _stack; } set { _stack = value; } }

        //A static counter to ensure that each added IP gets a unique ID
        private static int ID_Counter = 0;
        
        //An ID for debugging purposes
        private int _IP_ID;
        public int ID { get { return _IP_ID; } }

        public IP()
        {
            _position = Vector2.Zero;
            _delta = Vector2.East;
            _storageOffset = Vector2.Zero;

            _stack = new Stack<int>();
            
            _IP_ID = ID_Counter;
            ID_Counter++;
        }

        public IP(Vector2 position, Vector2 delta, Vector2 storageOffset, Stack<int> stack)
        {
            _position = position;
            _delta = delta;
            _storageOffset = storageOffset;

            _stack = stack;
            
            _IP_ID = ID_Counter;
            ID_Counter++;
        }

        //For use with Funge-98C
        public IP(IP parent)
        {
            this._position = parent._position;
            this._delta = parent._delta;
            this._delta.Negate();
            this._storageOffset = parent._storageOffset;
            this._stack = parent._stack;

            this._IP_ID = ID_Counter;
            ID_Counter++;
        }

        public void Move()
        {
            //TODO - Fix the j command and inputting negative values!
            //Based on the direction move or wrap the pointer around
            _position += _delta;
        }

        public void Reset()
        {
            _position.Clear();
            _delta = Vector2.East;
            _stack.Clear();
        }

        public void Stop()
        {
            _delta.Clear();
        }

        public override string ToString()
        {
            return "P: " + this._position + ", D: " + this._delta + ", S:" + this._stack + ", ID: " + this._IP_ID;
        }
    }
}
