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
                            //Size of field + border + width_of_sidebar
            int width_of_sidebar = 36;//Seems right?
            ConEx.ConEx_Draw.Init(80 + 1 + width_of_sidebar, 32);
            //Console.OutputEncoding = Encoding.GetEncoding(437);
            ConEx.ConEx_Input.Init();
            ConEx.ConEx_Input.TreatControlCAsInput = true;

            bool runProgram = true;
            
            while (runProgram)
            {
                Console.Clear();

                //A silly bit of fun to make the menu look cooler
                Random rnd = new Random();
                Console.ForegroundColor = (ConsoleColor)rnd.Next(7,16);
                
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
                    string input = "";
                    input = Console.ReadLine();
                    
                    if (input == "")
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

                //Create the board and inialize it based on the above
                //(you also have the potential to enter the options or help
                BoardManager board;

                switch (programMode)
                {
                    case ProgramMode.NewFile:
                        List<string> s = new List<string>();
                        //s.Add("\"dlroW olleH\">:#,_@");
                        board = new BoardManager(25,80,s);
                        board.UpdateBoard();
                        break;
                    case ProgramMode.OpenFile:
                        //Clear the previous window
                        Console.Clear();

                        board = new BoardManager(25, 80, OpenSubMenu());
                        board.UpdateBoard();
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

                        WriteFile(Directory.GetCurrentDirectory() + "\\options.txt", outOptions);
                        break;
                    case ProgramMode.Help:
                        //Set up the options menu
                        //Run its loop
                        break;
                }//Program mode initalization and running
            }//While(runProgram)
        }//Main(string[]args)

        /// <summary>
        /// A wrapper around StreamReader operations
        /// </summary>
        /// <param name="filePath">Full path to the file you want to open</param>
        /// <returns>A list of strings containing the lines of the file</returns>
        public static List<string> ReadFile(string filePath)
        {
            //The stream for reading the file
            StreamReader rStream = null;

            //The final list of strings
            List<string> inStrings = new List<string>();
            
            try
            {
                //Create the stream reader from the file path
                rStream = new StreamReader(filePath);

                string currentLine;

                //While the next charecter is not null
                while(rStream.EndOfStream == false)
                {
                    //Read a line and add it
                    currentLine = rStream.ReadLine();
                    inStrings.Add(currentLine);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading: " + e.Message);
            }
            finally
            {
                //Make sure we close the stream
                if (rStream != null)
                {
                    rStream.Close();
                }
            }
            return inStrings;
        }

        /// <summary>
        /// A wraper around StreamWriter operations
        /// </summary>
        /// <param name="filePath">Full path to the file you want to open</param>
        /// <param name="outStrings">The list of strings you want to write out</param>
        public static void WriteFile(string filePath, List<string> outStrings)
        {
            //The stream for writing the file
            StreamWriter wStream = null;

            try
            {
                //Create the stream writer from the file path
                wStream = new StreamWriter(filePath);

                for (int i = 0; i < outStrings.Count; i++)
                {
                    wStream.WriteLine(outStrings[i]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading: " + e.Message);
            }
            finally
            {
                //Make sure we close the stream
                if (wStream != null)
                {
                    wStream.Close();
                }
            }
        }//void WriteFile

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
                Console.WriteLine("\n Attempting to load {0}",fileString);
                outputLines = ReadFile(fileString);

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
}//Namespace BefungePF