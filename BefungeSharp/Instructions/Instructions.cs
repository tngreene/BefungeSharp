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
        Movement,//>v^<?
        FlowControl,//#@;jqk
        Logic,//!_|`

        Arithmetic,//Operators like +-*/
        Numbers,//0-9,a-f that will be pushed onto the stack
        StackManipulation,//:$u{}
        DataStorage,//gp
        IO,//&~,.
        FileIO,//io
        System,//=y
        StopExecution,//@
        String,//"
        Concurrent,//t
        Trefunge,//hlm
        NotImplemented,//Many of the Funge-98 instructions. For now! - 12/31/2014
        Nop//z and ' '
    }

    public abstract class Instruction
    {
        protected char name;
        /// <summary>
        /// What is the character that identifys this instruction
        /// </summary>
        public char Name { get { return name; } }
        
        protected CommandType type;
        /// <summary>
        /// What command type the command
        /// </summary>
        public CommandType Type { get { return type; } }

        protected ConsoleColor color;
        /// <summary>
        /// What custom color to display it as
        /// </summary>
        public ConsoleColor Color { get { return color;} }
        
        protected int flags;
        
        /// <summary>
        /// What minimum flags are needed, aka what minimum language and features are required for the instructions to work
        /// </summary>
        public int MinimumFlags { get { return flags; } }
        
        public Instruction(char inName, CommandType inType, ConsoleColor inColor, int minimum_flags)
        {
            this.name = inName;
            this.type = inType;
            this.color = inColor;
            this.flags = minimum_flags;
        }

        protected void EnsureStackSafety(Stack<int> stack, int required)
        {
            if (required > stack.Count)
            {
                int toAdd = required - stack.Count;
                int toStore = stack.Count;

                Stack<int> holder = new Stack<int>();
                for (int i = 0; i < toStore; i++)
                {
                    holder.Push(stack.Pop());
                }

                //Ensure that there will always be enough in the stack
                while (stack.Count < toAdd)
                {
                    //TODO - find out if we are at max stack capacity
                    //Insert behind the top of the stack
                    stack.Push(0);
                }

                for (int i = holder.Count; i > 0; i--)
                {
                    stack.Push(holder.Pop());
                }
            }
        }

        public abstract bool Preform(IP ip);
    }

    public static class InstructionManager
    {
        private static Dictionary<int, Instruction> instruction_set;
        public static Dictionary<int, Instruction> InstructionSet { get { return instruction_set; } }

        public static void BuildInstructionSet()
        {
            instruction_set = new Dictionary<int, Instruction>();
            for (char c = ' '; c <= '~'; c++)
            {
                switch (c)
                {
                    //--Delta Changing-
                    case '^':
                        instruction_set.Add(c, new Delta.CardinalInstruction(c, 0, Vector2.North));
                        break;
                    case '>':
                        instruction_set.Add(c, new Delta.CardinalInstruction(c, 0, Vector2.East));
                        break;
                    case 'v':
                        instruction_set.Add(c, new Delta.CardinalInstruction(c, 0, Vector2.South));
                        break;
                    case '<':
                        instruction_set.Add(c, new Delta.CardinalInstruction(c, 0, Vector2.West));
                        break;
                    case '?':
                        instruction_set.Add(c, new Delta.RandomDeltaInstruction(c, 0));
                        break;
                    case '[':
                        instruction_set.Add(c, new Delta.RotateDeltaInstruction(c, 0, false));
                        break;
                    case ']':
                        instruction_set.Add(c, new Delta.RotateDeltaInstruction(c, 0, true));
                        break;
                    case 'x':
                        instruction_set.Add(c, new Delta.SetDeltaInstruction(c, 0));
                        break;
                    case 'r':
                        instruction_set.Add(c, new Delta.ReverseDeltaInstruction(c, 0));
                        break;
                    //-----------------
                    //--Flow control---
                    case '#':
                        instruction_set.Add(c, new FlowControl.TrampolineInstruction(c, 0));
                        break;
                    case ';':
                        instruction_set.Add(c, new FlowControl.JumpOverInstruction(c, 0));
                        break;
                    case 'j':
                        instruction_set.Add(c, new FlowControl.JumpInstruction(c, 0));
                        break;
                    case '@':
                        instruction_set.Add(c, new FlowControl.StopInstruction(c, 0));
                        break;
                    case 'q':
                        instruction_set.Add(c, new FlowControl.QuitInstruction(c, 0));
                        break;
                    case 'k':
                        instruction_set.Add(c, new FlowControl.IterateInstruction(c, 0));
                        break;
                    //-----------------
                    //--Logic----------
                    case '!':
                        instruction_set.Add(c, new Logic.NotInstruction(c, 0));
                        break;
                    case '_':
                        instruction_set.Add(c, new Logic.HorizontalIfInstruction(c, 0));
                        break;
                    case '|':
                        instruction_set.Add(c, new Logic.VerticalIfInstruction(c, 0));
                        break;
                    case '`':
                        instruction_set.Add(c, new Logic.GreaterThanInstruction(c, 0));
                        break;
                    case 'w':
                        instruction_set.Add(c, new Logic.CompareInstruction(c, 0));
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
                        instruction_set.Add(c, new Number.NumberInstruction(c, 0, (int)c - '0'));
                        break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        instruction_set.Add(c, new Number.NumberInstruction(c, 0, (int)c - ('a' - 10)));
                        break;
                    //-----------------
                    //--Arithmatic-----
                    case '+':
                        instruction_set.Add(c, new Arithmetic.AddInstruction(c, 0));
                        break;
                    case '-':
                        instruction_set.Add(c, new Arithmetic.SubtractInstruction(c, 0));
                        break;
                    case '*':
                        instruction_set.Add(c, new Arithmetic.MultiplyInstruction(c, 0));
                        break;
                    case '/':
                        instruction_set.Add(c, new Arithmetic.DivideInstruction(c, 0));
                        break;
                    case '%':
                        instruction_set.Add(c, new Arithmetic.ModuloInstruction(c, 0));
                        break;
                    //-----------------
                    //--Strings--------
                    case '"':
                        instruction_set.Add(c, new String.ToggleStringModeInstruction(c, 0));
                        break;
                    case '\''://This is the ' character, aka fetch
                        instruction_set.Add(c, new String.FetchCharacterInstruction(c, 0));
                        break;
                    case 's':
                        instruction_set.Add(c, new String.StoreCharacterInstruction(c, 0));
                        break;
                    //-----------------
                    //--Stack Manipulation
                    case ':':
                        instruction_set.Add(c, new Stack.DuplicateInstruction(c, 0));
                        break;
                    case '$':
                        instruction_set.Add(c, new Stack.PopInstruction(c, 0));
                        break;
                    case '\\':
                        instruction_set.Add(c, new Stack.SwapInstruction(c, 0));
                        break;
                    case 'n':
                        instruction_set.Add(c, new Stack.ClearStackInstruction(c, 0));
                        break;
                    //-----------------
                    //--StackStack Manipulation
                    case 'u':
                    case '{':
                    case '}':
                        break;
                    //-----------------
                    //IO
                    case '&':
                    case '~':
                        //return new CommandInfo(c, CommandType.IO, ConsoleColor.Gray, -1);
                    case ',':
                    case '.':
                       // return new CommandInfo(c, CommandType.IO, ConsoleColor.Gray, 1);

                    //Funge-98
                    case 'i':
                    case 'o':
                        //return new CommandInfo(c,
                                            //   CommandType.FileIO,

                        break;

                    //--Data Storage-------
                    case 'g':
                        instruction_set.Add(c, new Storage.GetInstruction(c, 0));
                        break;
                    case 'p':
                        instruction_set.Add(c, new Storage.PutInstruction(c, 0));
                        break;
                    //---------------------
                    case 't'://Split IP, for concurrent Funge
                        //return new CommandInfo(c, CommandType.Concurrent, ConsoleColor.DarkBlue, 0);
                    
                    //--System---------
                    case '=':
                        instruction_set.Add(c, new SystemCalls.ExecuteInstruction(c, 0));
                        break;
                    case 'y':
                        instruction_set.Add(c, new SystemCalls.GetSysInfo(c, 0));
                        break;
                    //-----------------

                    //Footprint stuff
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

                    //Trefunge
                    case 'h'://Go high, 3D movement
                    case 'l'://Go low, 3D movement
                    case 'm'://3D if statment
                        instruction_set.Add(c, new Delta.ReverseDeltaInstruction(c, 0));
                        break;
                    //--Nop-----------
                    case ' ':
                        instruction_set.Add(c, new Nop.SpaceInstruction(c, 0));
                        break;
                    case 'z':
                        instruction_set.Add(c, new Nop.ExplicitNopInstruction(c, 0));
                        break;
                    //-----------------
                }
            }
           // return new CommandInfo(inChar, CommandType.NotImplemented, ConsoleColor.White, 0);//For all other non instructions

            
        }
    }

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
        /// <summary>
        /// How many cells must be on the stack for it to work
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

    public interface IAffectsRunningMode
    {
        /// <summary>
        /// Set the interpreter's current mode to something else
        /// </summary>
        /// <param name="mode">The new mode</param>
        void SetNewMode(BoardMode mode);
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
        List<List<char>> GetFungeSpace();
    }
}
