using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints.ROMA
{
	//Original Author: Chris Pressy
	//Original Spec: https://github.com/catseye/Funge-98/blob/master/library/ROMA.markdown
	public class ROMA : Fingerprint
	{
		public ROMA()
			: base(FingerprintType.Tame, "ROMA", "Members push their equvilant Roman numeral's value")
		{
		}

		public override void Load()
		{
			Members['C'] = new ROMA_Instruction('C', "Pushes 100 onto the stack",  100);
			Members['D'] = new ROMA_Instruction('D', "Pushes 500 onto the stack",  500);
			Members['I'] = new ROMA_Instruction('I', "Pushes 1 onto the stack",    1);
			Members['L'] = new ROMA_Instruction('L', "Pushes 50 onto the stack",   50);
			Members['M'] = new ROMA_Instruction('M', "Pushes 1000 onto the stack", 1000);
			Members['V'] = new ROMA_Instruction('V', "Pushes 5  onto the stack",   5);
			Members['X'] = new ROMA_Instruction('X', "Pushes 10 onto the stack",   10);
		}

		public override void Unload()
		{
			//Nothing special here
		}
	}
}
