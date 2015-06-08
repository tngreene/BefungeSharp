using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace BefungeSharp
{
    /// <summary>
    /// A class that contains static methods relating to file handling utilities
    /// of the text editor and Funge-IO portions
    /// </summary>
    public static class FileUtils
    {
        
        public static string LastUserOpenedPath { get; private set;}

        public static string LastUserOpenedDirectory { get { return Path.GetDirectoryName(LastUserOpenedPath); } }
        public static string LastUserOpenedFile { get { return Path.GetFileName(LastUserOpenedPath); } }
        
        static FileUtils()
        {
            LastUserOpenedPath = Environment.GetCommandLineArgs()[0];
        }

        /// <summary>
        /// A wrapper around StreamReader operations
        /// </summary>
        /// <param name="filePath">Full path to the file you want to open, assumed clean</param>
        /// <param name="readAsBinary">Reads the file as if it were all binary data</param>
        /// <param name="supressConsoleMessages">Blocks the printing of console messages (for when not in a terminal like mode)</param>
        /// <returns>If reading is a success a List of Lists of ints containing the file contents
        /// will be returned, if there was a problem null will be returned</returns>
        public static List<List<int>> ReadFile(string filePath, bool readAsBinary, bool supressConsoleMessages)
        {
            //The stream for reading the file
            StreamReader rStream = null;
            
            List<List<int>> fileContents = new List<List<int>>();
            fileContents.Add(new List<int>());

            try
            {
                //Create the stream reader from the file path
                rStream = new StreamReader(filePath);

                if (readAsBinary == true)
                {
                    while (rStream.EndOfStream == false)
                    {
                        int value = rStream.Read();
                        fileContents.Last().Add(value);
                    }
                }
                else
                {
                    //While the next character is not null
                    while (rStream.EndOfStream == false)
                    {
                        //Read a line and add it
                        int value = rStream.Read();

                        if (value == '\r' || value == '\n')
                        {
                            //If the line ending is \r\n
                            if (rStream.Peek() == '\n')
                            {
                                //advanced past it
                                rStream.Read();
                            }
                            fileContents.Add(new List<int>());
                        }
                        else
                        {
                            fileContents.Last().Add(value);
                        }
                    }
                }
                
                LastUserOpenedPath = Path.GetFullPath(filePath);
            }
            catch (Exception e)
            {
                if (supressConsoleMessages == false)
                {
                    Console.WriteLine("Error reading: " + e.Message);
                }
                //Reset the LastUserOpenedPath to something safe
                LastUserOpenedPath = Environment.GetCommandLineArgs()[0];
                
                return null;
            }
            finally
            {
                //Make sure we close the stream
                if (rStream != null)
                {
                    rStream.Close();
                }
            }

            return fileContents;
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
                
                LastUserOpenedPath = Path.GetFullPath(filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading: " + e.Message);

                //Reset the LastUserOpenedPath to something safe
                LastUserOpenedPath = Environment.GetCommandLineArgs()[0];

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
        /// Tests whether a path is valid or not
        /// </summary>
        /// <param name="path">The path to test</param>
        /// <returns>Returns true if the path is a valid path</returns>
        public static bool IsValidPath(string path)
        {
            //We need atleast a drive letter and :\ OR %X%
            if (path.Count() < 3)
            {
                return false;
            }
            
            foreach (char c in System.IO.Path.GetInvalidPathChars())
            {
                if (path.Contains(c) == true)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Tests whether a file name is valid or not
        /// </summary>
        /// <param name="name">The file name to test</param>
        /// <returns>Returns true if the path is a valid path</returns>
        public static bool IsValidFileName(string path)
        {
            if (path.Count() == 0)
            {
                return false;
            }
            
            path = path.Substring(path.LastIndexOf('\\') + 1);

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                if (path.Contains(c) == true)
                {
                    return false;
                }
            }

            return true;
        }
        
        /// <summary>
        /// Fully expands a path to expand environment variables
        /// and navigate ..'s
        /// </summary>
        /// <param name="path">The path to attempt to expand</param>
        /// <returns>The expanded string or "" if there was a problem</returns>
        public static string FullyExpandPath(string path)
        {
            if (FileUtils.IsValidPath(path) == false)
            {
                return "";
            }

            if (FileUtils.IsValidFileName(path) == false)
            {
                return "";
            }

            //Expands any environment variables
            string expanded = Environment.ExpandEnvironmentVariables(path);

            //If it is rooted
            if (Path.IsPathRooted(expanded) == true)
            {
                //Handles cases like . and ..
                expanded = Path.GetFullPath(expanded);
            }
            else
            {
                //Else we root it ourselfs
                expanded = (Directory.GetCurrentDirectory() + '\\' + path);
                expanded = Path.GetFullPath(expanded);
            }
            return expanded;
        }

        /// <summary>
        /// Tries to run our pretend CD command, 
        /// without accepting the optional argument for a path to change directories to
        /// </summary>
        /// <param name="input">The input to test if it is "CD" or "cd"</param>
        /// <returns>
        /// Returns true if the input was the CD command,
        /// false if not or if there was a problem
        /// </returns>
        public static bool CDCommand(string input)
        {
            if ((input.ToLower().StartsWith("cd")) && input.Length == 2)
            {
                FileUtils.DisplayCurrentDirectory();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to run our pretend CD command, with the path argument
        /// </summary>
        /// <param name="input">The input to test if it is the command</param>
        /// <returns>
        /// Returns true if the input was the CD command with the path argument,
        /// false if not or there was a problem
        /// </returns>
        public static bool CD_WithPathCommand(string input)
        {
            string path = "";
            try
            {
                path = FileUtils.FullyExpandPath(input.Substring(3));
                Directory.SetCurrentDirectory(path);
                Console.WriteLine();
                FileUtils.DisplayCurrentDirectory();
            }
            catch (Exception)
            {
                Console.WriteLine(path + " is not a valid path");
                Console.WriteLine();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Tries to run our pretend DIR command,
        /// printing out a partial list of the most recently used files
        /// </summary>
        /// <param name="input">The input to test if it is "DIR" or "dir"</param>
        /// <returns>
        /// Returns true if the input was the DIR command,
        /// false if not
        /// </returns>
        public static bool DIRCommand(string input, int start_index = 0, int count = 15)
        {
            List<string> outList = new List<string>();

            //TODO:Choose the allowed extentions based on the language
            string[] allowedExtensions;
            //Thanks Marc! http://stackoverflow.com/a/30082323
            allowedExtensions = new string[] { ".bf", ".b93", ".b98", ".tf", ".txt" };

            try
            {
                DirectoryInfo info = new DirectoryInfo(Directory.GetCurrentDirectory());
                List<FileInfo> fileList = info
                    .GetFiles("*", SearchOption.AllDirectories) //Get all the files with the allowed extensions,
                    .Where(file => allowedExtensions.Any(file.Extension.ToLower().EndsWith))
                    //Sort by the last access time so they appear on the top of the list and will get chosen first
                    .OrderBy(f => f.LastAccessTime)
                    .ToList();

                Console.WriteLine("Found {0} files, displaying up to {1} starting at index {2}:", fileList.Count, count, start_index);
                for (int i = start_index; i < fileList.Count || i < count; i++)
                {
                    outList.Add(fileList[i].FullName.Remove(0, Directory.GetCurrentDirectory().Count() + 1));
                }
            }
            catch (Exception e)
            {

            }

            foreach (var fileName in outList)
            {
                Console.WriteLine(fileName);
            }
            Console.WriteLine();
            return true;
        }

        //Print out the help text for the commands
        public static bool HelpCommand()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("back - Goes back to the previous window");
            Console.WriteLine("dir - Prints out the 15 most recently used files");
            Console.WriteLine("dir index - Like dir, but allowing you to print out other parts of the list of files. Ex: dir 5");
            Console.WriteLine("cd - Prints the current working directory");
            Console.WriteLine("cd path -  Attempts to set the current working directory to the path argument. Ex: cd C:\\");
            Console.WriteLine("last - Prints the last used file, if there is one");
            Console.WriteLine("use last - Attempts to use the last used file");
            Console.WriteLine("help - Brings up this information");
            return true;
        }
        /// <summary>
        /// Tries to run our LAST command,
        /// printing out the last used file path
        /// </summary>
        /// <param name="input">The input to test if it is "LAST" or "last"</param>
        /// <returns>
        /// Returns true if the input was the last command,
        /// false if not
        /// </returns>
        public static bool LastCommand(string input)
        {
            //We never tell people that we're using the exe as a technicality
            if (FileUtils.LastUserOpenedPath == Environment.GetCommandLineArgs()[0])
            {
                Console.WriteLine("No files have been used this session");
                Console.WriteLine();
                return true;
            }
            else
            {
                Console.WriteLine(FileUtils.LastUserOpenedPath);
                Console.WriteLine();
                return true;
            }
        }

        /// <summary>
        /// Checks if the input is matches the USE LAST command
        /// </summary>
        /// <param name="input">The input to test if it is "USE LAST" or "use last"</param>
        /// <returns>
        /// Returns the LastUserOpenedPath if it is not the default, or "" if it is the default
        /// </returns>
        public static string UseLastCommand(string input)
        {
            //We never allow opening the default
            if (FileUtils.LastUserOpenedPath == Environment.GetCommandLineArgs()[0])
            {
                Console.WriteLine("No last used file to use");
                Console.WriteLine();
                return  "";
            }
            return FileUtils.LastUserOpenedPath;
        }

        public static void DisplayCurrentDirectory()
        {
            Console.WriteLine("Currently in " + Directory.GetCurrentDirectory() + '\\');
            Console.WriteLine();
        }
    }
}
