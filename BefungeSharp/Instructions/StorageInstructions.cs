using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Storage
{
    public abstract class StorageInstruction : Instruction
    {
        protected BoardManager manager;
        /// <summary>
        /// An instruction which access and changes the storage space of funge space
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public StorageInstruction(char inName, int minimum_flags) : base(inName, CommandType.IO, ConsoleColor.Green, minimum_flags) { }
    }

    public class GetInstruction : StorageInstruction, IRequiresPush, IRequiresPop
    {
        public GetInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, RequiredCells());
            Vector2 v = StackUtils.VectorPop(ip.Stack);
            char foundChar = Program.BoardManager.GetCharacter(v.y, v.x);

            if (CanPushCells() == true)
            {
                ip.Stack.Push((int)foundChar);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanPushCells()
        {
            return true;
        }

        public int RequiredCells()
        {
            return 2;
        }
    }

    public class PutInstruction : StorageInstruction, IRequiresPop
    {
        public PutInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, RequiredCells());

            Vector2 v = StackUtils.VectorPop(ip.Stack);
            
            int charToPlace = ip.Stack.Pop();
            bool couldPlace = Program.BoardManager.PutCharacter(v.y, v.x, (char)charToPlace);

            return couldPlace;
        }

        public int RequiredCells()
        {
            return 3;
        }
    }
}
