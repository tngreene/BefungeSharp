using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints.ROMA
{
	public class ROMA_Instruction : FingerprintInstruction
	{
		public int Value { get; private set; }

		public ROMA_Instruction(char inName, string description, int value)
			: base(inName, RuntimeFeatures.UF, CommandType.Number, "ROMA", description)
		{
			Value = value;
		}
		
		public override bool Preform(IP ip)
		{
			ip.Stack.Push(Value);
			return true;
		}
	}
}
