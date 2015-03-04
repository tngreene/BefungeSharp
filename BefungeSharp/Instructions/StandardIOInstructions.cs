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

    public class InputCharacterInstruction : StandardIOInstruction
    {
        public InputCharacterInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            Console.SetCursorPosition(ip.Position.Data.x, ip.Position.Data.y);
            char charInput = Console.ReadKey(true).KeyChar;
            ip.Stack.Push((int)charInput);
            Program.WindowUI.AddText(charInput.ToString(), WindowUI.Categories.IN);
            return true;
        }
    }

    public class InputDecimalInstruction : StandardIOInstruction
    {
        public InputDecimalInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            Console.SetCursorPosition(ip.Position.Data.x, ip.Position.Data.y);
            string input = Console.ReadLine();
            int outResult = 0;
            bool succeded = int.TryParse(input, out outResult);
            if (succeded == true)
            {
                ip.Stack.Push(outResult);
                Program.WindowUI.AddText(input, WindowUI.Categories.IN);
            }
            else
            {
                ip.Stack.Push(0);
                Program.WindowUI.AddText("0", WindowUI.Categories.IN);
            }
            
            return true;
        }
    }

    public class OutputCharacterInstruction : StandardIOInstruction, IRequiresPop
    {
        public OutputCharacterInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

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
