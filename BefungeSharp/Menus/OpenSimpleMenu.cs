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
            Console.WriteLine("For example, examples\\itoroman.bf\n");
        }

        public void OnClosing()
        {
            return;
        }

        public void RunLoop()
        {
            OnOpening();
            Directory.SetCurrentDirectory(FileUtils.LastUserOpenedPath);
            Console.Write("Currently in " + Directory.GetCurrentDirectory());
            
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
                string fileString = Directory.GetCurrentDirectory() + inString;
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

            Program.BoardManager = new BoardManager(25, 80, outputLines);
            Program.BoardManager.UpdateBoard();
            OnClosing();
        }
    }
}
