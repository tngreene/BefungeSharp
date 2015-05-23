using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.StackStack
{
    public abstract class StackStackInstruction : Instruction
    {
        /// <summary>
        /// An instruction which changes the stack of an IP
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public StackStackInstruction(char inName, int minimum_flags) : base(inName, CommandType.StackStackManipulation, ConsoleColor.Yellow, minimum_flags) { }
    }

    /// <summary>
    /// The duplication instruction, duplicates the top value on the stack
    /// </summary>
    public class BeginBlockInstruction : StackStackInstruction, IRequiresPop
    {
        /// <summary>
        /// The duplication instruction, duplicates the top value on the stack
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public BeginBlockInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            int n = ip.Stack.PopOrDefault();
            Stack<int> SOSS = ip.Stack;
            
            ip.StackStack.Push(new Stack<int>());
            Stack<int> TOSS = ip.Stack;
            
            if (Math.Sign(n) == 1 || Math.Sign(n) == 0)
            {
                //Transfer from the SOSS to the new TOSS
                StackUtils.TransferCells(SOSS, TOSS, n, false, false);
            }
            else if(Math.Sign(n) == -1)
            {
                //Add n 0's onto the top of the SOSS. The new Stack<int>() is a trick for code reuse
                for (int i = 0; i < Math.Abs(n); i++)
                {
                    SOSS.Push(0);
                }
            }
            
            StackUtils.VectorPush(SOSS, ip.StorageOffset);
            ip.StorageOffset = FungeSpace.FungeSpaceUtils.MoveBy(ip.Position, ip.Delta).Data;
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class EndBlockInstruction : StackStackInstruction, IRequiresPop
    {
        /// <summary>
        /// The duplication instruction, duplicates the top value on the stack
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public EndBlockInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {   
            Stack<int> TOSS = ip.StackStack.ElementAt(0);
            Stack<int> SOSS = ip.StackStack.ElementAt(1);

            int n = TOSS.PopOrDefault();
            ip.StorageOffset = StackUtils.VectorPop(SOSS);

            if (Math.Sign(n) == 1 || Math.Sign(n) == 0)
            {
                //Transfer from the SOSS to the new TOSS
                StackUtils.TransferCells(TOSS, SOSS, n, false, false);
            }
            else if (Math.Sign(n) == -1)
            {
                //Add n 0's onto the top of the SOSS. The new Stack<int>() is a trick for code reuse
                for (int i = 0; i < Math.Abs(n); i++)
                {
                    SOSS.PopOrDefault();
                }
            }

            if (ip.StackStack.Count < 2)
            {
                InstructionManager.InstructionSet['r'].Preform(ip);
            }
            else
            {
                ip.StackStack.PopOrDefault();
            }
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class StackUnderStackInstruction : StackStackInstruction, IRequiresPop
    {
        public StackUnderStackInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            if(ip.StackStack.Count >= 2)
            {
                Instructions.InstructionManager.InstructionSet['r'].Preform(ip);
                return true;
            }

            int count = ip.Stack.PopOrDefault();
            int sign = Math.Sign(count);
            count = Math.Abs(count);

            if (sign == 1)
            {
                //If positive, transfering from SOSS [1] to TOSS [0]
                StackUtils.TransferCells(ip.StackStack.ElementAt(1), ip.StackStack.ElementAt(0), count, true, false);
            }
            else if (sign == -1)
            {
                //If negative, transfering from TOSS [0] to SOSS [1]
                StackUtils.TransferCells(ip.StackStack.ElementAt(0), ip.StackStack.ElementAt(1), count, true, false);
            }

            //Else, nothing happens
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }
}
