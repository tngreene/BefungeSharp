using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Stack
{
    public abstract class StackInstruction : Instruction
    {
        /// <summary>
        /// An instruction which changes the stack of an IP
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public StackInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.StackManipulation, minimum_flags) { }
    }

    /// <summary>
    /// The duplication instruction, duplicates the top value on the stack
    /// </summary>
    public class DuplicateInstruction : StackInstruction, IRequiresPush, IRequiresPeek
    {
        /// <summary>
        /// The duplication instruction, duplicates the top value on the stack
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public DuplicateInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }
        
        public bool CanPushCells()
        {
            return true;
        }

        public override bool Preform(IP ip)
        {
            if (CanPeek(ip.Stack))
            {
                ip.Stack.Push(ip.Stack.Peek());
                return true;
            }
            else
            {
                ip.Stack.Push(0);
                return false;
            }
        }

        public bool CanPeek(Stack<int> stack)
        {
            return stack.Count > 0 ? true : false;
        }
    }

    /// <summary>
    /// The pop instruction, pops the top value off and discards it
    /// </summary>
    public class PopInstruction : StackInstruction, IRequiresPop
    {
        /// <summary>
        /// The pop instruction, pops the top value off and discards it
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public PopInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public int RequiredCells()
        {
            return 1;
        }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack,RequiredCells());
            ip.Stack.Pop();
            return true;
        }
    }

    /// <summary>
    /// The swap instruction, swaps the first top two values on the stack
    /// </summary>
    public class SwapInstruction : StackInstruction, IRequiresPush, IRequiresPop
    {
        public SwapInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, this.RequiredCells());
            int b = ip.Stack.Pop();
            int a = ip.Stack.Pop();

            ip.Stack.Push(b);
            ip.Stack.Push(a);//Now a is on top
            
            return true;
        }

        public int RequiredCells()
        {
            return 2;
        }
    
        public bool CanPushCells()
        {
            //Since we always know we pop two off first then pushing will implicitly always be okay
 	        return true;
        }
    }

    /// <summary>
    /// The clear stack instruction, clears the entire stack
    /// </summary>
    public class ClearStackInstruction : StackInstruction, IRequiresPop
    {
        protected int stackSize;
        /// <summary>
        /// The clear stack instruction, pops the top value off and discards it
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public ClearStackInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { stackSize = 0; }

        public int RequiredCells()
        {
            return stackSize;
        }

        public override bool Preform(IP ip)
        {
            //This extra stack safety business is not really necissary because
            //.NET's Clear() takes care of it for you but it is included to
            //keep the code inline with the rest of it all
            stackSize = ip.Stack.Count;
            StackUtils.EnsureStackSafety(ip.Stack,RequiredCells());
            
            ip.Stack.Clear();
            stackSize = 0;
            return true;
        }
    }
}
