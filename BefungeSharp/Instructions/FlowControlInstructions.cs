using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.FlowControl
{
    public abstract class FlowControlInstruction : Instruction
    {
        public FlowControlInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.FlowControl, minimum_flags) { }
    }

    public class TrampolineInstruction : FlowControlInstruction
    {
        public TrampolineInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            Vector2 nextPosition = ip.Position.Data + ip.Delta;
            //TODO: This is majorly slow, if we could figure out a way to make sure it uses MoveBy as much as possible it would
            //really help
            ip.Position = FungeSpace.FungeSpaceUtils.MoveTo(ip.Position, nextPosition.y, nextPosition.x);
            return true;
        }
    }

    public class StopInstruction : FlowControlInstruction
    {
        public StopInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            ip.Active = false;
            ip.Delta = Vector2.Zero;
            return true;
        }
    }

    public class JumpInstruction : FlowControlInstruction, IRequiresPop
    {
        public JumpInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());
            int cellsToMove = ip.Stack.Pop();

            //We copy the delta because we only want to move along it, not change to it
            Vector2 moveDelta = ip.Delta;
            if (cellsToMove < 0)
            {
                moveDelta.Negate();
            }

            int toMove = Math.Abs(cellsToMove);
            for (int i = 0; i < toMove; i++)
            {
                int nextX = ip.Position.Data.x + moveDelta.x;
                int nextY = ip.Position.Data.y + moveDelta.y;
                //Move using our special move delta
                ip.Position = FungeSpace.FungeSpaceUtils.MoveTo(ip.Position, nextY, nextX);
            }
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class QuitInstruction : FlowControlInstruction, IRequiresPop, IAffectsRunningMode
    {
        
        public QuitInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }
        public BoardMode NewMode
        {
            get { return BoardMode.Edit; }
        }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack,RequiredCells());
            Program.Interpreter.ChangeMode(this);
            
            //One day this will be useful
            //int return_value = ip.Stack.Pop();
            
            //For now, we only have returning true
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }


        
    }

    public class IterateInstruction : FlowControlInstruction, IRequiresPop
    {
        public IterateInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());

            int iterations = ip.Stack.Pop();

            int cmd = 0;
            
            //Get the next command we'll be executing
            {
                //Create a copy
                IP traverseIP = new IP(ip.Position, ip.Delta, ip.StorageOffset, ip.StackStack, ip.IP_ParentID, false);
                
                //Move to the next cell over
                traverseIP.Move();

                cmd = traverseIP.Position.Data.value;
                
                //Keep moving through ethereal space until we're out of it
                while (cmd == ';' || cmd == ' ')
                {
                    Instructions.InstructionManager.InstructionSet[cmd].Preform(traverseIP);
                    cmd = traverseIP.Position.Data.value;
                }
            }

            if (iterations < 0)
            {
                InstructionManager.InstructionSet['r'].Preform(ip);
                return true;
            }
            else if (iterations == 0)
            {
                InstructionManager.InstructionSet['#'].Preform(ip);
                return true;
            }
            else
            {
                if (cmd >= ' ' && cmd <= '~')
                {
                    Instruction executable = InstructionManager.InstructionSet[cmd];
                    for (int i = 0; i < iterations; i++)
                    {
                        executable.Preform(ip);
                    }
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
        public JumpOverInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            do
            {
                ip.Move();
            }
            while (ip.GetCurrentCell().value != ';');
            
            ip.Move();

            return true;
        }
    }
}
