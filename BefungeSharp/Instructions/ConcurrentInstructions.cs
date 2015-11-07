using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Concurrent
{
    public abstract class ConcurrentInstruction : Instruction
    {
        public ConcurrentInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.Concurrent, minimum_flags) { }
    }

    public class SplitInstruction : ConcurrentInstruction
    {
        public SplitInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //A temporary reference to the new IP
            IP childIP = new IP(ip);

            //While it would be nice to not have to find out where in the list we are again
            //We currently don't have access to that kind of information
            //Fortunatly its not like this command will be running all the time

            //Find where the current ip is in the list of IP's
            for (int n = 0; n < Program.Interpreter.IPs.Count; n++)
			{
			    if(Program.Interpreter.IPs[n] == ip)
                {
                    //Insert after this one
                    Program.Interpreter.IPs.Insert(n + 1, childIP);
                    break;
                }
			}

            childIP.Active = true;
            childIP.Negate();
            return true;
        }
    }
}
