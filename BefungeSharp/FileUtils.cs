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
    }
}
