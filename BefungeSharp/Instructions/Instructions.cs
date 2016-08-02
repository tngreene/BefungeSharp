using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions
{
    //Types of commands that there could be
    public enum CommandType
    {
        //These are not necissarily complete lists
        Arithmetic,//Operators like +-*/
        Concurrent,//t
        DataStorage,//gp
        FlowControl,//#@;jqk
        FileIO,//io
		Fingerprint,//(), any other FP that doesn't match some command type here
        Logic,//!_|`
        Movement,//>v^<?
        Nop,//z and ' '
        NotImplemented,
        Number,//0-9,a-f that will be pushed onto the stack
        StackManipulation,//:$\
		StackStackManipulation,//{}u
        StopExecution,//@
        StdIO,//&~,.
        String,//"
        System,//=y
        Trefunge,//hlm
    }
        
    public abstract class Instruction
    {
        /// <summary>
        /// What is the character that identifys this instruction
        /// </summary>
        public char Name { get; protected set; }
        
        /// <summary>
        /// What command type the command
        /// </summary>
        public CommandType Type { get; protected set; }

        /// <summary>
        /// What custom color to display it as
        /// </summary>
        public ConsoleColor Color { get; protected set; }
        
        /// <summary>
        /// What minimum flags are needed, aka what minimum language and features are required for the instructions to work
        ///
        /// To be honest, I'm not quite sure what this is used for anymore, or if it is being used correctly. I just don't want to change
        /// all the instruction classes again without a good reason
        /// </summary>
        public RuntimeFeatures MinimumFlags { get; protected set; }

        public Instruction(char inName, CommandType inType, RuntimeFeatures minimum_flags)
        {
            this.Name = inName;
            this.Type = inType;
            this.Color = OptionsManager.Get<ConsoleColor>("V","COLOR_" + Enum.GetName(typeof(CommandType), inType));
            this.MinimumFlags = minimum_flags;
        }
        
        /// <summary>
        /// A factory method for making instructions
        /// </summary>
        /// <param name="c">The character that represents the instruction</param>
        /// <returns>A created instruction or null if it was unable to do so</returns>
        public static Instruction MakeInstruction(char c)
        {
            //Instruction works for all languages, all versions
            RuntimeFeatures SUPPORTED_BY_ALL = RuntimeFeatures.VERSION_93 | 
                                               RuntimeFeatures.VERSION_98 |
                                               RuntimeFeatures.UF |
                                               RuntimeFeatures.BF |
                                               RuntimeFeatures.TF;
            
            //Instruction works don't work for UF of any version
            RuntimeFeatures NO_UNFUNGE = SUPPORTED_BY_ALL ^ RuntimeFeatures.UF;
            
            //Instructions for 1,2,3D, but not in version 93
            RuntimeFeatures NO_93_COMPATIBILITY = SUPPORTED_BY_ALL ^ RuntimeFeatures.VERSION_93;

            RuntimeFeatures UF_93 = RuntimeFeatures.UF | RuntimeFeatures.VERSION_93;
            Instruction outInstruction = null;

            int dimensions = OptionsManager.Get<int>("I", "LF_DIMENSIONS");
            int spec_version = OptionsManager.Get<int>("I", "LF_SPEC_VERSION");
            //Based on the character, if it follows the necessary rules
            switch (c)
            {
                //--Delta Changing-
                case '^':
                    if(dimensions> 1)
                    {    
                        outInstruction = new Delta.CardinalInstruction(c, NO_UNFUNGE, Vector2.North);
                    }
                    break;
                case '>':
                    outInstruction = new Delta.CardinalInstruction(c, RuntimeFeatures.NULL, Vector2.East);
                    break;
                case 'v':
                    if (dimensions> 1)
                    {
                        outInstruction = new Delta.CardinalInstruction(c, NO_UNFUNGE, Vector2.South);
                    }
                    break;
                case '<':
                    outInstruction = new Delta.CardinalInstruction(c, RuntimeFeatures.NULL, Vector2.West);
                    break;
                case '?':
                    outInstruction = new Delta.RandomDeltaInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '[':
                    if (dimensions> 1 &&  spec_version > 93)
                    {
                        outInstruction = new Delta.TurnLeftInstruction(c, (RuntimeFeatures.BF | RuntimeFeatures.TF | RuntimeFeatures.VERSION_98));
                    }
                    break;
                case ']':
                    if (dimensions> 1 && spec_version > 93)
                    {
                        outInstruction = new Delta.TurnRightInstruction(c, (RuntimeFeatures.BF | RuntimeFeatures.TF | RuntimeFeatures.VERSION_98));
                    }
                    break;
                case 'x':
                    if (spec_version > 93)
                    {
                        outInstruction = new Delta.SetDeltaInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                case 'r':
                    if (spec_version > 93)
                    {
                        outInstruction = new Delta.ReverseDeltaInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                //-----------------
                //--Flow control---
                case '#':
                    outInstruction = new FlowControl.TrampolineInstruction(c, RuntimeFeatures.NULL);
                    break;
                case ';':
                    if (spec_version > 93)
                    {
                        outInstruction = new FlowControl.JumpOverInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                case 'j':
                    outInstruction = new FlowControl.JumpInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '@':
                    outInstruction = new FlowControl.StopInstruction(c, RuntimeFeatures.NULL);
                    break;
                case 'q':
                    if (spec_version > 93)
                    {
                        outInstruction = new FlowControl.QuitInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                case 'k':
                    if (spec_version > 93)
                    {
                        outInstruction = new FlowControl.IterateInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                //-----------------
                //--Logic----------
                case '!':
                    outInstruction = new Logic.NotInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '_':
                    outInstruction = new Logic.HorizontalIfInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '|':
                    outInstruction = new Logic.VerticalIfInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '`':
                    outInstruction = new Logic.GreaterThanInstruction(c, RuntimeFeatures.NULL);
                    break;
                case 'w':
                    if (dimensions> 1 && spec_version > 93)
                    {
                        outInstruction = new Logic.CompareInstruction(c, (RuntimeFeatures.BF | RuntimeFeatures.TF | RuntimeFeatures.VERSION_98));
                    }
                    break;
                //-----------------
                //--Simple Numbers-
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    outInstruction = new Number.NumberInstruction(c, RuntimeFeatures.NULL, (int)c - '0');
                    break;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    if (spec_version > 93)
                    {
                        outInstruction = new Number.NumberInstruction(c, NO_93_COMPATIBILITY, (int)c - ('a' - 10));
                    }
                    break;
                //-----------------
                //--Arithmatic-----
                case '+':
                    outInstruction = new Arithmetic.AddInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '-':
                    outInstruction = new Arithmetic.SubtractInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '*':
                    outInstruction = new Arithmetic.MultiplyInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '/':
                    outInstruction = new Arithmetic.DivideInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '%':
                    outInstruction = new Arithmetic.ModuloInstruction(c, RuntimeFeatures.NULL);
                    break;
                //-----------------
                //--Strings--------
                case '"':
                    outInstruction = new String.ToggleStringModeInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '\''://This is the ' character, aka fetch
                    if (spec_version > 93)
                    {
                        outInstruction = new String.FetchCharacterInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                case 's':
                    if (spec_version > 93)
                    {
                        outInstruction = new String.StoreCharacterInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                //-----------------
                //--Stack Manipulation
                case ':':
                    outInstruction = new Stack.DuplicateInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '$':
                    outInstruction = new Stack.PopInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '\\':
                    outInstruction = new Stack.SwapInstruction(c, RuntimeFeatures.NULL);
                    break;
                case 'n':
                    if (spec_version > 93)
                    {
                        outInstruction = new Stack.ClearStackInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                //-----------------
                //--StackStack Manipulation
                case '{':
                    if (spec_version > 93)
                    {
                        outInstruction = new StackStack.BeginBlockInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                case '}':
                    if (spec_version > 93)
                    {
                        outInstruction = new StackStack.EndBlockInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                case 'u':
                    if (spec_version > 93)
                    {
                        outInstruction = new StackStack.StackUnderStackInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                //-----------------
                //--StdIO----------
                case '&':
                    outInstruction = new StdIO.InputDecimalInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '~':
                    outInstruction = new StdIO.InputCharacterInstruction(c, RuntimeFeatures.NULL);
                    break;
                case ',':
                    outInstruction = new StdIO.OutputCharacterInstruction(c, RuntimeFeatures.NULL);
                    break;
                case '.':
                    outInstruction = new StdIO.OutputDecimalInstruction(c, RuntimeFeatures.NULL);
                    break;
                //-----------------
                //--FileIO---------
                case 'i':
                    if (OptionsManager.Get<bool>("I","LF_FILE_INPUT") == true && spec_version > 93)
                    {
                        outInstruction = new FileIO.InputFileInstruction(c, (RuntimeFeatures.FILE_INPUT | NO_93_COMPATIBILITY));
                    }
                    break;
                case 'o':
                    if (OptionsManager.Get<bool>("I", "LF_FILE_OUTPUT") == true && spec_version > 93)
                    {
                        outInstruction = new FileIO.OutputFileInstruction(c, (RuntimeFeatures.FILE_OUTPUT | NO_93_COMPATIBILITY));
                    }
                    break;
                //-----------------
                //--Data Storage---
                case 'g':
                    outInstruction = new Storage.GetInstruction(c, RuntimeFeatures.NULL);
                    break;
                case 'p':
                    outInstruction = new Storage.PutInstruction(c, RuntimeFeatures.NULL);
                    break;
                //-----------------
                //--Concurrent-----
                case 't':
                    if (OptionsManager.Get<bool>("I", "LF_CONCURRENCY") == true && spec_version > 93)
                    {
                        outInstruction = new Concurrent.SplitInstruction(c, (RuntimeFeatures.CONCURRENT_FUNGE | NO_93_COMPATIBILITY));
                    }
                    break;
                //-----------------
                //--System---------
                case '=':
                    if (OptionsManager.Get<int>("I", "LF_EXECUTE_STYLE") > 0 && spec_version > 93)
                    {
                        outInstruction = new SystemCall.ExecuteInstruction(c, (RuntimeFeatures.EXECUTE | NO_93_COMPATIBILITY));
                    }
                    break;
                case 'y':
                    if (spec_version > 93)
                    {
                        outInstruction = new SystemCall.GetSysInfo(c, NO_93_COMPATIBILITY);
                    }
                    break;
                //-----------------
                //--Footprint------
                case '(':
					if (spec_version > 93)
					{
						outInstruction = new Fingerprints.LoadSemantic(c, NO_93_COMPATIBILITY);
					}
					break;
                case ')':
					if (spec_version > 93)
					{
						outInstruction = new Fingerprints.UnloadSemantic(c, NO_93_COMPATIBILITY);
					}
					break;
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                    if (spec_version > 93)
                    {
						outInstruction = new Instructions.Fingerprints.NULL.NULL_Instruction(c);
                    }
                    break;
                //-----------------
                //--Trefunge-------
                case 'h'://Go high, 3D movement
                case 'l'://Go low, 3D movement
                case 'm'://3D if statment
                    if (spec_version > 93 && dimensions >= 3)
                    {
                        //For now Trefunge all get "Not Implemented", which acts like 'r'
                        outInstruction = new SystemCall.NotImplemented(c, (RuntimeFeatures.TF | RuntimeFeatures.VERSION_98));
                    }
                    break;
                //----------------
                //--Nop-----------
                case ' ':
                    outInstruction = new Nop.SpaceInstruction(c, RuntimeFeatures.NULL);
                    break;
                case 'z':
                    if (spec_version > 93)
                    {
                        outInstruction = new Nop.ExplicitNopInstruction(c, NO_93_COMPATIBILITY);
                    }
                    break;
                default:
                    return null;
                //-----------------
            }

            return outInstruction;
        }

        /// <summary>
        /// Preforms the action the instruction must do
        /// </summary>
        /// <param name="ip">The IP to act on (if the instruction uses the IP)</param>
        /// <returns>true if it was succesful, false if not</returns>
        public abstract bool Preform(IP ip);
    }

    public static class InstructionManager
    {
        public static Dictionary<int, Instruction> InstructionSet { get; private set; }

        public static void BuildInstructionSet()
        {
            InstructionSet = new Dictionary<int, Instruction>();
            InstructionSet.Add((char)182, new SystemCall.Breakpoint((char)182, 0));

            //For all printable ASCII characters
            for (char c = ' '; c <= '~'; c++)
            {
                Instruction instruction = Instruction.MakeInstruction(c);
                if (instruction != null)
                {
                    InstructionSet.Add(c, instruction);
                }
                else
                {
                    if (OptionsManager.Get<int>("I","LF_SPEC_VERSION") == 98)
                    {
                        InstructionSet.Add(c, new SystemCall.NotImplemented(c, RuntimeFeatures.NULL));
                    }
                    else if (OptionsManager.Get<int>("I","LF_SPEC_VERSION") == 93)
                    {
                        InstructionSet.Add(c, new Nop.ExplicitNopInstruction(c, RuntimeFeatures.NULL));
                    }
                }
            }
        }
    }

    //TODO:Clean up this mess below
    interface IPartnerSwappable
    {
        /// <summary>
        /// Makes the instruction swap its meaning with its pair, 
        /// such as with [ turning into ] after exucuting S
        /// </summary>
        void SwapMeaningWithPair();
    }

    /// <summary>
    /// Declares an instruction may require a stack pop to work
    /// </summary>
    public interface IRequiresPop
    {
        //NOTE: Due to the fact we are now using Stack.PopOrDefault this is no longer relavent. However,
        //, we are going to continue filling this out because who knows, it may be useful at somepoint.
        /// <summary>
        /// How many cells must be on the stack for it to work,
        /// </summary>
        int RequiredCells();
    }

    /// <summary>
    /// Declares an instruction may require a stack push to work
    /// </summary>
    public interface IRequiresPush
    {
        /// <summary>
        /// How much space (in cells) must be avaible for the operation to work
        /// </summary>
        /// <returns>If there is enough space on the stack to push the specified number of cells</returns>
        bool CanPushCells();
    }

    public interface IRequiresPeek
    {
        bool CanPeek(Stack<int> stack);
    }

    public interface IAffectsRunningMode
    {
        /// <summary>
        /// Set the interpreter's current mode to something else
        /// </summary>
        /// <param name="mode">The new mode</param>
        BoardMode NewMode { get; }
    }

    public interface INeedsCheckForTimeout
    {
        /// <summary>
        /// If the instruction could cause an infinite loop, TimeoutOccured defines when an instruction has or is about to enter
        /// into an infinite loop or stall.
        /// </summary>
        /// <returns>Returns true if the instruction has just or is about to put execution into a stall, false if not</returns>
        bool TimeoutOccured();
    }

    /// <summary>
    /// Declares an instruction will be altering the contents or size of funge space
    /// </summary>
    public interface IFungeSpaceAltering
    {
        
    }
}
