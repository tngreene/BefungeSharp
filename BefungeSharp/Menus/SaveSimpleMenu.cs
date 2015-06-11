﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace BefungeSharp.Menus
{
    public class SaveSimpleMenu : IMenu
    {
        public string[] InputRestriction
        {
            get { return new string[]{ "[^\"<>|;]" }; }
        }

        public void OnOpening()
        {
            //--General FileMenu content---------
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;

            Console.Clear();
            Console.WriteLine("*Enter in a file path (relative to current directory)");
            Console.WriteLine();

            Console.WriteLine("*Type help for a list of advanced commands");

            Console.WriteLine();
            FileUtils.DisplayCurrentDirectory();
            //--End General FileMenu content-----
        }

        public void OnClosing()
        {
            //--General FileMenu content---------
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
            Console.Clear();
            Console.CursorVisible = false;
            //--End General FileMenu content-----
        }

        public void RunLoop()
        {
            //--General FileMenu content-----
            OnOpening();

            string input = "";
            int timeout = 0;

            do
            {
                string path = "";
                input = Console.ReadLine().Trim();

                //Attempt to use our commands
                string testIfCommand = input.ToLower();

                Match dir_match = Regex.Match(testIfCommand, "dir ([0-9]+)$");

                if (testIfCommand == "back")
                {
                    return;
                }
                //Test if we are using the simple "dir" or one with parameters
                else if (testIfCommand == "dir")
                {
                    FileUtils.DIRCommand(input, 0, 15);
                    continue;
                }
                else if (dir_match.Success == true)
                {
                    FileUtils.DIRCommand(input, Convert.ToInt32(dir_match.Groups[1].Value), 15);
                    continue;
                }
                else if ((testIfCommand.StartsWith("cd")) && testIfCommand.Length == 2)
                {
                    FileUtils.CDCommand(input);
                    continue;
                }
                else if (testIfCommand.StartsWith("cd ") && testIfCommand.Length > 3)
                {
                    FileUtils.CD_WithPathCommand(input);
                    continue;
                }
                else if (testIfCommand == "help")
                {
                    FileUtils.HelpCommand();
                    continue;
                }
                else if (testIfCommand == "last")
                {
                    FileUtils.LastCommand(input);
                    continue;
                }
                else if (testIfCommand == "use last")
                {
                    path = FileUtils.UseLastCommand(input);
                }

                //If by this point the path has not been assaigned use the input
                if (path == "")
                {
                    //If we have a last user opened path
                    path = FileUtils.FullyExpandPath(Directory.GetCurrentDirectory() + '\\' + input);
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

                Vector2[] bounds = FungeSpace.FungeSpaceUtils.GetMatrixBounds(Program.Interpreter.FungeSpace);
                //Make sure we're only saving Q1
                bounds[0].x = 0;
                bounds[0].y = 0;

                List<List<int>> outStrings = FungeSpace.FungeSpaceUtils.MatrixToDynamicArray(Program.Interpreter.FungeSpace, bounds);
                    
                Console.WriteLine("Attempting to save {0}", path);
                bool success = FileUtils.WriteFile(path, outStrings, false, true);

                //--General FileMenu content-----
                if (success == false)
                {
                    Console.WriteLine("Please try again");

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
                Console.WriteLine("Could not save file, returning to home screen");
                OnClosing();
                return;
            }

            OnClosing();
        }

        
    }
}
