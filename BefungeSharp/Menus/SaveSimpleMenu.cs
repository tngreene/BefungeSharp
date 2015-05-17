using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Menus
{
    public class SaveSimpleMenu : IMenu
    {
        public string[] InputRestriction
        {
            get { return new string[]{ "[^\"<>|;]" }; }
        }

        public void RunLoop()
        {
            OnOpening();
            
            Console.Write("File Name: ");
            Console.Out.Flush();

            //Read filename from user
            string input = "";
            bool badInput = false;//Assume innocent until proven guilty

            int timeout = 0;
            do
            {
                input = Console.ReadLine();
                input = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory() + input);
                Console.WriteLine();

                if (FileUtils.IsValidPath(input) == false)
                {
                    Console.WriteLine(input + "contains an invalid folder path, please try again");
                    badInput = true;
                    timeout++;
                }

                if (FileUtils.IsValidFileName(input) == false)
                {
                    Console.WriteLine(input + "is not a valid name, please try again");
                    badInput = true;
                    timeout++;
                }

                if (timeout == 3)
                {
                    Console.WriteLine("File could not be saved, press any key to continue");
                    Console.ReadKey(true);
                    Console.CursorVisible = false;
                    return;
                }
            }
            while (badInput == true);

            //Test the ending
            string extention = System.IO.Path.GetExtension(input);

            //TODO - use extension based off the current mode
            //.uf for unfunge
            //.bf for befunge
            //.b98 for befunge98
            //.tf/.3f? for trefunge
            switch (extention)
            {
                //If they have included either a .txt or .bf then its okay
                case ".txt":
                case ".bf":
                    break;
                default:
                    input += ".bf";//OptionsManager.OptionsDictionary["Default extension"]
                    break;
            }

            Vector2[] bounds = FungeSpace.FungeSpaceUtils.GetMatrixBounds(Program.Interpreter.FungeSpace);
            List<string> outStrings = FungeSpace.FungeSpaceUtils.MatrixToStringList(Program.Interpreter.FungeSpace, bounds);

            Console.WriteLine("Writing file to " + System.IO.Path.GetFullPath(input));
            Console.WriteLine();

            Exception e = FileUtils.WriteFile(input, outStrings);
            if (e != null)
            {
                Console.WriteLine("Error writing file: " + e.Message);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("File written succesfully!");
                Console.WriteLine();
            }
            OnClosing();
        }

        public void OnOpening()
        {
            //Clears the screen and writes info
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;
            System.IO.Directory.SetCurrentDirectory(System.IO.Directory.GetCurrentDirectory() + '\\');
            Console.WriteLine("Current Working Directory: " + System.IO.Directory.GetCurrentDirectory());
            Console.WriteLine();

        }

        public void OnClosing()
        {
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
            Console.Clear();
            Console.CursorVisible = false;
        }
    }
}
