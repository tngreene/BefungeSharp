using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace BefungeSharp.Menus
{
    public class OpenSimpleMenu : IMenu
    {
        public string[] InputRestriction
        {
            get { return new string[] { "[^\"<>|;]" }; }
        }

        public void OnOpening()
        {
            Console.Clear();
            Console.WriteLine("Enter in a file path (relative to current directory)");
            Console.WriteLine("To see a partial list of existing files type dir");
            Console.WriteLine("To see the current working directory type \"cd\", to set it use \"cd path\"");
            DisplayCurrentDirectory();
        }

        public void OnClosing()
        {
            return;
        }

        public void RunLoop()
        {
            OnOpening();

            //Create the output list
            List<string> outputLines = new List<string>();
            int timeoutCounter = 0;
            string inString;

            do
            {
                //Get the string, such as examples\mything.txt
                inString = Console.ReadLine().TrimEnd(' ');

                if (inString == "DIR" || inString == "dir")
                {
                    List<string> filesList = FileUtils.PartialDirectoryList(15);//15 seems right
                    foreach (var fileName in filesList)
                    {
                        Console.WriteLine(fileName);
                    }
                }
                else if ((inString.StartsWith("CD") || inString.StartsWith("cd")) && inString.Length == 2)
                {
                    DisplayCurrentDirectory();
                }
                else if ((inString.StartsWith("CD ") || inString.StartsWith("cd ")) && inString.Length > 3)
                {
                    string path = "";
                    try
                    {
                        path = FileUtils.FullyExpandPath(Directory.GetCurrentDirectory() + '\\' + inString.Substring(3));
                        Directory.SetCurrentDirectory(path);
                        Console.WriteLine();
                        DisplayCurrentDirectory();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(path + " is not a valid path");
                    }
                }
                else
                {
                    string path = FileUtils.FullyExpandPath(Directory.GetCurrentDirectory() + '\\' + inString);
                                        
                    Console.WriteLine("Attempting to load {0}", path);
                    Exception e = FileUtils.ReadFile(path, ref outputLines);

                    if (e != null)
                    {
                        Console.WriteLine("Please try again");
                        
                        //Increase the time out on a failed attempt
                        timeoutCounter++;                      
                    }
                }
            }
            while (outputLines.Count == 0 && timeoutCounter < 3);

            if (timeoutCounter == 3)
            {
                Console.WriteLine("Could not open file, returning to home screen");
                OnClosing();
                return;
            }

            Program.BoardManager = new BoardManager(25, 80, outputLines);
            
            //Truely start the program up!
            Program.BoardManager.UpdateBoard();
            OnClosing();
        }

        private void DisplayCurrentDirectory()
        {
            Console.WriteLine("Currently in " + Directory.GetCurrentDirectory() + '\\');
            Console.WriteLine();
        }
    }
}
