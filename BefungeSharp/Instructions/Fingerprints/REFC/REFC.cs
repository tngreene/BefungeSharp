using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints.REFC
{
	//Original Author: Chris Pressy
	//Original Spec: https://github.com/catseye/Funge-98/blob/master/library/HRTI.markdown
	public class REFC : Fingerprint
	{
		static internal List<Vector2> VectorList { get; private set; }

		public REFC()
			: base(FingerprintType.StaticTame, "REFC", "Referenced Cells Extension")
		{
			VectorList = new List<Vector2>();
		}

		public override void Load()
		{
			Members['R'] = new ReferenceInstruction('R', "'Reference' pops a vector off the stack, and pushes a scalar value back onto the stack, unique within an internal list of references, which refers to that vector.");
			Members['D'] = new DereferenceInstruction('D', "'Dereference' pops a scalar value off the stack, and pushes the vector back onto the stack which corresponds to that unique reference value.");
		}

		public override void Unload()
		{
			//Nothing special here...
		}
	}
}
