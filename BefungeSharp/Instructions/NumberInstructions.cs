using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Number
{
    /// <summary>
    /// An instruction who adds a single number to the top of the stack of an ip
    /// </summary>
    public class NumberInstruction : Instruction
    {
        /// <summary>
        /// Represents the value that the number instruction adds to the top
        /// </summary>
        protected int value;

        /// <summary>
        /// An instruction who adds a single number to the top of the stack of an ip
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        /// <param name="value">The value this instruction adds onto the stack</param>
        public NumberInstruction(char inName, RuntimeFeatures minimum_flags, int value) : base (inName, CommandType.Number, minimum_flags)
        {
            this.value = value;
        }

        public override bool Preform(IP ip)
        {
            ip.Stack.Push(value);
            return true;
        }
    }
}
