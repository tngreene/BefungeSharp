using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpConfig;

namespace BefungeSharp
{
    [Flags]
    public enum RuntimeFeatures : int
    {
        NULL = 0x000,

        //Referenced in the language spec
        CONCURRENT_FUNGE = 0x001,
        FILE_INPUT = 0x002,
        FILE_OUTPUT = 0x004,
        EXECUTE = 0x008,
        UNBUFFERED_IO = 0x010,

        //Other language features (not referenced in the spec)
        NETWORKING = 0x020,

        //Language and spec version selection, default to _98
        UF = 0x040, //1D Funge-98
        BF = 0x080, //2D Funge-98
        TF = 0x100, //3D Funge-98
        VERSION_93 = 0x200, //Befunge-93 compatability mode
        VERSION_98 = 0x400  //IDE default
    }

    /// <summary>
    /// Represents a manager for gettings and setting program options
    /// </summary>
    public static class OptionsManager
    {
        /// <summary>
        /// True if session options has unsaved changes
        /// </summary>
        public static bool SessionOptionsChanged { get; private set; }
        
        /// <summary>
        /// The global program options chosen for this session
        /// </summary>
        public static SharpConfig.Configuration SessionOptions { get; private set; }
        
        /// <summary>
        /// The defaults options for the program.
        /// The property is not connected to anything with state
        /// </summary>
        public static SharpConfig.Configuration DefaultOptions { get { return CreateDefaultOptions(); } }
        
        /// <summary>
        /// The name of the options file
        /// </summary>
        public static string OptionsFileName { get { return "options.ini"; } }

