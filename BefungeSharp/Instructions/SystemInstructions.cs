using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace BefungeSharp.Instructions.SystemCall
{
    public abstract class SystemInstruction : Instruction
    {
        public SystemInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.System, minimum_flags) { }
    }

    public class ExecuteInstruction : SystemInstruction, IRequiresPop
    {
        public ExecuteInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());
            
            //Pop a command that will be fed into cmd, /k forces the window to stay open
            string command = StackUtils.StringPop(ip.Stack,true);

            //Assume we've failed
            int exitCode = -1;
            
            Process cmd = new Process();
            
            //Try to open a process
            try
            {
                cmd = Process.Start("cmd.exe", "/k" + command);
                //Pause the execution of this program to wait
                cmd.WaitForExit();
                exitCode = cmd.ExitCode;
            }
            catch(Exception e)
            {
                //If it fails
                if (cmd.ExitCode != 0)
                {
                    ip.Negate();
                    ip.Stack.Push(cmd.ExitCode);
                    return false;
                }
            }
            
            ip.Stack.Push(0);
            return true;
        }

        public int RequiredCells()
        {
            //Requires to pop atleast a 0, aka null string
            return 1;
        }
    }

    public class GetSysInfo : SystemInstruction, IRequiresPop
    {
        public GetSysInfo(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            //Instead of putting all 20 values on the stack only to remove 19 or 20 of them we simply
            //imitate the end result and move on. If someone can tell me why the spec should be followed to the
            //letter please e-mail the author. They would greatly appreciate it.
            
            StackUtils.EnsureStackSafety(ip.Stack, RequiredCells());
            int initialTOSS_Size = ip.Stack.Count;

            //Which option we will start examining
            int toExamine = ip.Stack.Pop();

            //Start with assuming we're just interested in one of them
            int toTake = 1;

            //If it is less than or equal to zero we are taking the whole deal
            if (toExamine <= 0)
            {
                toTake = 20;
            }
            
            while (toTake > 0)
            {
                switch (toExamine)
				{
					#region 20. A series of strings containing the environment variables (global environment)
					case 20:
                        {
                            foreach (System.Collections.DictionaryEntry entry in System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine))
                            {
                                StackUtils.StringPush(ip.Stack, entry.Key.ToString().ToUpper() + "=" + entry.Value.ToString().ToUpper());
                            }

                            foreach (System.Collections.DictionaryEntry entry in System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process))
                            {
                                StackUtils.StringPush(ip.Stack, entry.Key.ToString().ToUpper() + "=" + entry.Value.ToString().ToUpper());
                            }

                            foreach (System.Collections.DictionaryEntry entry in System.Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User))
                            {
                                StackUtils.StringPush(ip.Stack, entry.Key.ToString().ToUpper() + "=" + entry.Value.ToString().ToUpper());
                            }
                        }
                        break;
					#endregion
					#region 19. A doubly null terminated sequence of strings containing the file name, followed by all of the commandline arguments passed into the interpreter. The string of strings is terminated with double null terminators (global environment)
					case 19:
                        
                        {
                            StackUtils.StringPush(ip.Stack, "Your File Name Here!");

                            //Get an array of arguments from the current process's Arguments split on every space
                            string[] arguments = Process.GetCurrentProcess().StartInfo.Arguments.Split(new char[] { ' ' });

                            for (int i = 0; i < arguments.Length; i++)
                            {
                                StackUtils.StringPush(ip.Stack, arguments[i]);
                            }
                            ip.Stack.Push(0);
                            ip.Stack.Push(0);
                        }
                        break;
					#endregion
                    #region 18. Size of each stack in the stack stack listed from TOSS to BOSS (ip specific)
                    case 18:
                        {
                            foreach (var stack in ip.StackStack.Reverse())
                            {
                                //If this is the TOSS
                                if (stack == ip.Stack)
                                {
                                    ip.Stack.Push(initialTOSS_Size);
                                }
                                else
                                {
                                    ip.Stack.Push(stack.Count);
                                }
                            }
                        }
                        break;
					#endregion
                    #region 17. Number of stacks on the stack stack (ip specific)
                    case 17:
                        {
                            ip.Stack.Push(ip.StackStack.Count);
                        }
                        break;
                    #endregion
					#region 16. The current hour, minute, and second (local environment)
					case 16:
                        
                        {
                            System.DateTime time = System.DateTime.Now;
                            int hours = time.Hour * 256 * 256;
                            int minutes = time.Minute * 256;
                            int seconds = time.Second;

                            ip.Stack.Push(hours + minutes + seconds);
                        }
                        break;
					#endregion
                    #region case 15. The current year, month, and day (local environment)
					case 15:
                        {
                            System.DateTime time = System.DateTime.Now;
                            int year = (time.Year - 1900) * 256 * 256;
                            int month = time.Month * 256;
                            int day = time.Day;

                            ip.Stack.Push(year + month + day);
                        }
                        break;
					#endregion
					#region case 14. A vector pointing to the greatest non-empty space relative to the least point (local environment)
					case 14:
                        //If you were to have a non-empty cell at 79, 24 this point is 0 + 79, 0 + 24 
                        {
							Vector2[] bounds = new Vector2[2];
							ip.Position.ParentMatrix.GetRealWorldBounds(ref bounds);
							StackUtils.VectorPush(ip.Stack, bounds[1]);
                        }
                        break;
					#endregion
					#region 13. A vector pointing to the least non-empty space relative to the origin (local environment)
					case 13:
                        {
							//TODO: What if dy were run in empty fungespace?
							//This is really a pendantic question for a future spec
							Vector2[] bounds = new Vector2[2];
                            ip.Position.ParentMatrix.GetRealWorldBounds(ref bounds);
                            StackUtils.VectorPush(ip.Stack, bounds[0]);
                        }
                        break;
					#endregion
					#region 12. A vector containing the storage offset of the ip (ip specific)
					case 12:
                        {
                            StackUtils.VectorPush(ip.Stack, ip.StorageOffset);
                        }
                        break;
					#endregion
					#region 11. A vector containing the delta of the ip (ip specific)
					case 11:
                        {
                            StackUtils.VectorPush(ip.Stack, ip.Delta);
                        }
                        break;
					#endregion
					#region 10. A vector containing the position of the ip (ip specific)
					case 10:
                        {
                            StackUtils.VectorPush(ip.Stack, ip.Position.Location);
                        }
                        break;
					#endregion
					#region 09. A cell containing a unique team number (ip specific)
					case 9:
                        //This appears to be useless, not even appearing in RC/Funge
                        {
                            ip.Stack.Push(0);
                        }
                        break;
					#endregion
					#region 08. A cell containing the unique ID for the current ip (ip specific)
					case 8:
                        //Used in Concurrent Funge
                        {
                            ip.Stack.Push(ip.ID);
                        }
                        break;
					#endregion
                    #region 07. A cell containing the dimensions of the interpreter (global environment)
					case 7:
						//1 for Unefunge, 2 for Befunge, 3 for Trefunge, etc. 
                        {
                            //For now we'll just push 2
                            ip.Stack.Push(2);
                        }
                        break;
					#endregion
					#region 06. A cell containing the path sperator for use with 'i' and 'o' (global environment)
					case 6:
                        {
                            //Give this is a windows system the seperator is that aweful \
                            ip.Stack.Push(System.IO.Path.DirectorySeparatorChar);
                        }
                        break;
					#endregion
					#region 05. A cell containing an ID code for the Operating Paradigm, used for understanding how the '=' instruction will handle input (global environment)
					case 5:
                        {
                            ip.Stack.Push(OptionsManager.Get<int>("I", "LF_EXECUTE_STYLE"));
                        }
                        break;
					#endregion
					#region 04. A cell containing this implementation's version number (local environment)
					case 4:
                        //where all .'s are stripped out 
                        {
                            //With only Trefunge and more Fingerprints to add, I'd say we're 88% there!
                            ip.Stack.Push(088);
                        }
                        break;
					#endregion
					#region 03. A cell containing this implementation's handprint (local environment)
					case 3:
                        //Our handprint is BSHP for BefungeSharp! Oh so clever.
                        {
                            //0x42534850, aka 
                            //'B' = 66 = 0100 0010
                            //'S' = 83 = 0101 0011
                            //'H' = 72 = 0100 1000
                            //'P' = 80 = 0101 0000

                            //0100 0010 0101 0011 0100 1000 0101 0000
                            ip.Stack.Push(0x42534850);
                        }
                        break;
					#endregion
					#region 02. A cell containing the number of bytes per cell
					case 2:
                        {
                            ip.Stack.Push(sizeof(int));
                        }
                        break;
					#endregion
					#region 01. A cell containing various flags relating to which instructions
					case 1:
                        {
                            //ip.Stack.Push((int)flags);
                            //t,= implemented, StdIO acts like getch()
							RuntimeFeatures flags = 0x0;
							flags |= OptionsManager.Get<bool>("I", "LF_CONCURRENCY") == true ? RuntimeFeatures.CONCURRENT_FUNGE : 0x0;
							flags |= OptionsManager.Get<bool>("I", "LF_FILE_INPUT")  == true ? RuntimeFeatures.FILE_INPUT       : 0x0;
							flags |= OptionsManager.Get<bool>("I", "LF_FILE_OUTPUT") == true ? RuntimeFeatures.FILE_OUTPUT      : 0x0;
							flags |= OptionsManager.Get<int>("I", "LF_EXECUTE_STYLE") > 0    ? RuntimeFeatures.EXECUTE          : 0x0;
							flags |= OptionsManager.Get<int>("I", "LF_STD_INPUT_STYLE") > 0  ? RuntimeFeatures.UNBUFFERED_IO    : 0x0;
							ip.Stack.Push((int)flags);
                        }
                        break;
					#endregion
                    #region default
                    default:
                        {
                            //Since it is impossible to get stack[20-20]
                            //We must ensure we can atleast access stack[1]
                            StackUtils.EnsureStackSafety(ip.Stack, (toExamine - 20) + 1);
                            //If it is greater than 20 we will be "picking" off the stack
                            int result = ip.Stack.ElementAtOrDefault(toExamine - 20);
                            ip.Stack.Push(result);
                        }
                        break;
					#endregion
                }
                
                //If we are taking everything ever
                if (toTake == 20)
                {
                    //Decrease toExamine
                    toExamine--;
                }
                //Decrease the number to take
                toTake--;
            }
            return true;
        }

        public int RequiredCells()
        {
            return 1;
        }
    }

    public class NotImplemented : SystemInstruction
    {
        public NotImplemented(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { Color = ConsoleColor.DarkRed; }

        public override bool Preform(IP ip)
        {
            Vector2 newDelta = ip.Delta;
            newDelta.Negate();
            ip.Delta = newDelta;

            return true;
        }
    }

    public class Breakpoint : SystemInstruction, IAffectsRunningMode
    {
        public Breakpoint(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { Color = ConsoleColor.Red; }

        public override bool Preform(IP ip)
        {
            int stophere = 0;
            stophere++;
            Program.Interpreter.ChangeMode(this);
            return true;
        }

        public BoardMode NewMode
        {
            get { return BoardMode.Run_STEP; }
        }
    }

}
