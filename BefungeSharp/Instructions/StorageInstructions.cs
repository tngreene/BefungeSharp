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
        public StorageInstruction(char inName, int minimum_flags) : base(inName, CommandType.DataStorage, ConsoleColor.Green, minimum_flags) { }
    }

    public class GetInstruction : StorageInstruction, IRequiresPush, IRequiresPop
    {
        public GetInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());
            Vector2 v = StackUtils.VectorPop(ip.Stack);
            
            FungeSpace.FungeNode lookup = ip.Position.ParentMatrix.GetNode(v.y, v.x);
            
            if (lookup != null)
            {
                ip.Stack.Push(lookup.Data.value);
                return true;
            }
            else
            {
                ip.Stack.Push(' ');
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
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());

            Vector2 v = StackUtils.VectorPop(ip.Stack);
            
            int value = ip.Stack.Pop();
            ip.Position.ParentMatrix.InsertCell(v.x, v.y, value);

            return true;
        }

        public int RequiredCells()
        {
            return 3;
        }
    }
}
