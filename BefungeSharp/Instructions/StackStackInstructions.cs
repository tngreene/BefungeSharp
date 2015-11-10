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
        public StackStackInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.StackStackManipulation, minimum_flags) { }
    }

    public class BeginBlockInstruction : StackStackInstruction, IRequiresPop
    {
        /// <summary>
        /// The Begin Block instruction, adds a new Stack on the StackStack,
        /// changes the newly designated SOSS, and changes the IP's Storage Offset
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public BeginBlockInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            //1.) Pop n off the current TOSS (soon to be the SOSS)
            //2.) Push a new Stack on the StackStack
            //3.) Handle cases for n
            //  a.) n >  0: Move n cells from the SOSS to the new TOSS. If SOSS.Count < n, fill behind the top values with 0's
            //  b.) n == 0: No cells tranfered
            //  c.) n <  0: |n| are pushed onto the SOSS
            //4.) Push the IP's storage offset as a vector onto the SOSS
            //5.) Set the IP's storage offset to the position + delta
            int n = ip.Stack.PopOrDefault();

            ip.StackStack.Push(new Stack<int>());
            Stack<int> TOSS = ip.StackStack.ElementAt(0);
            Stack<int> SOSS = ip.StackStack.ElementAt(1);
            
            if (Math.Sign(n) == 1)// || Math.Sign(n) == 0) //Case n == 0 is taken care of implicitly
            {
                //Transfer from the SOSS to the new TOSS
                StackUtils.TransferCells(SOSS, TOSS, n, false, true);
            }
            else if(Math.Sign(n) == -1)
            {
                //Add n 0's onto the top of the SOSS
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
        /// The End Block Instruction, reverses the Begin Block Instruction
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public EndBlockInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //1.) Pop n off the current TOSS (soon to be deleted)
            //2.) Set the IP's storage offset to a vector popped off the SOSS
            //3.) Handle cases for n
            //  a.) n >  0: Move n cells from the TOSS to the SOSS. if TOSS.Count < n, fill behind the top values with 0's
            //  b.) n == 0: No cells tranfered
            //  c.) n <  0: |n| are popped off the SOSS
            
            if (ip.StackStack.Count == 1)
            {
                InstructionManager.InstructionSet['r'].Preform(ip);
                return true;
            }
            Stack<int> TOSS = ip.StackStack.ElementAt(0);
            Stack<int> SOSS = ip.StackStack.ElementAt(1);

            int n = TOSS.PopOrDefault();
            ip.StorageOffset = StackUtils.VectorPop(SOSS);

            if (Math.Sign(n) == 1 || Math.Sign(n) == 0)
            {
                //Transfer from the TOSS to the new SOSS
                StackUtils.TransferCells(TOSS, SOSS, n, false, true);
            }
            else if (Math.Sign(n) == -1)
            {
                //Removes n 0's from the SOSS
                for (int i = 0; i < Math.Abs(n); i++)
                {
                    SOSS.PopOrDefault();
                }
            }

            ip.StackStack.PopOrDefault();
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class StackUnderStackInstruction : StackStackInstruction, IRequiresPop
    {
        /// <summary>
        /// The StackUnderStack instruction, transers cells between the TOSS and SOSS, or reflects in the case of underflow
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public StackUnderStackInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //1.) If StackStack.Count == 1, reflect
            if(ip.StackStack.Count == 1)
            {
                Instructions.InstructionManager.InstructionSet['r'].Preform(ip);
                return true;
            }
            
            //2.) Pop a count off the TOSS, called n
            int n = ip.Stack.PopOrDefault();
            int sign = Math.Sign(n);
            n = Math.Abs(n);

            //3.) Handle cases for n
            //  a.) n >  0: Move n cells from SOSS to TOSS. if SOSS.Count < n, fill in front of the top values with 0's
            //  b.) n == 0: No cells tranfered
            //  c.) n <  0: Move |n| cells from the TOSS to the SOSS. if TOSS.Count < n, fill in front of the top values with 0's
            if (sign == 1)
            {
                //If positive, transfering from SOSS [1] to TOSS [0]
                StackUtils.TransferCells(ip.StackStack.ElementAt(1), ip.StackStack.ElementAt(0), n, true, true);
            }
            else if (sign == -1)
            {
                //If negative, transfering from TOSS [0] to SOSS [1]
                StackUtils.TransferCells(ip.StackStack.ElementAt(0), ip.StackStack.ElementAt(1), n, true, true);
            }

            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }
}
