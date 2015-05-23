using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BefungeSharp.FungeSpace;

namespace BefungeSharp
{
    public class IP
    {
        //The position of the IP in funge space
        private FungeNode _position;

        /// <summary>
        /// The position of the IP in funge space
        /// </summary>
        public FungeNode Position { get { return _position; } set { _position = value; } }

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

        private Stack<Stack<int>> _stackStack;
        
        /// <summary>
        /// A public interface for dealing with the TOSS (which is what most people ever will use)
        /// </summary>
        public Stack<int> Stack { get { return _stackStack.Peek(); } }

        /// <summary>
        /// A public interface for dealing with the real stack stack
        /// </summary>
        public Stack<Stack<int>> StackStack { get { return _stackStack; } }

        /// <summary>
        /// Active is whether the IP should be drawn/updated/moved/anything. It is not the same a stopped IP
        /// All IPs start out inactive and are only activated when ready
        /// </summary>
        private bool _active;
        public bool Active { get { return _active; } set { _active = value; } }

        //A static counter to ensure that each added IP gets a unique ID starting with 0
        private static int ID_Counter = 0;
        
        //Beware! Only to be used when ending the interpreter!
        public static void ResetCounter() { ID_Counter = 0; }

        private int _IP_ParentID;
        public int IP_ParentID { get { return _IP_ParentID; } }

        //An ID for debugging purposes/concurrency
        private int _IP_ID;
        public int ID { get { return _IP_ID; } }

        /// <summary>
        /// For if we are currently picking up chars in a string mode 
        /// </summary>
        public bool StringMode { get; set; }

        public IP()
        {
            _position = null;
            _delta = Vector2.East;
            _storageOffset = Vector2.Zero;

            _stackStack = new Stack<Stack<int>>();
            _stackStack.Push(new Stack<int>());
            _active = false;

            _IP_ParentID = 0;
            _IP_ID = ID_Counter;
            ID_Counter++;
            StringMode = false;
        }

        public IP(FungeNode position, Vector2 delta, Vector2 storageOffset, Stack<Stack<int>> stack_stack, int parent_id, bool willIncrementCounter)
        {
            _position = position;
            _delta = delta;
            _storageOffset = storageOffset;

            _stackStack = stack_stack;
            _active = false;

            _IP_ParentID = parent_id;

            if (willIncrementCounter == false)
            {
                _IP_ID = -1;
            }
            else
            {
                _IP_ID = ID_Counter;
                ID_Counter++;
            }
            StringMode = false;
        }

        //For use with Funge-98C
        public IP(IP parent)
        {
            this._position = parent._position;
            this._delta = parent._delta;
            this._storageOffset = parent._storageOffset;
            
            this._stackStack = new Stack<Stack<int>>();
            //Copy the exact contents and order of the stack stack
            for (int stackNumber = parent._stackStack.Count - 1; stackNumber >= 0; stackNumber--)
            {
                this._stackStack.Push(new Stack<int>());
                Stack<int> currentStack = parent._stackStack.ElementAt(stackNumber);
                for (int i = currentStack.Count - 1; i >= 0; i--)
                {
                    this._stackStack.Peek().Push(currentStack.ElementAt(i));
                }
            }
            _active = false;

            this._IP_ParentID = parent._IP_ParentID;
            this._IP_ID = ID_Counter;
            ID_Counter++;

            StringMode = false;
        }

        public void Move()
        {
            this._position = FungeSpaceUtils.MoveBy(this._position, this._delta);
        }

        public void Move(int repeat)
        {
            for (int i = 0; i <= repeat; i++)
            {
                Move();
            }
        }

        //TODO - fix pass by value/reference problem
        public void Negate()
        {
            this._delta.Negate();
        }

        public void Reset()
        {
            _position = _position.ParentMatrix.Origin;
            _delta = Vector2.East;
            _stackStack = new Stack<Stack<int>>();

            _active = false;
            StringMode = false;
        }

        public void Stop()
        {
            _delta.Clear();
        }

        //For when you need 
        public FungeNode GetPosition()
        {
            return _position;
        }
        /// <summary>
        /// Get's the cell currently under the IP and returns it's value
        /// </summary>
        /// <returns></returns>
        public FungeCell GetCurrentCell()
        {
            return _position.Data;
        }
        public override string ToString()
        {
            return "P: " + this._position + ", D: " + this._delta + ", TOSS:" + this.Stack + ", ID: " + this._IP_ID;
        }
    }
}
