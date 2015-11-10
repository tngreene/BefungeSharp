using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BefungeSharp.FungeSpace;

namespace BefungeSharp.Instructions.FileIO
{
    public abstract class FileIOInstruction : Instruction
    {
        public FileIOInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.FileIO, minimum_flags) { }
    }

    public class InputFileInstruction : FileIOInstruction, IRequiresPop, IFungeSpaceAltering
    {
        public InputFileInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //Pop the necissary parameters
            string filename = StackUtils.StringPop(ip.Stack);
            int flags = ip.Stack.PopOrDefault();
            Vector2 Va = StackUtils.VectorPop(ip.Stack);
            
            //Attempt to open a file and its contents
            List<List<int>> openedFile = FileUtils.ReadFile(filename, (flags & 1) == 1,
                Program.Interpreter.CurMode != BoardMode.Run_TERMINAL, false);//Supress messages when not in termainal mode
            
            if (openedFile == null)
            {
                InstructionManager.InstructionSet['r'].Preform(ip);
            }
            else
            {
                FungeSparseMatrix overlay_matrix = new FungeSparseMatrix(openedFile);
                FungeSpaceUtils.OverlayMatrix(ip.Position.ParentMatrix, overlay_matrix, Va);

                Vector2[] worldBounds = FungeSpaceUtils.GetRealWorldBounds(overlay_matrix);
                Vector2 Vb = Va + worldBounds[1];

                StackUtils.VectorPush(ip.Stack, Va);
                StackUtils.VectorPush(ip.Stack, Vb);
            }
            return true;
        }

        public int RequiredCells()
        {
            //null string, flag, x, y
            return 4;
        }
    }

    
    public class OutputFileInstruction : FileIOInstruction, IRequiresPop
    {
        public OutputFileInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //Pop the necissary parameters
            string filename = StackUtils.StringPop(ip.Stack);
            
            int flags = ip.Stack.PopOrDefault();
            bool linearFile = (flags & 1) == 1;
            Vector2 Vb = StackUtils.VectorPop(ip.Stack);
            Vector2 Va = StackUtils.VectorPop(ip.Stack);
            
            List<List<int>> outArray = new List<List<int>>();
            
            if(linearFile == true)
            {
                outArray.Add(new List<int>());
                //FOR TESTING
                //outArray[0].AddRange(new int[] { '1', '2', '3', '4', ' ', '\t', ' ', ' ', ' ', ' ', '6', ' ', ' ', '\r', '\n', ' ', ' ', '\r', ' ', ' ', '\n', ' ', ' ' });
           
                //Enumerate over Va->Vb and all values to outArray[0]
                ip.Position.ParentMatrix.EnumerationArea = new FungeSpaceArea(Va,Vb);
                foreach (var node in ip.Position.ParentMatrix)
	            {
		            outArray[0].Add(node.Data.value);
	            }
                
                bool isGoodSpace = false;
                List<int> outputFile = new List<int>();

                //Starting from the end investigate if a space or other char should be added to the out array
                for (int i = outArray[0].Count - 1; i >= 0 ; i--)
			    {
                    int currentCell = outArray[0][i];  
                    if (currentCell == ' ')
                    {
                        if (isGoodSpace == false)
                        {
                            isGoodSpace = false;
                            continue;
                        }
                        if (isGoodSpace == true)
                        {
                            outputFile.Add(currentCell);
                            isGoodSpace = true;
                            continue;
                        }
                    }
                    else if (currentCell == '\r' || currentCell == '\n')
                    {
                        //If we were to encounter a space to the left of us it would be bad
                        isGoodSpace = false;
                    }
                    else
                    {
                        outputFile.Add(currentCell);
                        //If we were to encounter a space to the left of us it would be good
                        isGoodSpace = true;
                    }
			    }
                //Replace the data with our stripped version
                outArray[0] = outputFile;
            }
            else
            {
                outArray = FungeSpaceUtils.MatrixToDynamicArray(ip.Position.ParentMatrix, new Vector2[] {Va, Vb});
            }

            //Attempt to open a file and its contents
            bool success = FileUtils.WriteFile(filename, outArray,
                Program.Interpreter.CurMode != BoardMode.Run_TERMINAL, false); //Supress messages when not in termainal mode

            if (success == false)
            {
                InstructionManager.InstructionSet['r'].Preform(ip);
            }
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }
}
