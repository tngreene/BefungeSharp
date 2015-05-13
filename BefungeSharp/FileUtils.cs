using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BefungeSharp
{
    /// <summary>
    /// A class that contains static methods relating to file handling utilities
    /// of the text editor and Funge-IO portions
    /// </summary>
    public static class FileUtils
    {
        
        public static string LastUserOpenedPath { get; private set;}

        public static string LastUserOpenedFile { get { return Path.GetFileName(LastUserOpenedPath); } }
        
        static FileUtils()
        {
            LastUserOpenedPath = Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// A wrapper around StreamReader operations
        /// </summary>
        /// <param name="filePath">Full path to the file you want to open, assumed clean</param>
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

                //While the next character is not null
                while (rStream.EndOfStream == false)
                {
                    //Read a line and add it
                    currentLine = rStream.ReadLine();
                    inStrings.Add(currentLine);
                }
                LastUserOpenedPath = Path.GetFullPath(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading: " + e.Message);
                LastUserOpenedPath = "";
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
        /// <returns>If the write succeedes it will return a null exception, else it will return the exception that was generated</returns>
        public static Exception WriteFile(string filePath, List<string> outStrings)
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
                return e;
            }
            finally
            {
                //Make sure we close the stream
                if (wStream != null)
                {
                    wStream.Close();
                }
            }
            return null;
        }//void WriteFile

        /// <summary>
        /// Saves the current board to the default save location
        /// </summary>
        public static void SaveFungeSpace()
        {
            //Clears the screen and writes info
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;
            System.IO.Directory.SetCurrentDirectory(System.IO.Directory.GetCurrentDirectory() + '\\');

            Console.WriteLine("Current Working Directory: " + System.IO.Directory.GetCurrentDirectory());
            Console.WriteLine();
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

                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    badInput |= System.IO.Path.GetFileName(input).Contains(c); //A good input will never return true
                }

                foreach (char c in System.IO.Path.GetInvalidPathChars())
                {
                    badInput |= input.Contains(c); //A good input will never return true
                }

                if (badInput == true || input.Count() == 0)
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
            //.tf for trefunge
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
            
            Vector2 [] bounds = FungeSpace.FungeSpaceUtils.GetMatrixBounds(Program.Interpreter.FungeSpace);
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
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
            Console.Clear();
            Console.CursorVisible = false;
        }
    }
}
