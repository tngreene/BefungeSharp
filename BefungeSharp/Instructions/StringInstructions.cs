using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.String
{
    public abstract class StringInstruction : Instruction
    {
        public StringInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.String, minimum_flags) { }
    }

    public class ToggleStringModeInstruction : StringInstruction
    {
        public ToggleStringModeInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //Negates and assaigns, a fancy toggle
            ip.StringMode = !ip.StringMode;
            return true;
        }
    }

    public class FetchCharacterInstruction : StringInstruction
    {
        public FetchCharacterInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            Vector2 nextPosition = ip.Position.Data + ip.Delta;
            ip.Position = FungeSpace.FungeSpaceUtils.MoveTo(ip.Position, nextPosition.y, nextPosition.x);

            ip.Stack.Push(ip.GetCurrentCell().value);
            //The IP will be moved again when the instruction is finised calling
            return true;
        }
    }

    public class StoreCharacterInstruction : StringInstruction, IRequiresPop
    {
        public StoreCharacterInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            ip.Move();
            ip.Stack.Push(ip.Position.Data.x);
            ip.Stack.Push(ip.Position.Data.y);

            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());

            int y = ip.Stack.Pop();
            int x = ip.Stack.Pop();
            int value = ip.Stack.Pop();
            
            ip.Position.ParentMatrix.InsertCell(new FungeSpace.FungeCell(x, y, value));
            //The IP will be moved again when the instruction is finised calling
            return true;
        }

        public int RequiredCells()
        {
            return 3;
        }
    }
}