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
			int count = ip.Stack.Pop();

			string short_name = "";
			for (int i = 0; i < count; i++)
			{
				short_name += (char)ip.Stack.Pop();
			}

			//TODO: For when we have a Fingerprint w/o a name
			//int bitstring = Fingerprint.GenerateBitstring(short_name);
			//bool enabled = Interpreter.IsFingerprintEnabled(bitstring);
			
			Fingerprint fp = Interpreter.IsFingerprintEnabled(short_name);
			if (fp == null)
			{
				new Delta.ReverseDeltaInstruction('r', 0).Preform(ip);
			}
			else
			{
				fp.Load();
				ip.LoadedFingerprints.Push(fp);
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
			throw new NotImplementedException();
		}
	}
}
