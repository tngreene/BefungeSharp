using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions
{
    public abstract class Instruction
    {
        
        protected char name;//What is the name of it, such as < or | or 4
        protected CommandType type;//What type the command is
        protected ConsoleColor color;//What color to display it as
        protected UInt32 flags;//What minimum flags are needed, aka what minimum language and features are required for the instructions to work

        public Instruction(char inName, CommandType inType, ConsoleColor inColor, UInt32 minimum_flags)
        {
            this.name = inName;
            this.type = inType;
            this.color = inColor;
            this.flags = minimum_flags;
        }

        protected void EnsureStackSafety(Stack<int> stack, int required)
        {
            //Ensure that there will always be enough in the stack
            while (stack.Count < required)
            {
                //TODO - find out if we are at max stack capacity
                stack.Push(0);
            }
        }

        public abstract bool Preform(IP ip, BoardManager mgr = null);
    }

    public static class InstructionManager
    {
        private static Dictionary<char, Instruction> instruction_set;
        public static Dictionary<char, Instruction> InstructionSet { get { return instruction_set; } }

        public static void BuildInstructionSet()
        {
            instruction_set = new Dictionary<char, Instruction>();
            for (char c = ' '; c <= '~'; c++)
            {
                switch (c)
                {
                    //Logic
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
                    //Flow control
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
                        instruction_set.Add(c, new Delta.RandomDeltaInstruction(c, 0, Vector2.Zero));
                        break;
                    case '#':
                    //Funge-98 flow control
                    case '[':
                    case ']':
                    case 'r':
                    case ';':
                        //CommandInfo flowCommand = new CommandInfo(c, CommandType.Movement, ConsoleColor.Cyan, 0);
                        //return flowCommand;
                    case 'j':
                    case 'k':
                        //CommandInfo flowCommand98 = new CommandInfo(c, CommandType.Movement, ConsoleColor.Cyan, 1);
                        //return flowCommand98;
                    case 'x':
                        instruction_set.Add(c, new Delta.SetDeltaInstruction(c, 0, Vector2.Zero));
                        break;
                        //return new CommandInfo(c, CommandType.Movement, ConsoleColor.Cyan, 2);
                    case '@':
                    case 'q':
                       // CommandInfo stopCommand = new CommandInfo(c, CommandType.StopExecution, ConsoleColor.Red, 0);
                        //return stopCommand;

            
                    //Arithmatic
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
                    //Numbers
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
                        instruction_set.Add(c, new Number.NumberInstruction(c,0,(int)c-'0'));
                        break;
                    case 'a':
                    case 'b':
                    case 'c':
                    case 'd':
                    case 'e':
                    case 'f':
                        instruction_set.Add(c, new Number.NumberInstruction(c,0,(int)c-('a'-10)));
                        break;

                        //Stack Manipulation
                        //CommandInfo stackManipCommand;
                    case ':':
                       // stackManipCommand = new CommandInfo(c,
                                                         //   CommandType.StackManipulation,
                                                       //     ConsoleColor.DarkYellow, 1);//Technically we're not poping
                        //But it does require something on the stack
                        //return stackManipCommand;
                    case '$':
                       // stackManipCommand = new CommandInfo(c, CommandType.StackManipulation, ConsoleColor.DarkYellow, 1);
                        //return stackManipCommand;
                    case '\\':
                      //  stackManipCommand = new CommandInfo(c, CommandType.StackManipulation, ConsoleColor.DarkYellow, 2);
                        //return stackManipCommand;
                    case 'n':
                       // return new CommandInfo(c, CommandType.StackManipulation, ConsoleColor.DarkYellow, 1);//Op must be >=1 on stack

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
                                             //  ConsoleColor.Gray, -Int32.MaxValue);//Beware, must be a try catch operation!
                    //Data Storage
                    case 'g':
                        //return new CommandInfo(c, CommandType.DataStorage, ConsoleColor.Green, 2);
                    case 'p':
                       // return new CommandInfo(c, CommandType.DataStorage, ConsoleColor.Green, 3);
                    //String Manipulation
                    case '"':
                        //return new CommandInfo(c, CommandType.String, ConsoleColor.Green, 0);
                    case 't'://Split IP, for concurrent Funge
                        //return new CommandInfo(c, CommandType.Concurrent, ConsoleColor.DarkBlue, 0);
                    case 's':

                    case '\''://This is the ' charector



                    //Stack-Stack Manipulation 98
                    case 'u':
                    case '{':
                    case '}':

                    //Funge-98 ONLY Schematics
                    case '=':
                    //Handprint stuff
                    case 'y':
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
                    case 'z'://Does not exist - TODO its actually nop
                        break;
                        //---------------------------
                        //return new CommandInfo(c, CommandType.NotImplemented, ConsoleColor.DarkRed, 0);
                }
            }
           // return new CommandInfo(inChar, CommandType.NotImplemented, ConsoleColor.White, 0);//For all other non instructions

            
        }
    }
    

    interface IStackAltering
    {
        /// <summary>
        /// How many cells must be on the stack for it to work
        /// </summary>
        int RequiredCells();
    }

    interface IFungeSpaceAltering
    {
        //If the space
    }
}
