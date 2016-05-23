using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints.NULL
{
	public class NULL_Instruction : FingerprintInstruction
	{
		public NULL_Instruction(char inName)
			: base(inName, RuntimeFeatures.UF, CommandType.Fingerprint, "NULL", "Reverses IP's delta")
		{

		}

		public override bool Preform(IP ip)
		{
			new Instructions.Delta.ReverseDeltaInstruction('r', RuntimeFeatures.UF).Preform(ip);
			return true;
		}
	}
}
