using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints.REFC
{
	public abstract class REFC_Instruction : FingerprintInstruction
	{
		public REFC_Instruction(char inName, string description)
			: base(inName, RuntimeFeatures.UF, CommandType.Fingerprint, "REFC", description)
		{
		}
	}

	public class ReferenceInstruction : REFC_Instruction
	{
		public ReferenceInstruction(char inName, string description)
			: base(inName, description)
		{
		}

		public override bool Preform(IP ip)
		{
			Vector2 vec = StackUtils.VectorPop(ip.Stack);
			REFC.VectorList.Add(vec);
			ip.Stack.Push(REFC.VectorList.Count - 1);
			return true;
		}
	}

	public class DereferenceInstruction : REFC_Instruction
	{
		public DereferenceInstruction(char inName, string description)
			: base(inName, description)
		{
		}

		public override bool Preform(IP ip)
		{
			int vec_number = ip.Stack.Pop();
			if(vec_number < 0 || vec_number > REFC.VectorList.Count)
			{
				Instruction.MakeInstruction('r').Preform(ip);
				return true;
			}
			else
			{
				StackUtils.VectorPush(ip.Stack, REFC.VectorList[vec_number]);
			}
			return true;
		}
	}
}
