using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace BefungeSharp.Menus
{
    public class OpenSimpleMenu : Menu
    {
        public override void OnOpening()
        {
            //--General FileMenu content---------
            base.OnOpening();

            Console.WriteLine("*Enter in a file path (relative to current directory)");
            Console.Write("*Type ");
            Console.BackgroundColor = ConsoleColor.White;
            ConsoleColor old = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("help");
            Console.ForegroundColor = old;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine(" for a list of advanced commands");

            Console.WriteLine();
            FileUtils.DisplayCurrentDirectory();
            //--End General FileMenu content-----
        }
        
        public override void RunLoop()
        {
            //--General FileMenu content-----
            OnOpening();

            //Create the output list
            List<List<int>> outputLines = new List<List<int>>();

            string input = "";
            int timeout = 0;

            do
            {
                string path = "";
                input = Console.ReadLine().Trim().ToLower();

                if (input == "back")
                {
                    return;
                }
                else if (input.StartsWith("dir"))
                {
                    FileUtils.DIRCommand(input);
                    continue;
                }
                else if (input.StartsWith("cd"))
                {
                    FileUtils.CDCommand(input);
                    continue;
                }
                else if (input == "help")
                {
                    FileUtils.HelpCommand();
                    continue;
                }
                else if (input == "last")
                {
                    FileUtils.LastCommand();
                    continue;
                }
                else if (input == "clear last")
                {
                    FileUtils.ClearLast();
                    continue;
                }
                else if (input == "use last")
                {
                    path = FileUtils.UseLastCommand();
                    if (path == "")
                    {
                        continue;
                    }
                }

                //If by this point the path has not been assaigned use the input
                if (path == "")
                {
                    //If we have a last user opened path
                    path = FileUtils.FullyExpandPath(input);
                }

                if (FileUtils.IsValidPath(path) == false)
                {
                    Console.WriteLine(input + " is an invalid file path, please try again");
                    timeout++;
                    continue;
                }

                if (FileUtils.IsValidFileName(path) == false)
                {
                    Console.WriteLine(input + " is an invalid file name, please try again");
                    timeout++;
                    continue;
                }
                //--End General FileMenu content-

                //TODO:Check the extension and compare 
                //if the current settings would allow such a file,
                //then give a warning and ask them what they want to do
                Console.WriteLine("Attempting to load {0}", path);
                Console.WriteLine();
                outputLines = FileUtils.ReadFile(path, false, false, true);
               
                //--General FileMenu content-----
                if (outputLines == null)
                {
                    Console.WriteLine("Please try again");
                    Console.WriteLine();
                    //Increase the time out on a failed attempt
                    timeout++;
                    continue;
                }
                else
                {
                    Console.WriteLine("Success!");
                    Console.WriteLine();
                    break;
                }
                //--End General FileMenu content-
            }
            while (timeout < 3);

            if (timeout == 3)
            {
                Console.WriteLine("Could not open file, returning to home screen");
                OnClosing();
                return;
            }

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            int rows = outputLines.Count;
            int columns = 0;

            foreach (var list in outputLines)
            {
                if (columns < list.Count)
                {
                    columns = list.Count;
                }
            }

            //TODO:Is this useful to ever show?
            Console.WriteLine("Creating FungeSpace with a width of {0} and a height of {1}", columns, rows);
            Console.WriteLine("Please wait");
            Program.BoardManager = new BoardManager(outputLines);
            stopwatch.Stop();
            Console.WriteLine("Completed in {0}", stopwatch.Elapsed);
            OnClosing();

            //Truely start the program up!
            Program.BoardManager.UpdateBoard();
        }
    }
}
