using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Storage
{
    public abstract class StorageInstruction : Instruction
    {
        protected BoardManager manager;
        /// <summary>
        /// An instruction which access and changes the storage space of funge space
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public StorageInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.DataStorage, minimum_flags) { }
    }

    public class GetInstruction : StorageInstruction, IRequiresPush, IRequiresPop
    {
        public GetInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());
            Vector2 v = ip.StorageOffset + StackUtils.VectorPop(ip.Stack);

			//When we have a good way to stop the interpreter and display runtime messages to the user
			/*if(Program.Interpreter.SpecVersion == 93 &&
				Program.Interpreter.FS_93_Area.Contains(v.x, v.y) == true)
			{
				//Print warning
				"Out of Bounds Error: Cannot retrieve value in cell ({0},{1}). Please change spec version.", v.x, v.y);
				if(OptionsManager.Get<bool>("I","RT_STRICT_BF93") == true)
				{
					//Stop if strict mode
					Ending interpreter, set option RT_STRICT_BF93 to avoid."", 
				}
			}*/
			FungeSpace.FungeNode lookup = ip.Position.ParentMatrix.GetNode(v.y, v.x);
            
            if (lookup != null)
            {
                ip.Stack.Push(lookup.Data.value);
                return true;
            }
            else
            {
                ip.Stack.Push(' ');
                return false;
            }
        }

        public bool CanPushCells()
        {
            return true;
        }

        public int RequiredCells()
        {
            return 2;
        }
    }

    public class PutInstruction : StorageInstruction, IRequiresPop
    {
        public PutInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }
        
        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());

            Vector2 v = ip.StorageOffset + StackUtils.VectorPop(ip.Stack);

			//When we have a good way to stop the interpreter and display runtime messages to the user
			/*if(Program.Interpreter.SpecVersion == 93 &&
				Program.Interpreter.FS_93_Area.Contains(v.x, v.y) == true)
			{
				//Print warning
				"Out of Bounds Error: Cannot store {0} in cell ({1},{2}). Please change spec version.", ip.Stack.Peep(), v.x, v.y);
				if(OptionsManager.Get<bool>("I","RT_STRICT_BF93") == true)
				{
					//Stop if strict mode
					Ending interpreter, set option RT_STRICT_BF93 to avoid."", 
				}
			}*/

			int value = ip.Stack.Pop();
            ip.Position.ParentMatrix.InsertCell(v.x, v.y, value);

            return true;
        }

        public int RequiredCells()
        {
            return 3;
        }
    }
}
