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
        /// <summary>
        /// The last used file's file encoding
        /// </summary>
        public static string LastUsedEncoding
        {
            get
            {
                return OptionsManager.Get<string>("G", "FILE_ENCODING");
            }
            private set
            {
                OptionsManager.Set<string>("G", "FILE_ENCODING", value);
            }
        }

        /// <summary>
        /// The fully rooted last used file path,
        /// or "" if there was no last used file
        /// </summary>
        public static string LastUserOpenedPath
        {
            get
            {
                return OptionsManager.Get<string>("G", "FILE_LAST_USED");
            }
            private set
            {
                OptionsManager.Set<string>("G", "FILE_LAST_USED", value);
            }
        }

        /// <summary>
        /// The fully rooted directory of the last used file,
        /// or "" if there was no last used file
        /// </summary>
        public static string LastUserOpenedDirectory { 
            get
            {
                if (LastUserOpenedPath == "")
                {
                    return "";
                }
                else
                {
                    return Path.GetDirectoryName(LastUserOpenedPath);
                }
            } 
        }

        /// <summary>
        /// The file name of the last used file, without the rooted path
        /// </summary>
        public static string LastUserOpenedFile { get { return Path.GetFileName(LastUserOpenedPath); } }

        /*static FileUtils()
        {
           
        }*/

        /// <summary>
        /// Copies the current file about to be written over into the backup directory,
        /// manages the content of the backup directory
        /// </summary>
        /// <param name="filePath">A valid, real, file path of the file we're about to copy</param>
        /// <param name="supressConsoleMessages">Blocks the printing of console messages (for when not in a terminal like mode)</param>
        public static void BackupFile(string filePath, bool supressConsoleMessages)
        {
            string backupsPath = Path.GetDirectoryName(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0])) + "\\" +
                                                        OptionsManager.Get<string>("G", "FILE_BACKUPS_FOLDER");
            if (Directory.Exists(backupsPath) == false)
            {
                try 
	            {	        
                    Directory.CreateDirectory(backupsPath);
	            }
	            catch (Exception e)
	            {
		            if(supressConsoleMessages == false)
                    {
                        Console.WriteLine("Backups directory could not be created: " + e.Message);
                    }
                    return;
	            }
            }
            try 
            {
                //...\Backups\fileName_YYYY_MM_DD_HH_MM_SS.fileExtension.bak
                StringBuilder sb = new StringBuilder(9);
                sb.Append(backupsPath);
                sb.Append("\\");
                sb.Append(Path.GetFileNameWithoutExtension(filePath));
                sb.Append("_");
                sb.Append(DateTime.Now.Year.ToString());
                sb.Append("_");
                sb.Append(DateTime.Now.Month.ToString().PadLeft(2, '0'));
                sb.Append("_");
                sb.Append(DateTime.Now.Day.ToString().PadLeft(2, '0'));
                sb.Append("_");
                sb.Append(DateTime.Now.Hour.ToString().PadLeft(2, '0'));
                sb.Append("_");
                sb.Append(DateTime.Now.Minute.ToString().PadLeft(2, '0'));
                sb.Append("_");
                sb.Append(DateTime.Now.Second.ToString().PadLeft(2, '0'));
                sb.Append(Path.GetExtension(filePath));
                sb.Append(".bak");

                File.Copy(filePath, sb.ToString(),true);
            }
	        catch (Exception e)
	        {
		        if(supressConsoleMessages == false)
                {
                    Console.WriteLine("Could not copy file to Backup folder: " + e.Message);
                }
                return;
	        }
            try 
	        {	        
		        DirectoryInfo info = new DirectoryInfo(backupsPath);
                
                //Get all the .bak files that are back ups of the file we're about to save,
                //in order of newest to oldest
                List<FileInfo> fileList = info
                    .GetFiles("*.bak", SearchOption.TopDirectoryOnly)
                    .Where(
                        delegate(FileInfo file)
                        {
                            string backupFile = Path.GetFileNameWithoutExtension(file.Name);
                            string fileToCopy = Path.GetFileNameWithoutExtension(filePath);

                            if (backupFile.StartsWith(fileToCopy))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }   
                        })
                    .Reverse()//TODO: This is unneeded
                    .ToList();

                //Delete all but the most recent n of them
                int amountToKeep = OptionsManager.Get<int>("G", "FILE_MAX_BACKUPS");
                for (int i = fileList.Count - 1; i >= amountToKeep; i--)
                {
                    File.Delete(fileList[i].FullName);
                }
	        }
	        catch (Exception)
	        {
		        //Its okay if the file doesn't delete
		        return;
	        }
        }

        /// <summary>
        /// A wrapper around StreamReader operations
        /// </summary>
        /// <param name="filePath">Full path to the file you want to open, assumed clean</param>
        /// <param name="readAsBinary">Reads the file as if it were all binary data</param>
        /// <param name="supressConsoleMessages">Blocks the printing of console messages (for when not in a terminal like mode)</param>
        /// <param name="changeLastUsed">If true it allows for the IDE's last used file</param>
        /// <returns>If reading is a success a List of Lists of ints containing the file contents
        /// will be returned, if there was a problem null will be returned</returns>
        public static List<List<int>> ReadFile(string filePath, bool readAsBinary, bool supressConsoleMessages, bool changeLastUsed)
        {
            //The stream for reading the file
            FileStream fStream = null;

            List<List<int>> fileContents = new List<List<int>>();
            fileContents.Add(new List<int>());

            try
            {
                //Read open the file
                fStream = File.OpenRead(filePath);

                //Attempt to read BOM Marks, test them
                byte[] BOMMarks = new byte[4];
                fStream.Read(BOMMarks, 0, 4);
                Encoding encoding = DetectBOMBytes(BOMMarks);
                
                //If we are not using any Unicode Encodings,
                //assume we are just using ANSI text files
                if (encoding == null)
                {
                    //Save the last encoding we're using
                    LastUsedEncoding = Encoding.Default.BodyName;
                    encoding = Encoding.Default;
                }
                else
                {
                    LastUsedEncoding = encoding.BodyName;
                }
                
                //Reset the file position and open up a stream reader
                fStream.Position = 0;
                using (StreamReader rStream = new StreamReader(fStream, Encoding.GetEncoding(LastUsedEncoding)))
                {
                    //If we're reading as binary files
                    //from the 'i' instruction probably
                    if (readAsBinary == true)
                    {
                        //Read all of the stream at once and you're done
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

                    //If we're opening a file with the menu system
                    //Change the last user opened path
                    if (changeLastUsed == true)
                    {
                        LastUserOpenedPath = Path.GetFullPath(filePath);
                    }
                }
            }
            catch (Exception e)
            {
                if (supressConsoleMessages == false)
                {
                    Console.WriteLine("Error reading: " + e.Message);
                }
                
                if (changeLastUsed == true)
                {
                    //Reset the LastUserOpenedPath to something safe
                    LastUserOpenedPath = "";
                }
                return null;
            }
            finally
            {
                //Make sure we close the stream
                if (fStream != null)
                {
                    fStream.Close();
                    fStream.Dispose();
                }
            }

            return fileContents;
        }

        /// <summary>
        /// A wraper around StreamWriter operations
        /// </summary>
        /// <param name="filePath">Full path to the file you want to open</param>
        /// <param name="outStrings">The list of strings you want to write out</param>
        /// <param name="writeAsBinary">Writes file as according to the Funge-98 spec on the 'o' writing as a binary file</param>
        /// <param name="supressConsoleMessages">Blocks the printing of console messages (for when not in a terminal like mode)</param>
        /// <param name="changeLastUsed">If true it allows for the IDE's last used file</param>
        /// <returns>Returns true if the file was written succesfully, false if not</returns>
        public static bool WriteFile(string filePath, List<List<int>> outStrings, bool supressConsoleMessages, bool changeLastUsed)
        {
            //The stream for writing the file
            StreamWriter wStream = null;
            try
            {
                //If the file exists we need to back it up so
                //just incase we nuke the file they haven't lost everything
                if (File.Exists(filePath) == true)
                {
                    BackupFile(filePath, supressConsoleMessages);
                }

                //Create the stream writer from the file path
                wStream = new StreamWriter(filePath, false, Encoding.GetEncoding(LastUsedEncoding));

                for (int r = 0; r < outStrings.Count; r++)
			    {
                    //Attempt to find the last space in the row
                    for (int c = 0; c < outStrings[r].Count; c++)
                    {
                        wStream.Write((char)outStrings[r][c]);
                    }
                    wStream.WriteLine();
			    }
                
                if (changeLastUsed == true)
                {
                    LastUserOpenedPath = Path.GetFullPath(filePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing: " + e.Message);

                //Reset the LastUserOpenedPath to something safe
                LastUserOpenedPath = "";

                return false;
            }
            finally
            {
                //Make sure we close the stream
                if (wStream != null)
                {
                    wStream.Close();
                }
            }
            return true;
        }//void WriteFile

        /// <summary>
        /// Tests whether a path is valid or not
        /// </summary>
        /// <param name="path">The path to test</param>
        /// <returns>Returns true if the path is a valid path</returns>
        public static bool IsValidPath(string path)
        {
            //We need atleast "."
            if (path == "")
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
        
        //Thank you to http://www.architectshack.com/TextFileEncodingDetector.ashx
        /*
         * Simple class to handle text file encoding woes (in a primarily English-speaking tech 
         *      world).
         * 
         *  - This code is fully managed, no shady calls to MLang (the unmanaged codepage
         *      detection library originally developed for Internet Explorer).
         * 
         *  - This class does NOT try to detect arbitrary codepages/charsets, it really only
         *      aims to differentiate between some of the most common variants of Unicode 
         *      encoding, and a "default" (western / ascii-based) encoding alternative provided
         *      by the caller.
         *      
         *  - As there is no "Reliable" way to distinguish between UTF-8 (without BOM) and 
         *      Windows-1252 (in .Net, also incorrectly called "ASCII") encodings, we use a 
         *      heuristic - so the more of the file we can sample the better the guess. If you 
         *      are going to read the whole file into memory at some point, then best to pass 
         *      in the whole byte byte array directly. Otherwise, decide how to trade off 
         *      reliability against performance / memory usage.
         *      
         *  - The UTF-8 detection heuristic only works for western text, as it relies on 
         *      the presence of UTF-8 encoded accented and other characters found in the upper 
         *      ranges of the Latin-1 and (particularly) Windows-1252 codepages.
         *  
         *  - For more general detection routines, see existing projects / resources:
         *    - MLang - Microsoft library originally for IE6, available in Windows XP and later APIs now (I think?)
         *      - MLang .Net bindings: http://www.codeproject.com/KB/recipes/DetectEncoding.aspx
         *    - CharDet - Mozilla browser's detection routines
         *      - Ported to Java then .Net: http://www.conceptdevelopment.net/Localization/NCharDet/
         *      - Ported straight to .Net: http://code.google.com/p/chardetsharp/source/browse
         *  
         * Copyright Tao Klerks, 2010-2012, tao@klerks.biz
         * Licensed under the modified BSD license:
         * 

        Redistribution and use in source and binary forms, with or without modification, are 
        permitted provided that the following conditions are met:

         - Redistributions of source code must retain the above copyright notice, this list of 
        conditions and the following disclaimer.
         - Redistributions in binary form must reproduce the above copyright notice, this list 
        of conditions and the following disclaimer in the documentation and/or other materials
        provided with the distribution.
         - The name of the author may not be used to endorse or promote products derived from 
        this software without specific prior written permission.

        THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, 
        INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR 
        A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY 
        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
        BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
        PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
        WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
        ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
        OF SUCH DAMAGE.
        */
        public static Encoding DetectBOMBytes(byte[] BOMBytes)
        {
            if (BOMBytes == null)
                throw new ArgumentNullException("Must provide a valid BOM byte array!", "BOMBytes");
 
            if (BOMBytes.Length < 2)
                return null;
 
            if (BOMBytes[0] == 0xff 
                && BOMBytes[1] == 0xfe 
                && (BOMBytes.Length < 4 
                    || BOMBytes[2] != 0 
                    || BOMBytes[3] != 0
                    )
                )
                return Encoding.Unicode;
 
            if (BOMBytes[0] == 0xfe 
                && BOMBytes[1] == 0xff
                )
                return Encoding.BigEndianUnicode;
 
            if (BOMBytes.Length < 3)
                return null;
 
            if (BOMBytes[0] == 0xef && BOMBytes[1] == 0xbb && BOMBytes[2] == 0xbf)
                return Encoding.UTF8;
 
            if (BOMBytes[0] == 0x2b && BOMBytes[1] == 0x2f && BOMBytes[2] == 0x76)
                return Encoding.UTF7;
 
            if (BOMBytes.Length < 4)
                return null;
 
            if (BOMBytes[0] == 0xff && BOMBytes[1] == 0xfe && BOMBytes[2] == 0 && BOMBytes[3] == 0)
                return Encoding.UTF32;
 
            if (BOMBytes[0] == 0 && BOMBytes[1] == 0 && BOMBytes[2] == 0xfe && BOMBytes[3] == 0xff)
                return Encoding.GetEncoding(12001);
 
            return null;
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
            string expanded = "";

            try
            {
                expanded = Environment.ExpandEnvironmentVariables(path);

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
            }
            catch (Exception)
            {
                return "";
            }
            return expanded;
        }

        #region Menu Commands
        /// <summary>
        /// Our CMD-like CD command
        /// </summary>
        /// <param name="input">What to try to parse for our CD command</param>
        /// <returns>
        /// Returns true if the input was valid for the CD command,
        /// false if not or if there was a problem
        /// </returns>
        public static bool CDCommand(string input)
        {
            input = input.Trim().ToLower();
            if (input == "cd")
            {
                FileUtils.DisplayCurrentDirectory();
                return true;
            }
            else if (input.StartsWith("cd ") && input.Length >= 3 + 1)//+1 because it needs to be atleast cd .
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
            }
            else
            {
                Console.WriteLine(input + " is not a valid path");
                Console.WriteLine();
            }
            return true;
        }
        
        /// <summary>
        /// Our CMD-like DIR command.
        /// Prints out a partial list of the most recently used files
        /// </summary>
        /// <param name="input">What to try to parse for our DIR command</param>
        /// <returns>
        /// True if the input was valid for the DIR command,
        /// false if not or if there was a problem
        /// </returns>
        public static bool DIRCommand(string input)
        {
            int start_index = 0;
            
            Match dir_match = Regex.Match(input.Trim().ToLower(), "dir ([0-9]+)$");
            if (dir_match.Success == true)
            {
                start_index = Convert.ToInt32(dir_match.Groups[1].Value);
            }
            else
            {
                start_index = 0;
            }

            int count = 10;
            
            //A collection of allowed extentions based on the language features supported
            List<string> allowedExtensions = new List<string>();

            //Always allow text files
            allowedExtensions.Add(".txt");

            int dimensions = OptionsManager.Get<int>("Interpreter","LF_DIMENSIONS");
            bool UF_SUPPORT = OptionsManager.Get<bool>("Interpreter", "LF_UF_SUPPORT") == true && dimensions >= 1;
            if(UF_SUPPORT)
            {
                allowedExtensions.Add(".uf");
            }

            bool BF93_SUPPORT = OptionsManager.Get<bool>("Interpreter", "LF_BF93_SUPPORT") && dimensions >= 2;
            bool BF98_SUPPORT = OptionsManager.Get<bool>("Interpreter", "LF_BF98_SUPPORT") && dimensions >= 2;
            
            //If either befunge is turned on
            if(BF93_SUPPORT || BF98_SUPPORT)
            {
                //Add the general .bf file extentions
                allowedExtensions.Add(".bf");

                //Test for individual support
                if(BF93_SUPPORT)
                {
                    allowedExtensions.Add(".b93");
                }
                if(BF98_SUPPORT)
                {
                    allowedExtensions.Add(".b98");
                }
            }

            bool TF_SUPPORT = OptionsManager.Get<bool>("Interpreter", "LF_TF_SUPPORT") && dimensions >= 3;

            if(TF_SUPPORT)
            {
                allowedExtensions.Add(".tf");
            }

            List<string> outList = new List<string>();
            //Thanks Marc! http://stackoverflow.com/a/30082323
            try
            {
                DirectoryInfo info = new DirectoryInfo(Directory.GetCurrentDirectory());
                List<FileInfo> fileList = info
                    .GetFiles("*", SearchOption.AllDirectories) //Get all the files with the allowed extensions,
                    .Where(file => allowedExtensions.Any(file.Extension.ToLower().EndsWith) || file.Extension == "")
                    //Sort by the last access time so they appear on the top of the list and will get chosen first
                    .OrderBy(f => f.LastAccessTime)
                    .Reverse()
                    .ToList();

                if (start_index >= fileList.Count == true)
                {
                    Console.WriteLine("Found {0} files, start index {1} too high, displaying nothing", fileList.Count, start_index);
                }
                else
                {
                    Console.WriteLine("Found {0} files, displaying up to {1} starting at index {2}:", fileList.Count, count, start_index);
                }
                for (int i = start_index; i < fileList.Count && i < start_index + count; i++)
                {
                    outList.Add(fileList[i].FullName.Remove(0, Directory.GetCurrentDirectory().Count() + 1));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine();
                return false;
            }

            foreach (var fileName in outList)
            {
                Console.WriteLine(fileName);
            }
            Console.WriteLine();
            return true;
        }

        /// <summary>
        /// Prints out the help text for using our cmd like commands
        /// </summary>
        /// <returns>Returns true always</returns>
        public static bool HelpCommand()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("back - Goes back to the previous window");
            Console.WriteLine("dir - Prints out some of the most recently used files");
            Console.WriteLine("dir index - Like dir, but allowing you to print out other parts of the list of files. Ex: dir 5");
            Console.WriteLine("cd - Prints the current working directory");
            Console.WriteLine("cd path -  Attempts to set the current working directory to the path argument. Ex: cd C:\\");
            Console.WriteLine("last - Prints the last used file, if there is one");
            Console.WriteLine("use last - Attempts to use the last used file");
            Console.WriteLine("clear last - Clears the last used file, if there is one");
            Console.WriteLine("help - Brings up this information");
            return true;
        }

        /// <summary>
        /// Tries to run our LAST command,
        /// printing out the last used file path
        /// </summary>
        /// <returns>
        /// Returns true if the input was the last command,
        /// false if not
        /// </returns>
        public static bool LastCommand()
        {
            //We never tell people that we're using the exe as a technicality
            if (FileUtils.LastUserOpenedPath == "")
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

        public static void ClearLast()
        {
            //This part is just to avoid user confusion
            if (FileUtils.LastUserOpenedPath == "")
            {
                Console.WriteLine("No files have been used this session");
            }
            else
            {
                Console.WriteLine("Last used file has been cleared");
            }
            FileUtils.LastUserOpenedPath = "";
            Console.WriteLine();
        }

        /// <summary>
        /// Checks if the input is matches the USE LAST command
        /// </summary>
        /// <returns>
        /// Returns the LastUserOpenedPath if it is not the default, or "" if it is the default
        /// </returns>
        public static string UseLastCommand()
        {
            //We never allow opening the default
            if (FileUtils.LastUserOpenedPath == "")
            {
                Console.WriteLine("No last used file to use");
                Console.WriteLine();
                return  "";
            }
            return FileUtils.LastUserOpenedPath;
        }
                
        /// <summary>
        /// Displays the current directory
        /// </summary>
        public static void DisplayCurrentDirectory()
        {
            //Takes care of a silly asthetic problem where it would print out 
            //C:\\ instead of C:\
            if (Directory.GetCurrentDirectory().Length == 3)
            {
                Console.WriteLine("Currently in " + Directory.GetCurrentDirectory());
            }
            else
            {
                Console.WriteLine("Currently in " + Directory.GetCurrentDirectory() + '\\');
            }
            Console.WriteLine();
        }
        #endregion
    }
}
