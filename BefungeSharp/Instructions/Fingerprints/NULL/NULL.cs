using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints.NULL
{
	//Original Author: Chris Pressy
	//Original Spec: https://github.com/catseye/Funge-98/blob/master/library/NULL.markdown
	public class NULL : Fingerprint
	{
		public NULL() : base(FingerprintType.Tame, "NULL", "NULL")
		{
		}

		public override void Load()
		{
			for (char c = 'A'; c <= 'Z'; ++c)
			{
				Members[c] = new NULL_Instruction(c);
			}
		}

		public override void Unload()
		{
			//Nothing special here needed
		}
	}
}
