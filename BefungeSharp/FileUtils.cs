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
        /// <param name="fileContents">A list of strings containing the lines of the file</returns>
        /// <returns>If the write succeedes it will return a null exception, else it will return the exception that was generated</returns>
        public static Exception ReadFile(string filePath, ref List<string> fileContents)
        {
            //The stream for reading the file
            StreamReader rStream = null;
            
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
                    fileContents.Add(currentLine);
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
                if (rStream != null)
                {
                    rStream.Close();
                }
            }
            return null;
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
        /// Tests whether a path is valid or not
        /// </summary>
        /// <param name="path">The path to test</param>
        /// <returns>Returns true if the path is a valid path</returns>
        public static bool IsValidPath(string path)
        {
            ///We need atleast a drive letter and :\
            if (path.Count() < 3)
            {
                return false;
            }

            bool testResult = true;
            foreach (char c in System.IO.Path.GetInvalidPathChars())
            {
                testResult |= path.Contains(c); //A good input will never return true
            }
            return testResult;
        }

        /// <summary>
        /// Tests whether a file name is valid or not
        /// </summary>
        /// <param name="name">The file name to test</param>
        /// <returns>Returns true if the path is a valid path</returns>
        public static bool IsValidFileName(string name)
        {
            if (name.Count() < 1)
            {
                return false;
            }

            bool testResult = true;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                testResult |= System.IO.Path.GetFileName(name).Contains(c); //A good input will never return true
            }
            return testResult;
        }

        /// <summary>
        /// Generates a short list of strings which are a partial list of directories and files
        /// </summary>
        /// <returns>The list of strings to be displayed</returns>
        public static List<string> PartialDirectoryList(int count)
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
                for (int i = 0; i < fileList.Count && i < count; i++)
                {
                    outList.Add(fileList[i].FullName.Remove(0, Directory.GetCurrentDirectory().Count() + 1)); 
                }
            }
            catch (Exception e)
            {

            }

            return outList;
        }

        public static string FullyExpandPath(string path)
        {
            //Expands any environment variables
            string expanded = Environment.ExpandEnvironmentVariables(path);
            //Handles cases like . and ..
            expanded = Path.GetFullPath(expanded);
            
            return expanded;
        }
    }
}
