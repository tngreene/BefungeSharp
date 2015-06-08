using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.FileIO
{
    public abstract class FileIOInstruction : Instruction
    {
        public FileIOInstruction(char inName, int minimum_flags) : base(inName, CommandType.FileIO, ConsoleColor.Gray, minimum_flags) { }
    }

    public class InputFileInstruction : FileIOInstruction, IRequiresPop, IFungeSpaceAltering
    {
        public InputFileInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //Pop the necissary parameters
            string filename = StackUtils.StringPop(ip.Stack);
            int flags = ip.Stack.PopOrDefault();
            Vector2 Va = StackUtils.VectorPop(ip.Stack);
            
            //Attempt to open a file and its contents
            List<List<int>> openedFile = FileUtils.ReadFile(filename, (flags & 1) == 1,
                Program.Interpreter.CurMode != BoardMode.Run_TERMINAL);//Supress messages when not in termainal mode
            
            if (openedFile == null)
            {
                InstructionManager.InstructionSet['r'].Preform(ip);
            }
            else
            {
                FungeSpace.FungeSparseMatrix overlay_matrix = new FungeSpace.FungeSparseMatrix(openedFile);
                FungeSpace.FungeSpaceUtils.OverlayMatrix(ip.Position.ParentMatrix, overlay_matrix, Va);

                Vector2[] worldBounds = FungeSpace.FungeSpaceUtils.GetRealWorldBounds(overlay_matrix);
                Vector2 Vb = Va + worldBounds[1];

                StackUtils.VectorPush(ip.Stack, Va);
                StackUtils.VectorPush(ip.Stack, Vb);
            }
            return true;
        }

        public int RequiredCells()
        {
            throw new NotImplementedException();
        }
    }

    public class OutputFileInstruction : FileIOInstruction, IRequiresPop
    {
        public OutputFileInstruction(char inName, int minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }
}
