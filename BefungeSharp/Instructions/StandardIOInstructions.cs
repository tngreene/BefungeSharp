using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.StdIO
{
    public abstract class StandardIOInstruction : Instruction
    {
        public StandardIOInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.StdIO, minimum_flags) { }
    }

    public class InputCharacterInstruction : StandardIOInstruction
    {
        public InputCharacterInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            ip.Stack.Push((int)Program.WindowUI.GetCharacter());
            
            return true;
        }
    }

    public class InputDecimalInstruction : StandardIOInstruction
    {
        public InputDecimalInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            ip.Stack.Push(Program.WindowUI.GetDecimal());
           
            return true;
        }
    }

    public class OutputCharacterInstruction : StandardIOInstruction, IRequiresPop
    {
        public OutputCharacterInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());
            
            //Convert to a character and then output it
            Program.WindowUI.AddText(((char)ip.Stack.Pop()).ToString(), WindowUI.Categories.OUT);
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class OutputDecimalInstruction : StandardIOInstruction, IRequiresPop
    {
        public OutputDecimalInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());
            Program.WindowUI.AddText(ip.Stack.Pop().ToString(), WindowUI.Categories.OUT);
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }
}
