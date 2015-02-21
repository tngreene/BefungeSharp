using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.StdIO
{
    public abstract class StandardIOInstruction : Instruction
    {
        public StandardIOInstruction(char inName, int minimum_flags) : base(inName, CommandType.StdIO, ConsoleColor.DarkGray, minimum_flags) { }
    }

    public class OutputDecimalInstruction : StandardIOInstruction, IRequiresPop
    {
        public OutputDecimalInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

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
