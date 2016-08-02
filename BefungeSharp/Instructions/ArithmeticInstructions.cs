using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Arithmetic
{
    /*Arithmetic
     *  pop b, pop a, a OP b 
        case '+':
        case '-'://Subtract a-b       
        case '*':
        case '/'://Divide a/b                        
        case '%'://modulous a % b
    */
    public abstract class ArithmeticInstruction : Instruction, IRequiresPop
    {
        public ArithmeticInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.Arithmetic, minimum_flags) { }

        /// <summary>
        /// Implements IStackAltering
        /// </summary>
        /// <returns>The number of required cells for the operation to work</returns>
        public int RequiredCells()
        {
            return 2;
        }
    }

    /// <summary>
    /// The add instruction, pops a and b then puts a + b onto the stack
    /// </summary>
    public class AddInstruction : ArithmeticInstruction
    {
        public AddInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, this.RequiredCells());
            ip.Stack.Push(ip.Stack.Pop() + ip.Stack.Pop());
            return true;
        }
    }

    /// <summary>
    /// The subtract instruction, pops a and b then puts b - a onto the stack
    /// </summary>
    public class SubtractInstruction : ArithmeticInstruction
    {
        public SubtractInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, this.RequiredCells());
            int b = ip.Stack.Pop();
            int a = ip.Stack.Pop();
            ip.Stack.Push(a - b);
            return true;
        }
    }

    /// <summary>
    /// The Multiplyinstruction, pops a and b then puts a * b onto the stack
    /// </summary>
    public class MultiplyInstruction : ArithmeticInstruction
    {
        public MultiplyInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, this.RequiredCells());
            ip.Stack.Push(ip.Stack.Pop() * ip.Stack.Pop());
            return true;
        }
    }

    /// <summary>
    /// The divide instruction, pops a and b then puts b/a, integer division only, onto the stack
    /// </summary>
    public class DivideInstruction : ArithmeticInstruction
    {
        public DivideInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, this.RequiredCells());
            int b = ip.Stack.Pop();
            int a = ip.Stack.Pop();
            //TODO - does this follow this "the / "Divide" instruction, which pops two values, divides the second by the first using integer division, and pushes the result (note that division by zero produces a result of zero in Funge-98, but Befunge-93 instead is supposed to ask the user what they want the result of the division to be); and"
            if (b != 0)
            {
                int result = a / b;
                ip.Stack.Push(result);
            }
            else
            {
                ip.Stack.Push(0);
            }
            return true;
        }
    }

    /// <summary>
    /// The modulo instruction, pops a and b then puts b % a onto the stack
    /// </summary>
    public class ModuloInstruction : ArithmeticInstruction
    {
        public ModuloInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, this.RequiredCells());

            int b = ip.Stack.Pop();
            int a = ip.Stack.Pop();

            //TODO does this follow the spec?the % "Remainder" instruction, which pops two values, divides the second by the first using integer division, and pushes the remainder, of those. Remainder by zero is subject to the same rules as division by zero, but if either argument is negative, the result is implementation-defined.
            if (b != 0)
            {
                int result = a % b;
                ip.Stack.Push(result);
            }
            else
            {
                ip.Stack.Push(0);
            }
            return true;
        }
    }
}