using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BefungeSharp.FungeSpace;
using BefungeSharp.Instructions;
using BefungeSharp.Instructions.Fingerprints;

namespace BefungeSharp
{
    public class IP
    {
        /// <summary>
        /// The position of the IP in funge space
        /// </summary>
        public FungeNode Position { get; set; }

		/// <summary>
        /// The velocity vector d of the IP
        /// </summary>
        public Vector2 Delta { get; set; }

		/// <summary>
		/// The storage offset of the IP
		/// </summary>
        public Vector2 StorageOffset { get; set; }

        /// <summary>
        /// A public interface for dealing with the TOSS (which is what most people ever will use)
        /// </summary>
		public Stack<int> Stack { get { return StackStack.Peek(); } }

        /// <summary>
        /// A public interface for dealing with the real stack stack
        /// </summary>
        public Stack<Stack<int>> StackStack { get; private set; }

		/// <summary>
		/// The dictionary of ip's loaded fingerprints, by default is loaded with "NULL"
		/// </summary>
		public List<Fingerprint> LoadedFingerprints { get; private set; }

        /// <summary>
        /// Active is whether the IP should be drawn/updated/moved/anything. It is not the same a stopped IP
        /// All IPs start out inactive and are only activated when ready
        /// </summary>
        public bool Active { get; set; }

        //A static counter to ensure that each added IP gets a unique ID starting with 0
        private static int ID_Counter = 0;
        
        //Beware! Only to be used when ending the interpreter!
        public static void ResetCounter() { ID_Counter = 0; }

        public int IP_ParentID { get; private set; }

        //An ID for debugging purposes/concurrency
        public int ID { get; private set; }

        /// <summary>
        /// For if we are currently picking up chars in a string mode 
        /// </summary>
        public bool StringMode { get; set; }

        public IP()
        {
            Position = null;
            Delta = Vector2.East;
            StorageOffset = Vector2.Zero;

            StackStack = new Stack<Stack<int>>();
            StackStack.Push(new Stack<int>());
            Active = false;

            IP_ParentID = 0;
            ID = ID_Counter;
            ID_Counter++;
            StringMode = false;

			LoadedFingerprints = new List<Fingerprint>();
			LoadedFingerprints.Add(new Instructions.Fingerprints.NULL.NULL());
			LoadedFingerprints.First().Load();
        }

        public IP(FungeNode position, Vector2 delta, Vector2 storageOffset, Stack<Stack<int>> stack_stack, int parent_id, bool willIncrementCounter)
        {
            Position = position;
            Delta = delta;
            StorageOffset = storageOffset;

            StackStack = stack_stack;
            Active = false;

            IP_ParentID = parent_id;

            if (willIncrementCounter == false)
            {
                ID = -1;
            }
            else
            {
                ID = ID_Counter;
                ID_Counter++;
            }
            StringMode = false;

			LoadedFingerprints = new List<Fingerprint>();
			LoadedFingerprints.Add(new Instructions.Fingerprints.NULL.NULL());
			LoadedFingerprints.First().Load();
        }

        //For use with Funge-98C
        public IP(IP parent)
        {
            this.Position = parent.Position;
            this.Delta = parent.Delta;
            this.StorageOffset = parent.StorageOffset;
            
            this.StackStack = new Stack<Stack<int>>();
            //Copy the exact contents and order of the stack stack
            for (int stackNumber = parent.StackStack.Count - 1; stackNumber >= 0; stackNumber--)
            {
                this.StackStack.Push(new Stack<int>());
                Stack<int> currentStack = parent.StackStack.ElementAt(stackNumber);
                for (int i = currentStack.Count - 1; i >= 0; i--)
                {
                    this.StackStack.Peek().Push(currentStack.ElementAt(i));
                }
            }
            Active = false;

            this.IP_ParentID = parent.IP_ParentID;
            this.ID = ID_Counter;
            ID_Counter++;

            StringMode = false;

			LoadedFingerprints = new List<Fingerprint>(parent.LoadedFingerprints);
        }

        public void Move()
        {
            this.Position = FungeSpaceUtils.MoveBy(this.Position, this.Delta);
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
			this.Delta = new Vector2(this.Delta.x, this.Delta.y);
        }

        public void Reset()
        {
            Position = Position.ParentMatrix.Origin;
            Delta = Vector2.East;
            StackStack = new Stack<Stack<int>>();

            Active = false;
            StringMode = false;
        }

        public void Stop()
        {
            Delta.Clear();
        }

        /// <summary>
        /// Get's the cell currently under the IP and returns it's value
        /// </summary>
        /// <returns></returns>
        public FungeCell GetCurrentCell()
        {
            return Position.Data;
        }

		public void CallFingerprintMember(char c)
		{
			if (c < 'A' || c > 'Z')
			{
				return;
			}

			//From the top to the bottom, check if the fingerprint has a member
			//Eventually it will reach NULL, and quit
			for (int i = LoadedFingerprints.Count(); i >= 0; --i)
			{
				Instruction inst = LoadedFingerprints.ElementAt(i).Members[c];
				if (inst != null)
				{
					inst.Preform(this);
					return;
				}
			}
		}
		//---------------------------------------------------------------------

        public override string ToString()
        {
            return "P: " + this.Position + ", D: " + this.Delta + ", TOSS:" + this.Stack + ", ID: " + this.ID;
        }
    }
}
