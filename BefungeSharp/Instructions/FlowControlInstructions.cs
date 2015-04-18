using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.FlowControl
{
    public abstract class FlowControlInstruction : Instruction
    {
        public FlowControlInstruction(char inName, int minimum_flags) : base(inName, CommandType.FlowControl, ConsoleColor.Cyan, minimum_flags) { }
    }

    public class TrampolineInstruction : FlowControlInstruction
    {
        public TrampolineInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            //Get the bounds because we'll be testing if we're about to go over the left or top edge
            Vector2[] bounds = FungeSpace.FungeSpaceUtils.GetRealWorldBounds(ip.Position.ParentMatrix);

            if (ip.Position.Data.y == bounds[0].y && ip.Delta == Vector2.North)
            {
                return true;
            }

            if (ip.Position.Data.x == bounds[0].x && ip.Delta == Vector2.West)
            {
                return true;
            }

            //Only move if we aren't about about to jump over an edge
            ip.Move();
            return true;
        }
    }

    public class StopInstruction : FlowControlInstruction
    {
        public StopInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            ip.Active = false;
            ip.Delta = Vector2.Zero;
            return true;
        }
    }

    public class JumpInstruction : FlowControlInstruction, IRequiresPop
    {
        public JumpInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

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
        
        public QuitInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }
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
        public IterateInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());

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
                if (temporaryIP.Position.Data.value >= ' ' && temporaryIP.Position.Data.value <= '~')
                {
                    Instruction executable = InstructionManager.InstructionSet[temporaryIP.Position.Data.value];
                    for (int i = 0; i < iterations; i++)
                    {
                        executable.Preform(temporaryIP);
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
        public JumpOverInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

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