        static OptionsManager()
        {
            //0. Create the default options dictionary
            SessionOptions = DefaultOptions;
            
            //1. Attempt to open the options file
            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\");

                //Can be disabled for testing
                SessionOptions = SharpConfig.Configuration.LoadFromFile(OptionsManager.OptionsFileName);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("options.ini could not be found, creating new copy from defaults");
                SessionOptions.SaveToStream(File.Create(OptionsManager.OptionsFileName));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Attempts to get the value of a setting based on the string,
        /// Searches through SessionOptions, then Default Options
        /// </summary>
        /// <typeparam name="T">The data type you expect to get back</typeparam>
        /// <param name="section">The name or first letter of the section</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The data inside of the setting</returns>
        public static T Get<T>(string section, string name)
        {
			//Compare the first character of the section name to our list
			switch(section)
			{
                case "General":
                case "G":
                    section = "General";
                    break;
                case "Editor":
                case "E":
                    section = "Editor";
                    break;
                case "Visualizer":
                case "V":
                    section = "Visualizer";
                    break;
                case "Interpreter"://This or section.toUpper()[0]?
                case "I":
                    section = "Interpreter";
                    break;
                default:
                    throw new Exception("Section: " + section + "not found!");
                    break;
			}

            //If the section and setting exists, return it
            //otherwise, try finding it in the defaults
            if (SessionOptions.Contains(section) == true)
            {
                if (SessionOptions[section].Contains(name) == true)
                {
                    return SessionOptions[section][name].GetValueTyped<T>();
                }
            }
            else if (DefaultOptions.Contains(section) == true)
            {
                if (DefaultOptions[section].Contains(name) == true)
                {
                    return DefaultOptions[section][name].GetValueTyped<T>();
                }
            }
            else
            {
                //We're in trouble.
                throw new Exception("Section: " + section + " Setting: " + name + " not found!");
            }

            //Technically we'll never reach here, but VS doesn't know that for sure
            return default(T);
        }

        /// <summary>
        /// Attempts to set a setting with a particular value
        /// </summary>
        /// <typeparam name="T">The data type to set</typeparam>
        /// <param name="section">The name or first letter of the section</param>
        /// <param name="name">The name of the setting</param>
        /// <param name="value">The new value to give the setting</param>
        public static void Set<T>(string section, string name, T value)
        {
            switch(section.ToUpper()[0])
			{
				case 'G':
					section = "General";
					break;
				case 'E':
					section = "Editor";
                    break;
				case 'V':
					section = "Visualizer";
					break;
				case 'I':
					section = "Interpreter";
					break;
				default:
					throw new Exception("Section: " + section + "not found!");
					break;
			}

            string exceptionString = "";
            if (SessionOptions.Contains(section) == true)
            {
                if (SessionOptions[section].Contains(name) == true)
                {
                    SessionOptions[section][name].SetValue<T>(value);
                    SessionOptionsChanged = true;
                    return;
                }
                exceptionString += "Setting: " + name + " does not exist!";
                return;
            }
            exceptionString.Insert(0, "Section: " + section + " ");
            throw new Exception(exceptionString);
        }

        /// <summary>
        /// A much needed wrapper around adding a setting to a configuration
        /// </summary>
        /// <typeparam name="T">The type of data to store</typeparam>
        /// <param name="config">The configuration to apply it to</param>
        /// <param name="section">The section name</param>
        /// <param name="name">The setting name</param>
        /// <param name="value">The value to store</param>
        /// <param name="comment">The comment for the setting (default value of "")</param>
        /// <param name="symbol">The comment symbol for the comment (default value of ';'</param>
        private static void AddSetting<T>(this Configuration config, string section, string name, T value, string comment = "", char symbol = ';')
        {
            config[section][name].SetValue<T>(value);
            config[section][name].Comment = new Comment(comment, symbol);
        }
        
        public static void ResetSessionOptions()
        {
            OptionsManager.SessionOptions = OptionsManager.DefaultOptions;
        }
        /// <summary>
        /// Creates the default options configuration
        /// </summary>
        /// <returns>
        /// The default options configuration
        /// </returns>
        private static Configuration CreateDefaultOptions()
        {
			Configuration.ValidCommentChars = new char[]{ ';' };
            //Rather than make a whole bunch of method calls we'll just
            //statically write the file as a raw string and adjust as
            //needed. It also makes commenting easier
            return Configuration.LoadFromString(
            @"  ; Settings namespaces
                ; General
                ;   FILE - General File System settings
                ; Editor
                ;   UI - Editor User Interface
                ; Visualizer
                ;   COLOR - Syntax Highlighting
                ;   GRID - Viewport Movement along a grid
                ; Interpreter
                ;   LF - Languages and Features
                ;   RT - Runtime Behaviors
                ;   FS - FungeSpace settings
                ; Settings that affect the whole program
                [General]
                FILE_BACKUPS_FOLDER=Backups ; 
                FILE_MAX_BACKUPS=3 ; 
                FILE_ENCODING=utf-8 ; See https://msdn.microsoft.com/en-us/library/system.text.encoding%28v=vs.110%29.aspx for possible values
                FILE_LAST_USED= ; Settings that affect edit mode

                [Editor] ; Settings that affect edit mode
                UI_SNIPPET_N=^:# !#,| ;  a single line of Funge code, chosen when the IP's delta is north
                UI_SNIPPET_E=>:#,_ ;  a single line of Funge code, chosen when the IP's delta is east
                UI_SNIPPET_S=v:#,| ;  a single line of Funge code, chosen when the IP's delta is south
                UI_SNIPPET_W=<:# !#,_ ;  a single line of Funge code, chosen when the IP's delta is west
                UI_INVERSE_SCROLLING=False ; While moving around FungeSpace in Edit mode the controls are reversed
                GRID_XOFFSET=16 ; The number of cells to shift the view port horizontally. Should be a multiple of 16, or 1
                GRID_YOFFSET=5 ; The number of cells to shift the view port vertically. Should be a multiple of 5, or 1

                [Visualizer] ; Settings the control how FungeSpace and data is visualized
                COLOR_SYNTAX_HIGHLIGHTING=True ; Enables or disables syntax highlighting
                ; Only ConsoleColors are supported
                COLOR_Arithmetic=Green
                COLOR_Concurrent=Blue
                COLOR_DataStorage=Green
                COLOR_FlowControl=Cyan
                COLOR_FileIO=Gray
                COLOR_Fingerprint=DarkRed
                COLOR_Logic=DarkGreen
                COLOR_Movement=Cyan
                COLOR_Nop=DarkBlue
                COLOR_NotImplemented=DarkRed
                COLOR_Number=Magenta
                COLOR_StackManipulation=DarkYellow
                COLOR_StackStackManipulation=Yellow
                COLOR_StopExecution=Red
                COLOR_StdIO=DarkGray
                COLOR_String=DarkYellow
                COLOR_System=DarkMagenta
                COLOR_Trefunge=DarkRed

                ; Settings for the interpreter to use at runtime
                [Interpreter]
                LF_CONCURRENCY=True ; Enables the 't' instruction
                LF_FILE_INPUT=True ; Enables the 'i' instruction. Potentially unsafe
                LF_FILE_OUTPUT=True ; Enables the 'o' instruction. Potentially unsafe
                LF_EXECUTE_STYLE=1 ; 0 for none, 1 for system() calls, 2 specific programs, 3 for this running shell. Currently using 1
                LF_STD_INPUT_STYLE=1 ; 0 for unbuffered, 1 for buffered. For now use 1
                LF_STD_OUTPUT_STYLE=0 ; 0 for unbuffered, 1 for buffered. For now use 0
                LF_NETWORKING=False ; Enables BefungeSharp to make Network connections. Currently unimplemented
                LF_UF_SUPPORT=True ; Unfunge is supported
                LF_BF93_SUPPORT=True ; Befunge-93 is supported
                LF_BF98_SUPPORT=True ; Befunge-98 is supported
                LF_TF_SUPPORT=False ; Trefunge is not supported
                LF_DIMENSIONS=2 ; The current number of dimensions to use
                LF_SPEC_VERSION=98 ; The current spec to use, must be supported
                RT_DEFAULT_MODE=Edit ; The default mode the program goes into after opening a file
                RT_STRICT_BF93=False ; If 'g', 'p' used out of bounds ends the interpreter
                FS_DEFAULT_AREA_WIDTH=80 ; The default width of pre-allocated FungeSpace, must be atleast 80 and a multiple of 16
                FS_DEFAULT_AREA_HEIGHT=25 ; The default width of pre-allocated FungeSpace, must be atleast 25 and a multiple of 5");
        }
    }
}
