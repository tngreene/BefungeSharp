using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.String
{
    public abstract class StringInstruction : Instruction
    {
        public StringInstruction(char inName, UInt32 minimum_flags) : base(inName, CommandType.Nop, ConsoleColor.DarkYellow, minimum_flags) { }
    }

    public class ToggleStringModeInstruction : StringInstruction
    {
        public ToggleStringModeInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //Negates and assaigns, a fancy toggle
            ip.StringMode = !ip.StringMode;
            return true;
        }
    }

    public class FetchCharacterInstruction : StringInstruction
    {
        public FetchCharacterInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            ip.Move();
            ip.Stack.Push(ip.GetCurrentCell());
            //The IP will be moved again when the instruction is finised calling
            return true;
        }
    }

    public class StoreCharacterInstruction : StringInstruction, IRequiresPop
    {
        public StoreCharacterInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            ip.Move();
            ip.Stack.Push(ip.Position.x);
            ip.Stack.Push(ip.Position.y);

            base.EnsureStackSafety(ip.Stack, RequiredCells());

            int y = ip.Stack.Pop();
            int x = ip.Stack.Pop();

            int charToPlace = ip.Stack.Pop();
            bool couldPlace = Program.GetBoardManager().PutCharacter(y, x, (char)charToPlace);
            //The IP will be moved again when the instruction is finised calling
            return true;
        }

        public int RequiredCells()
        {
            return 3;
        }
    }
}