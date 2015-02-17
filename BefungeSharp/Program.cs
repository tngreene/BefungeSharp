using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using BefungeSharp.FungeSpace;
namespace BefungeSharp
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

    public class Program
    {
        private static BoardManager s_board_manager;
        public static BoardManager BoardManager { get { return s_board_manager; } }

        private static WindowUI s_window_UI;
        public static WindowUI WindowUI { get { return s_window_UI; } set { s_window_UI = value; } }
        
        [STAThread]
        static void Main(string[] args)
        {
            //FungeSpaceUtils.TestMatrix();
            /* 1.) Attempt to read program options, if none create defaults
             * 2.) Ask user to select program mode
             * 3.) Run that mode's update method
             * 4.) Check if the program should switch to a different mode
             * 5.) When its time to exit, do so
             */
                            //Size of field + border + width_of_sidebar
            int width_of_sidebar = 36;//Seems right?

            Console.Title = "BefungeSharp, the Premier Funge-98 IDE for Windows";
            ConEx.ConEx_Draw.Init(80 + 1 + width_of_sidebar, 32);
            
            ConEx.ConEx_Input.Init(1000);
            ConEx.ConEx_Input.TreatControlCAsInput = true;

            bool runProgram = true;
            
            while (runProgram)
            {
                Console.Clear();

                //A silly bit of fun to make the menu look cooler
                Random rnd = new Random();
                Console.ForegroundColor = (ConsoleColor)rnd.Next(7,16);
                
                //Ask what they would like to do
                Console.WriteLine("Welcome to BefungeSharp! Please select an option:\n");
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
                    string input = "";
                    input = Console.ReadLine();
                    
                    if (input == "" || input == null)
                    {
                        continue;
                    }

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

                switch (programMode)
                {
                    case ProgramMode.NewFile:
                        List<string> s = new List<string>();
                        //s.Add("\"dlroW olleH\">:#,_@");
                        s_board_manager = new BoardManager(25, 80, s);

                        s_board_manager.UpdateBoard();
                        break;
                    case ProgramMode.OpenFile:
                        //Clear the previous window
                        Console.Clear();

                        s_board_manager = new BoardManager(25, 80, OpenSubMenu());
                        s_board_manager.UpdateBoard();
                        break;
                    case ProgramMode.Options:
                        //Set up the options menu
                        //Run its loop

                        //A test for the write file function
                        List<string> outOptions = new List<string>();
                        outOptions.Add("DEFAULTRUNSPEED=MEDIUM");
                        outOptions.Add("DISPLAYASASCII=FALSE");
                        outOptions.Add("BREAKONZ=FALSE");
                        outOptions.Add("DEFAULTEXTENSION=.txt");

                        FileUtils.WriteFile(Directory.GetCurrentDirectory() + "\\options.txt", outOptions);
                        break;
                    case ProgramMode.Help:
                        //Set up the options menu
                        //Run its loop
                        break;
                }//Program mode initalization and running
            }//While(runProgram)
        }//Main(string[]args)


        static List<string> OpenSubMenu()
        {
            Console.Write("Open a .txt or .bf, .b93, .b98, paths relative to current directory\n");
            Console.Write("For example, examples\\calculator.bf\n");

            //Create the output list
            List<string> outputLines = new List<string>();
            int timeoutCounter = 0;
            string inString;

            do
            {
                //Increase the time out
                timeoutCounter++;

                //Get the string, such as examples\mything.txt
                inString = Console.ReadLine();

                //Apppend C:\Users\...etc + \ + my words
                string fileString = Directory.GetCurrentDirectory() + '\\' + inString;
                Console.WriteLine("\n Attempting to load {0}", fileString);
                outputLines = FileUtils.ReadFile(fileString);

                if (outputLines.Count == 0)
                {
                    Console.WriteLine("\nPlease try again");
                }
            }
            while (outputLines.Count == 0 && timeoutCounter < 3);

            if (timeoutCounter >= 3)
            {
                Console.WriteLine("Could not open the file you wanted, starting in \"New File\" mode");
            }
            else if (outputLines.Count == 0)
            {
                Console.WriteLine("It appears the file had no lines, starting in \"New File\" mode");
            }
            return outputLines;
        }        
    }//class Program
}//Namespace BefungeSharp