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


}
