using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BefungePF
{
    enum ProgramMode
    {
        MainMenu,
        None,
        NewFile,
        OpenFile,
        Options,
        Help,
        Exit
    };

    class Program
    {
        static void Main(string[] args)
        {
            /* 1.) Attempt to read program options, if none create defaults
             * 2.) Ask user to select program mode
             * 3.) Run that mode's update method
             * 4.) Check if the program should switch to a different mode
             * 5.) When its time to exit, do so
             */
            Console.CursorVisible = true;
            Console.BufferWidth = 81;
            Console.WindowWidth = 81;
            Console.BufferHeight = 32;
            Console.WindowHeight = 32;
            bool runProgram = true;
            while (runProgram)
            {
                Console.Clear();
                //Ask what they would like to do
                Console.WriteLine("Welcome to BefungePF! Please select an option:\n");
                Console.WriteLine("1.) New File");
                Console.WriteLine("2.) Open File");
                Console.WriteLine("3.) Options");
                Console.WriteLine("4.) Help");
                Console.WriteLine("5.) Exit");


                //The initial program mode starts as none
                ProgramMode programMode = ProgramMode.None;

                do
                {
                    //Get their input
                    string input = Console.ReadLine();

                    //For all the options try to match their number or other parts of the name
                    //Save the program mode selected
                    //If
                    if (input.Contains("1") ||
                        input.Contains("ew")
                        )
                    {
                        programMode = ProgramMode.NewFile;
                    }
                    else if (input.Contains("2") ||
                            input.Contains("pen")
                        )
                    {
                        programMode = ProgramMode.OpenFile;
                    }
                    else if (input.Contains("3") ||
                          input.Contains("ptions")
                        )
                    {
                        programMode = ProgramMode.Options;
                    }
                    else if (input.Contains("4") ||
                        input.Contains("elp"))
                    {
                        programMode = ProgramMode.Help;
                    }
                    else if (input.Contains("5") ||
                        input.Contains("xit") ||
                        input.Contains("uit"))
                    {
                        programMode = ProgramMode.Exit;
                        runProgram = false;
                        //Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine(input + " is not a valid choice");
                    }
                }
                while (programMode == ProgramMode.None);

                Console.Clear();

                //Create the board and inialize it based on the above
                //(you also have the potential to enter the options or help
                BoardManager board;
                //= new BoardManager();
                switch (programMode)
                {
                    case ProgramMode.NewFile:
                        board = new BoardManager(25,80);
                        board.UpdateBoard();
                        break;
                    case ProgramMode.OpenFile:
                        string[] initLines = ExamplePrograms.calculator;
                            
                        int[] initInputs = {0, 97, 52, 120 };
                        board = new BoardManager(25, 80, initLines, initInputs);
                        board.UpdateBoard();
                        break;
                    case ProgramMode.Options:
                        //Set up the options menu
                        //Run its loop
                        break;
                    case ProgramMode.Help:
                        //Set up the options menu
                        //Run its loop
                        break;
                }//Program mode initalization and running
            }//While(runProgram)
        }//Main(string[]args)
    }//class Program
}//Namespace BefungePF


/*StreamReader rStream;
            StreamWriter wStream;
            Console.BufferHeight = Console.WindowHeight;
            //Try to find the options file
            try
            {
                string currentPath = Directory.GetCurrentDirectory();
                //If there
                if (Directory.GetFiles(currentPath, "options.txt") != null)
                {
                    rStream = new StreamReader(currentPath + "options.txt");
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                //if (rStream != null)
                {
                    //     rStream.Close();
                }
                //wStream.Close();
            }
            */