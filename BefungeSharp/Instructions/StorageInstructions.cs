using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Storage
{
    public abstract class StorageInstruction : Instruction
    {
        /// <summary>
        /// An instruction which access and changes the storage space of funge space
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public StorageInstruction(char inName, UInt32 minimum_flags) : base(inName, CommandType.IO, ConsoleColor.Green, minimum_flags) { }
    }

    public class GetInstruction : StorageInstruction, IRequiresPush, IRequiresPop
    {
        public GetInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip, BoardManager mgr = null)
        {
            base.EnsureStackSafety(ip.Stack, RequiredCells());
            int y = ip.Stack.Pop();
            int x = ip.Stack.Pop();
            char foundChar = mgr.GetCharacter(y, x);

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
        public PutInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip, BoardManager mgr = null)
        {
            base.EnsureStackSafety(ip.Stack, RequiredCells());
            
            int y = ip.Stack.Pop();
            int x = ip.Stack.Pop();
            
            int charToPlace = ip.Stack.Pop();
            bool couldPlace = mgr.PutCharacter(y, x, (char)charToPlace);
            
            return couldPlace;
        }

        public int RequiredCells()
        {
            return 3;
        }
    }
}
