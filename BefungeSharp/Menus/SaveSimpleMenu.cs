using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace BefungeSharp.Menus
{
    public class SaveSimpleMenu : Menu
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

                //File overwrite safeguard
                if (File.Exists(path) == true)
                {
                    Console.WriteLine("{0} already exists, are you sure you want to overwrite file? Y/N", Path.GetFileName(path));
                    input = Console.ReadKey().KeyChar.ToString().ToLower();
                    Console.WriteLine();
                    if (input != "y")
                    {
                        Console.WriteLine("Aborting save, please input new file name");
                        continue;
                    }
                }

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
