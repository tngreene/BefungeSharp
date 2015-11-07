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
        /// </summary>
        public RuntimeFeatures MinimumFlags { get; protected set; }
        
        public Instruction(char inName, CommandType inType, RuntimeFeatures minimum_flags)
        {
            this.Name = inName;
            this.Type = inType;
            this.Color = OptionsManager.SessionOptions["Visualizer"]["COLOR_" + Enum.GetName(typeof(CommandType), inType)].GetValueTyped<ConsoleColor>();
            
            if ((minimum_flags & Program.Interpreter.EnvFlags) == minimum_flags)
            {
                this.MinimumFlags = minimum_flags;
            }
            else
            {
                this.MinimumFlags = RuntimeFeatures.NULL;
            }
        }
        
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
            Instruction outInstruction = null;
            switch (c)
            {
                //--Delta Changing-
                case '^':
                    outInstruction = new Delta.CardinalInstruction(c, NO_UNFUNGE, Vector2.North);
                    break;
                case '>':
                    outInstruction = new Delta.CardinalInstruction(c, SUPPORTED_BY_ALL, Vector2.East);
                    break;
                case 'v':
                    outInstruction = new Delta.CardinalInstruction(c, NO_UNFUNGE, Vector2.South);
                    break;
                case '<':
                    outInstruction = new Delta.CardinalInstruction(c, SUPPORTED_BY_ALL, Vector2.West);
                    break;
                case '?':
                    outInstruction = new Delta.RandomDeltaInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '[':
                    outInstruction = new Delta.TurnLeftInstruction(c, (RuntimeFeatures.BF | RuntimeFeatures.TF | RuntimeFeatures.VERSION_98));
                    break;
                case ']':
                    outInstruction = new Delta.TurnRightInstruction(c, (RuntimeFeatures.BF | RuntimeFeatures.TF | RuntimeFeatures.VERSION_98));
                    break;
                case 'x':
                    outInstruction = new Delta.SetDeltaInstruction(c, NO_93_COMPATIBILITY);
                    break;
                case 'r':
                    outInstruction = new Delta.ReverseDeltaInstruction(c, NO_93_COMPATIBILITY);
                    break;
                //-----------------
                //--Flow control---
                case '#':
                    outInstruction = new FlowControl.TrampolineInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case ';':
                    outInstruction = new FlowControl.JumpOverInstruction(c, NO_93_COMPATIBILITY);
                    break;
                case 'j':
                    outInstruction = new FlowControl.JumpInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '@':
                    outInstruction = new FlowControl.StopInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case 'q':
                    outInstruction = new FlowControl.QuitInstruction(c, NO_93_COMPATIBILITY);
                    break;
                case 'k':
                    outInstruction = new FlowControl.IterateInstruction(c, NO_93_COMPATIBILITY);
                    break;
                //-----------------
                //--Logic----------
                case '!':
                    outInstruction = new Logic.NotInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '_':
                    outInstruction = new Logic.HorizontalIfInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '|':
                    outInstruction = new Logic.VerticalIfInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '`':
                    outInstruction = new Logic.GreaterThanInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case 'w':
                    outInstruction = new Logic.CompareInstruction(c, (RuntimeFeatures.BF | RuntimeFeatures.TF | RuntimeFeatures.VERSION_98));
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
                    outInstruction = new Number.NumberInstruction(c, SUPPORTED_BY_ALL, (int)c - '0');
                    break;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    outInstruction = new Number.NumberInstruction(c, NO_93_COMPATIBILITY, (int)c - ('a' - 10));
                    break;
                //-----------------
                //--Arithmatic-----
                case '+':
                    outInstruction = new Arithmetic.AddInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '-':
                    outInstruction = new Arithmetic.SubtractInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '*':
                    outInstruction = new Arithmetic.MultiplyInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '/':
                    outInstruction = new Arithmetic.DivideInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '%':
                    outInstruction = new Arithmetic.ModuloInstruction(c, SUPPORTED_BY_ALL);
                    break;
                //-----------------
                //--Strings--------
                case '"':
                    outInstruction = new String.ToggleStringModeInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '\''://This is the ' character, aka fetch
                    outInstruction = new String.FetchCharacterInstruction(c, NO_93_COMPATIBILITY);
                    break;
                case 's':
                    outInstruction = new String.StoreCharacterInstruction(c, NO_93_COMPATIBILITY);
                    break;
                //-----------------
                //--Stack Manipulation
                case ':':
                    outInstruction = new Stack.DuplicateInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '$':
                    outInstruction = new Stack.PopInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '\\':
                    outInstruction = new Stack.SwapInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case 'n':
                    outInstruction = new Stack.ClearStackInstruction(c, NO_93_COMPATIBILITY);
                    break;
                //-----------------
                //--StackStack Manipulation
                case '{':
                    outInstruction = new StackStack.BeginBlockInstruction(c, NO_93_COMPATIBILITY);
                    break;
                case '}':
                    outInstruction = new StackStack.EndBlockInstruction(c, NO_93_COMPATIBILITY);   
                    break;
                case 'u':
                    outInstruction = new StackStack.StackUnderStackInstruction(c, NO_93_COMPATIBILITY);
                    break;
                //-----------------
                //--StdIO----------
                case '&':
                    outInstruction = new StdIO.InputDecimalInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '~':
                    outInstruction = new StdIO.InputCharacterInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case ',':
                    outInstruction = new StdIO.OutputCharacterInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case '.':
                    outInstruction = new StdIO.OutputDecimalInstruction(c, SUPPORTED_BY_ALL);
                    break;
                //-----------------
                //--FileIO---------
                case 'i':
                    outInstruction = new FileIO.InputFileInstruction(c, (RuntimeFeatures.FILE_INPUT | NO_93_COMPATIBILITY));
                    break;
                case 'o':
                    outInstruction = new FileIO.OutputFileInstruction(c, (RuntimeFeatures.FILE_OUTPUT | NO_93_COMPATIBILITY));
                    break;
                //-----------------
                //--Data Storage---
                case 'g':
                    outInstruction = new Storage.GetInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case 'p':
                    outInstruction = new Storage.PutInstruction(c, SUPPORTED_BY_ALL);
                    break;
                //-----------------
                //--Concurrent-----
                case 't':
                    outInstruction = new Concurrent.SplitInstruction(c, (RuntimeFeatures.CONCURRENT_FUNGE | NO_93_COMPATIBILITY));
                    break;
                //-----------------
                //--System---------
                case '=':
                    outInstruction = new SystemCall.ExecuteInstruction(c, (RuntimeFeatures.EXECUTE | NO_93_COMPATIBILITY));
                    break;
                case 'y':
                    outInstruction = new SystemCall.GetSysInfo(c, NO_93_COMPATIBILITY);
                    break;
                //-----------------
                //--Footprint------
                case '(':
                case ')':
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
                    outInstruction = new SystemCall.NotImplemented(c, NO_93_COMPATIBILITY);
                    break;
                //-----------------
                //--Trefunge-------
                case 'h'://Go high, 3D movement
                case 'l'://Go low, 3D movement
                case 'm'://3D if statment
                    //For now Footprint and Trefunge all get "Not Implemented", which acts like 'r'
                    outInstruction = new SystemCall.NotImplemented(c, (RuntimeFeatures.TF | RuntimeFeatures.VERSION_98));
                    break;
                //----------------
                //--Nop-----------
                case ' ':
                    outInstruction = new Nop.SpaceInstruction(c, SUPPORTED_BY_ALL);
                    break;
                case 'z':
                    outInstruction = new Nop.ExplicitNopInstruction(c, NO_93_COMPATIBILITY);
                    break;
                default:
                    return null;
                //-----------------
            }

            if (outInstruction.MinimumFlags != RuntimeFeatures.NULL)
            {
                return outInstruction;
            }
            else
            {
                return null;
            }
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
        private static Dictionary<int, Instruction> instruction_set;
        public static Dictionary<int, Instruction> InstructionSet { get { return instruction_set; } }

        public static void BuildInstructionSet()
        {
            instruction_set = new Dictionary<int, Instruction>();
            instruction_set.Add((char)182, new SystemCall.Breakpoint((char)182, 0));

            //For all printable ASCII characters
            for (char c = ' '; c <= '~'; c++)
            {
                Instruction instruction = Instruction.MakeInstruction(c);
                if (instruction != null)
                {
                    instruction_set.Add(c, instruction);
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
