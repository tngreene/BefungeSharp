using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BefungeSharp.Instructions.Fingerprints.HRTI
{
	public abstract class HRTI_Instruction : FingerprintInstruction
	{
		protected static long CurrentMicroseconds { get { return (long)(Math.Floor(Stopwatch.Frequency / 1000000.0) * HRTI.Timer.ElapsedTicks); } }

		public HRTI_Instruction(char inName, string description)
			: base(inName, RuntimeFeatures.UF, CommandType.Fingerprint, "HRTI", description)
		{
		}
	}

	public class GranularityInstruction : HRTI_Instruction
	{
		public GranularityInstruction(char inName, string description)
			: base(inName, description)
		{
		}

		public override bool Preform(IP ip)
		{
			ip.Stack.Push((int)Math.Floor((double)(Stopwatch.Frequency / (1000000))));
			return true;
		}
	}

	public class MarkInstruction : HRTI_Instruction
	{
		public MarkInstruction(char inName, string description)
			: base(inName, description)
		{
		}

		public override bool Preform(IP ip)
		{
			//If ip has never been marked yet
			if (HRTI.MarkList.ContainsKey(ip.ID) == true)
			{
				HRTI.MarkList[ip.ID] = CurrentMicroseconds;
			}
			else
			{
				HRTI.MarkList.Add(ip.ID, CurrentMicroseconds);
			}

			return true;
		}
	}

	public class TimerInstruction : HRTI_Instruction
	{
		public TimerInstruction(char inName, string description)
			: base(inName, description)
		{
		}

		public override bool Preform(IP ip)
		{
			if (HRTI.MarkList.ContainsKey(ip.ID) == true)
			{
				ip.Stack.Push((int)(CurrentMicroseconds - HRTI.MarkList[ip.ID]));
			}
			else
			{
				Instruction.MakeInstruction('r').Preform(ip);
			}

			return true;
		}
	}

	public class EraseMarkInstruction : HRTI_Instruction
	{
		public EraseMarkInstruction(char inName, string description)
			: base(inName, description)
		{
		}

		public override bool Preform(IP ip)
		{
			if (HRTI.MarkList.ContainsKey(ip.ID) == true)
			{
				HRTI.MarkList.Remove(ip.ID);
			}

			return true;
		}
	}

	public class SecondInstruction : HRTI_Instruction
	{
		public SecondInstruction(char inName, string description)
			: base(inName, description)
		{
		}

		public override bool Preform(IP ip)
		{
			long current_time = HRTI.Timer.ElapsedTicks;
			current_time %= Stopwatch.Frequency;

			ip.Stack.Push((int)((HRTI.Timer.Elapsed.Seconds) * (1000000000L/Stopwatch.Frequency)));
			return true;
		}
	}
}
