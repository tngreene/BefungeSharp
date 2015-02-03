using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.FlowControl
{
    public abstract class FlowControlInstruction : Instruction
    {
        public FlowControlInstruction(char inName, UInt32 minimum_flags) : base(inName, CommandType.FlowControl, ConsoleColor.Cyan, minimum_flags) { }
    }

    public class TrampolineInstruction : FlowControlInstruction
    {
        public TrampolineInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            ip.Move();
            return true;
        }
    }

    public class StopInstruction : FlowControlInstruction
    {
        public StopInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            ip.Active = false;
            ip.Delta = Vector2.Zero;
            return true;
        }
    }

    public class JumpInstruction : FlowControlInstruction, IRequiresPop
    {
        public JumpInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, RequiredCells());
            int cellsToMove = ip.Stack.Pop();

            //We copy the delta because we only want to move along it, not change to it
            Vector2 moveDelta = ip.Delta;
            if (cellsToMove < 0)
            {
                moveDelta.Negate();
            }

            for (int i = 0; i < cellsToMove; i++)
            {
                //Move using our special move delta
                ip.Position += moveDelta;
            }
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class QuitInstruction : FlowControlInstruction, IRequiresPop
    {
        public QuitInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack,RequiredCells());

            //For now, we only have returning false or true
            return ip.Stack.Pop() == 0 ? false : true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class IterateInstruction : FlowControlInstruction, IRequiresPop
    {
        public IterateInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, RequiredCells());

            int iterations = ip.Stack.Pop();

            //Create a puppet of the current ip that has a reference to the ip's stack and doesn't increment the ip counter
            IP temporaryIP = new IP(ip.Position, ip.Delta, ip.StorageOffset, ip.Stack, ip.ID, false);

            //Move to the next non-space
            temporaryIP.Move();

            
            if (iterations < 0)
            {
                InstructionManager.InstructionSet['r'].Preform(ip);
            }
            else if (iterations == 0)
            {
                InstructionManager.InstructionSet['#'].Preform(ip);
            }
            else
            {
                Instruction executable = InstructionManager.InstructionSet[Program.GetBoardManager().GetCharacter(temporaryIP.Position.y, temporaryIP.Position.x)];
                for (int i = 0; i < iterations; i++)
                {
                    executable.Preform(temporaryIP);
                }
            }
            
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class JumpOverInstruction : FlowControlInstruction
    {
        public JumpOverInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            do
            {
                ip.Move();
            }
            while (ip.GetCurrentCell() != ';');
            ip.Move();

            return true;
        }
    }
}
