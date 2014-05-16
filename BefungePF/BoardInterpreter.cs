using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungePF
{
    enum Direction
    {
        North,
        East,
        South,
        West
    }

    enum CommandType
    {
        Logic,
        Movement,
        Arithmatic,
        Numbers,
        StackManipulation,
        IO,
        DataStorage,
        StopExecution,
        NotImplemented
    }

    class BoardInterpreter
    {
        private BoardManager boardRef;

        //Direction of the instruction pointer
        private Direction direction;

        //Instruction Pointer X(column) and Y(row)
        private int IP_X;
        private int IP_Y;
        public int X { get { return IP_X; } }
        public int Y { get { return IP_Y; } }

        public BoardInterpreter(BoardManager mgr)
        {
            boardRef = mgr;

            direction = Direction.East;
            IP_X = 0;
            IP_Y = 0;
        }

        public CommandType Update()
        {
            DrawIP();
            CommandType type = TakeStep();
                        
            return type;
        }

        private void DrawIP()
        {
            //Get the previous charecter, where the ip just was
            //Set the cursor's position
            //Write the charecter
            char prevChar = '\0';
            //Change the background color to what it should be
            Console.BackgroundColor = ConsoleColor.Black;
            
            switch (direction)
            {
                case Direction.North:
                    prevChar = boardRef.GetCharecter(IP_Y+1, IP_X);
                    if (prevChar != '\0')
                    {
                        Console.SetCursorPosition(IP_X, IP_Y + 1);
                        Console.ForegroundColor = BoardManager.LookupColor(prevChar);
                        Console.Write(prevChar);
                    }
                    break;
                case Direction.East:
                    prevChar = boardRef.GetCharecter(IP_Y, IP_X-1);
                    if (prevChar != '\0')
                    {
                        Console.SetCursorPosition(IP_X - 1, IP_Y);
                        Console.ForegroundColor = BoardManager.LookupColor(prevChar);
                        Console.Write(prevChar);
                    }
                    break;
                case Direction.South:
                    prevChar = boardRef.GetCharecter(IP_Y-1, IP_X);
                    if (prevChar != '\0')
                    {
                        Console.SetCursorPosition(IP_X, IP_Y - 1);
                        Console.ForegroundColor = BoardManager.LookupColor(prevChar);
                        Console.Write(prevChar);
                    }
                    break;
                case Direction.West:
                    prevChar = boardRef.GetCharecter(IP_Y, IP_X+1);
                    if (prevChar != '\0')
                    {
                        Console.SetCursorPosition(IP_X + 1, IP_Y);
                        Console.ForegroundColor = BoardManager.LookupColor(prevChar);
                        Console.Write(prevChar);
                    }
                    break;
            }

            //Get the current ip's
            char charecterUnder = boardRef.GetCharecter(IP_Y, IP_X);
            Console.SetCursorPosition(IP_X, IP_Y);
            //Change the background color and write it
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = BoardManager.LookupColor(charecterUnder);
            Console.Write(charecterUnder);
            //Change it back for safety
            Console.BackgroundColor = ConsoleColor.Black;
        }

        
        private CommandType TakeStep()
        {
            /* 1.) Find out what is under the IP
             * 2.) Execute command
             * 3.) Move along delta
             */
            char cmd = boardRef.GetCharecter(IP_Y, IP_X);
            CommandType returnType;
            switch (cmd)
            {
                //Logic
                case '!':
                case '_':
                case '|':
                case '`':
                    returnType = CommandType.Logic;
                    break;
                //Flow control
                case '^':
                    direction = Direction.North;
                    returnType = CommandType.Movement;
                    break;
                case '>':
                    direction = Direction.East;
                    returnType = CommandType.Movement;
                    break;
                case '<':
                    direction = Direction.West;
                    returnType = CommandType.Movement;
                    break;
                case 'v':
                    direction = Direction.South;
                    returnType = CommandType.Movement;
                    break;
                case '?':
                    Random rnd = new Random();
                    direction = (Direction)rnd.Next(0, 4);
                    returnType = CommandType.Movement;
                    break;
                case '#':
                    returnType = CommandType.Movement;
                    break;
                case '@':
                    returnType = CommandType.StopExecution;
                    break;
                //Funge-98 flow control
                case 'j':
                case 'k':
                case 'q':
                case '[':
                case ']':
                case 'r':
                case ';':
                    returnType = CommandType.Movement;
                    break;

                case '"':
                case '\''://This is the ' charector
                //Arithmatic
                case '*':
                case '+':
                case '-':
                case '/':
                case '%':
                    returnType = CommandType.Arithmatic;
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
                    //Push the number (offset by the ascii code number)
                    boardRef.PushValue(boardRef.GlobalStack, (int)cmd - 48);
                    returnType = CommandType.Numbers;
                    break;
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                    //Push the number (offset by the ascii code number-10)
                    boardRef.PushValue(boardRef.GlobalStack, (int)cmd - 97 - 10);
                    returnType = CommandType.Numbers;
                    break;
                //Stack Manipulation
                case ':':
                case '=':
                case '\\':
                case '$':
                    returnType = CommandType.Numbers;
                    break;
                //IO
                case '&':
                case '~':
                case ',':
                case '.':
                //Funge-98
                case 'i':
                case 'o':
                    returnType = CommandType.IO;
                    break;
                //Data Storage
                case 'g':
                case 'p':
                    returnType = CommandType.DataStorage;
                    break;
                case 'h':


                case 'l':
                case 'm':
                case 'n':



                case 's':
                case 't':
                case 'u':
                case 'w':
                case 'x':

                case 'z':
                case '{':
                case '}':

                //Funge-98 ONLY Schematics

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
                    returnType = CommandType.NotImplemented;
                    break;
                default:
                    returnType = CommandType.NotImplemented;
                    break;
            }

            //Based on the direction move or wrap the pointer around
            switch (direction)
            {
                case Direction.North:
                    if (IP_Y > 0)
                    {
                        IP_Y -= 1;
                    }
                    else
                    {
                        IP_Y = 24;
                    }
                    break;
                case Direction.East:
                    if (IP_X < 79 - 1)
                    {
                        IP_X += 1;
                    }
                    else
                    {
                        IP_X = 0;
                    }
                    break;
                case Direction.South:
                    if (IP_Y < 24-1)
                    {
                        IP_Y += 1;
                    }
                    else
                    {
                        IP_Y = 0;
                    }
                    break;
                case Direction.West:
                    if (IP_X > 0)
                    {
                        IP_X -= 1;
                    }
                    else
                    {
                        IP_X = 79;
                    }
                    break;
            }
            return returnType;
        }
    }
}
