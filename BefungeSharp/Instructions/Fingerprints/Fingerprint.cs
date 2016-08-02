using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Fingerprints
{
	public enum FingerprintType
	{
		/// <summary>
		/// Copy per IP, highly integrated with interpreter or IP
		/// </summary>
		Feral,

		/// <summary>
		/// One copy per interpret, non-integrated with interpreter or IP
		/// </summary>
		StaticTame,

		/// <summary>
		/// One copy per interpret, highly integrated with interpreter or IP
		/// </summary>
		StaticFeral,

		/// <summary>
		/// Copy per IP, non-integrated with interpreter or IP
		/// </summary>
		Tame
	}

	public abstract class Fingerprint
	{
		/// <summary>
		/// Bitstring of fingerprint such as 0x54555254 or 0x48525449
		/// </summary>
		public uint Bitstring { get; private set; }

		/// <summary>
		/// Long name such as "Simple Turtle Graphics Library" or "High Resolution Time Interface"
		/// </summary>
		public string LongName { get; private set; }

		/// <summary>
		/// Short abrivated name such as "TURT" or HRTI
		/// </summary>
		public string ShortName { get; private set; }

		/// <summary>
		/// The behavior of the fingerprint, TODO is this needed?
		/// </summary>
		public FingerprintType Type { get; private set; }

		/// <summary>
		/// The instructions used in the fingerprint
		/// </summary>
		public Dictionary<char, Instruction> Members { get; private set; }

		/// <summary>
		/// A definition of a fingerprint
		/// </summary>
		/// <param name="type">The type of fingerprint this is</param>
		/// <param name="long_name">The long description of fingerprint</param>
		/// <param name="short_name">The short description of fingerprint</param>
		/// <param name="bitstring">An optional bitstring if there is no short name</param>
		public Fingerprint(FingerprintType type, string short_name="", string long_name="", uint bitstring=0)
		{
			Type = type;
			ShortName = short_name;
			LongName  = long_name;

			Bitstring = bitstring == 0 ? EncodeBitstring(short_name) : bitstring;

			Members = new Dictionary<char, Instruction>(26);
			for (char c = 'A'; c < 'Z'; c++)
			{
				Members.Add(c, null);
			}
		}

		/// <summary>
		/// Preform load function
		/// </summary>
		public abstract void Load();

		/// <summary>
		/// Preform unload function
		/// </summary>
		public abstract void Unload();

		public static uint EncodeBitstring(Stack<int> stack)
		{
			int count = stack.Pop();
			uint bitstring = 0;
			while(count > 0)
			{
				bitstring = (bitstring << 8) + (uint)stack.PopOrDefault();
				count--;
			}
			
			return bitstring;
		}

		public static uint EncodeBitstring(string str)
		{
			uint bitstring = 0;
			for (int i = 0; i < str.Count(); i++)
			{
				bitstring = (bitstring << 8) + str[i];
			}

			return bitstring;
		}

		public static string DecodeBitstring(uint bitstring)
		{
			string decoded_string = "";
			uint mask = 0xFF000000;
			int bytes_remaining = 4;
			while (bytes_remaining > 0)
			{
				uint masked_bits = mask & bitstring;
				char c = (char)(masked_bits >> (bytes_remaining - 1) * 8);

				decoded_string += c;
				mask >>= 8;

				--bytes_remaining;
			}

			return decoded_string;
		}

		/// <summary>
		/// Print all members and a brief description of them
		/// </summary>
		public void PrintMembers()
		{
			Console.WriteLine("Loaded members of {0}: Count: {1}", ShortName, Members.Count);
			foreach (var entry in Members)
			{
				Console.WriteLine("Name: {0}", entry.Key);
			}
		}
	}
}
