using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BefungeSharp.Instructions.System
{
    public abstract class SystemInstruction : Instruction
    {
        public SystemInstruction(char inName, UInt32 minimum_flags) : base(inName, CommandType.IO, ConsoleColor.DarkMagenta, minimum_flags) { }
    }

    public class ExecuteInstruction : SystemInstruction, IRequiresPop
    {
        public ExecuteInstruction(char inName, UInt32 minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            base.EnsureStackSafety(ip.Stack, RequiredCells());
            
            //Pop a command that will be fed into cmd, /k forces the window to stay open
            string command = "/k " + StackUtils.PopString(ip.Stack);

            //start the cmd.exe process
            Process cmd = Process.Start("cmd.exe", command);
            //Pause the execution of this program to wait
            cmd.WaitForExit();
            //Check the exit code of the commandline
            if (cmd.ExitCode != 0)
            {
                ip.Negate();
                ip.Stack.Push(cmd.ExitCode);
                return false;
            }
            ip.Stack.Push(0);
            return true;
        }

        public int RequiredCells()
        {
            //Requires to pop atleast a 0, aka null string
            return 1;
        }
    }
}
