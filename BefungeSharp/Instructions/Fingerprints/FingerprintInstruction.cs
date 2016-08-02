using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints
{
	public abstract class FingerprintInstruction : Instruction
	{
		/// <summary>
        /// A fingerprint instruction
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        /// <param name="value">The new delta which will be applied to the IP</param>
        public FingerprintInstruction(char inName, RuntimeFeatures minimum_flags, CommandType type, string parent_fp, string description) : base(inName, type, minimum_flags)
		{
			Description = description;
		}

		/// <summary>
		/// The short name of this fingerprint instructions parent fingerprint
		/// </summary>
		public string ParentFP { get; private set; }

		//Description for Fingerprint Instructions (excluding '(' and ')')
		public string Description { get; protected set; }
	}
	
	public class LoadSemantic : FingerprintInstruction
	{
		public LoadSemantic(char inName, RuntimeFeatures minimum_flags)
			: base(inName, minimum_flags, CommandType.Fingerprint, "", "")
		{
		}

		public override bool Preform(IP ip)
		{
			uint bitstring = Fingerprint.EncodeBitstring(ip.Stack);

			Fingerprint fp = Interpreter.IsFingerprintEnabled(bitstring);
			if (fp == null)
			{
				return Instructions.Instruction.MakeInstruction('r').Preform(ip);
			}
			else
			{
				fp.Load();
				ip.LoadedFingerprints.Insert(0,fp);
				ip.Stack.Push((int)bitstring);
				ip.Stack.Push(1);
			}
			return true;
		}
	}

	public class UnloadSemantic : FingerprintInstruction
	{
		public UnloadSemantic(char inName, RuntimeFeatures minimum_flags)
			: base(inName, minimum_flags, CommandType.Fingerprint, "", "")
		{
		}

		public override bool Preform(IP ip)
		{
			//Make sure you cannot pop off NULL
			if (ip.LoadedFingerprints.Count() == 1)
			{
				Instruction.MakeInstruction('r').Preform(ip);
			}
			else
			{
				uint bitstring = Fingerprint.EncodeBitstring(ip.Stack);
				for (int i = 0; i < ip.LoadedFingerprints.Count(); i++)
				{
					if (ip.LoadedFingerprints.ElementAt(i).Bitstring == bitstring)
					{
						ip.LoadedFingerprints.ElementAt(i).Unload();
						ip.LoadedFingerprints.RemoveAt(i);
					}
				}
			}

			return true;
		}
	}
}
