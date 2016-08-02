using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints.HRTI
{
	//Original Author: Chris Pressy
	//Original Spec: https://github.com/catseye/Funge-98/blob/master/library/HRTI.markdown
	public class HRTI : Fingerprint
	{
		static internal Dictionary<int, long> MarkList { get; private set; }
		static internal System.Diagnostics.Stopwatch Timer { get; private set; }

		public HRTI()
			: base(FingerprintType.StaticTame, "HRTI", "High Resolution Timer Interface")
		{
			MarkList = new Dictionary<int, long>();
			Timer = new System.Diagnostics.Stopwatch();
		}

		public override void Load()
		{
			Timer.Start();
			Members['E'] = new EraseMarkInstruction  ('E', "Erase mark' erases the last timer mark by this IP (such that T above will act like r)");
			Members['G'] = new GranularityInstruction('G', "'Granularity' pushes the smallest clock tick the underlying system can reliably handle, measured in microseconds.");
			Members['M'] = new MarkInstruction       ('M', "'Mark' designates the timer as having been read by the IP with this ID at this instance in time.)");
			Members['S'] = new SecondInstruction     ('S', "'Second' pushes the number of microseconds elapsed since the last whole second.");
			Members['T'] = new TimerInstruction      ('T', "'Timer' pushes the number of microseconds elapsed since the last time an IP with this ID marked the timer. If there is no previous mark, acts like r.");
		}

		public override void Unload()
		{
			//Nothing special here...
		}
	}
}
