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
        
        static OptionsManager()
        {
            //0. Create the default options dictionary
            SessionOptions = DefaultOptions;
            
            //1. Attempt to open the options file
            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\");

                //Can be disabled for testing
                SessionOptions = SharpConfig.Configuration.LoadFromFile("options.ini");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("options.ini could not be found, creating new copy from defaults");
                SessionOptions.SaveToStream(File.Create("options.ini"));
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
        /// <param name="section">The name of the section</param>
        /// <param name="name">The name of the setting</param>
        /// <returns>The data inside of the setting</returns>
        public static T Get<T>(string section, string name)
        {
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

            //We're in trouble.
            throw new Exception("Section: " + section + " Setting: " + name + " not found!");
            
            //Technically we'll never reach here, but VS doesn't know that for sure
            return default(T);
        }

        /// <summary>
        /// Attempts to set a setting with a particular value
        /// </summary>
        /// <typeparam name="T">The data type to set</typeparam>
        /// <param name="section">The name of the section</param>
        /// <param name="name">The name of the setting</param>
        /// <param name="value">The new value to give the setting</param>
        public static void Set<T>(string section, string name, T value)
        {
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
        /// <summary>
        /// Creates the default options configuration
        /// </summary>
        /// <returns>
        /// The default options configuration
        /// </returns>
        private static Configuration CreateDefaultOptions()
        {
            //Settings namespaces
            //General
            //  FILE - General File System settings
            //Editor
            //  UI - Editor User Interface
            //Visualizer - Visualizer
            //  COLOR - Syntax Highlighting
            //  GRID - Viewport Movement along a grid
            //INTP - Interpreter
            //  LF - Languages and Features
            //  RT - Runtime Behaviors
            //  FS - FungeSpace settings
            Configuration config = new Configuration();
            config["General"].Comment = new Comment("Settings that affect the whole program", ';');
            config.AddSetting<string>("General","FILE_BACKUPS_FOLDER","Backups");
            config.AddSetting<int>   ("General","FILE_MAX_BACKUPS",3);
            config.AddSetting<string>("General","FILE_ENCODING","utf-8","See https://msdn.microsoft.com/en-us/library/system.text.encoding%28v=vs.110%29.aspx for possible values");
            config.AddSetting<string>("General","FILE_LAST_USED","","Settings that affect edit mode");

            config["Editor"].Comment = new Comment("Settings that affect edit mode", ';');
            config.AddSetting<string>("Editor","UI_SNIPPET_N","^:# !#,|"," a single line of Funge code, chosen when the IP's delta is north");
            config.AddSetting<string>("Editor","UI_SNIPPET_E",">:#,_"," a single line of Funge code, chosen when the IP's delta is east");
            config.AddSetting<string>("Editor","UI_SNIPPET_S","v:#,|"," a single line of Funge code, chosen when the IP's delta is south");
            config.AddSetting<string>("Editor","UI_SNIPPET_W","<:# !#,_"," a single line of Funge code, chosen when the IP's delta is west");
            config.AddSetting<bool>  ("Editor","UI_INVERSE_SCROLLING",false,"While moving around FungeSpace in Edit mode the controls are reversed");
            
            config["Visualizer"].Comment = new Comment("Settings the control how FungeSpace and data is visualized\r\n"+
                                                       "\t\t\t  ; Only ConsoleColors are supported", ';');
            
            Setting[] code_highlights = new Setting[] { 
                new Setting("COLOR_Arithmetic",             "Green"),
                new Setting("COLOR_Concurrent",             "Blue"),
                new Setting("COLOR_DataStorage",            "Green"),
                new Setting("COLOR_FlowControl",            "Cyan"),
                new Setting("COLOR_FileIO",                 "Gray"),
                new Setting("COLOR_Logic",                  "DarkGreen"),
                new Setting("COLOR_Movement",               "Cyan"),
                new Setting("COLOR_Nop",                    "DarkBlue"),
                new Setting("COLOR_NotImplemented",         "DarkRed"),
                new Setting("COLOR_Number",                 "Magenta"),
                new Setting("COLOR_StackManipulation",      "DarkYellow"),
                new Setting("COLOR_StackStackManipulation", "Yellow"),
                new Setting("COLOR_StopExecution",          "Red"),
                new Setting("COLOR_StdIO",                  "DarkGray"),
                new Setting("COLOR_String",                 "DarkYellow"),
                new Setting("COLOR_System",                 "DarkMagenta"),
                new Setting("COLOR_Trefunge",               "DarkRed")
            };

            foreach (var setting in code_highlights)
            {
                config["Visualizer"].Add(setting);
            }
            config.AddSetting<int>("Visualizer","GRID_XOFFSET",16,"The number of cells to shift the view port horizontally. Should be a multiple of 16, or 1");
            config.AddSetting<int>("Visualizer","GRID_YOFFSET", 5,"The number of cells to shift the view port vertically. Should be a multiple of 5, or 1");
            
            config["Interpreter"].Comment = new Comment("Settings for the interpreter to use at runtime\r\n" +
                                                    "\t\t\t  ; Only enable a single dimension and a single version", ';');

            //LF stands for languages and features
            config.AddSetting<bool>("Interpreter","LF_CONCURRENCY",     true,"Enables the 't' instruction");
            config.AddSetting<bool>("Interpreter","LF_FILE_INPUT",      true,"Enables the 'i' instruction. Potentially unsafe");
            config.AddSetting<bool>("Interpreter","LF_FILE_OUTPUT",     true,"Enables the 'o' instruction. Potentially unsafe");
            config.AddSetting<int> ("Interpreter","LF_EXECUTE_STYLE",      1,"0 for none, 1 for system() calls, 2 specific programs, 3 for this running shell. Currently using 1");
            config.AddSetting<int> ("Interpreter","LF_STD_INPUT_STYLE",    1,"0 for unbuffered, 1 for buffered. For now use 1");
            config.AddSetting<int> ("Interpreter","LF_STD_OUTPUT_STYLE",   0,"0 for unbuffered, 1 for buffered. For now use 0");
            config.AddSetting<bool>("Interpreter","LF_NETWORKING",     false,"Enables BefungeSharp to make Network connections. Currently unimplemented");
            config.AddSetting<bool>("Interpreter","LF_UF_SUPPORT",      true,"Unfunge is supported");
            config.AddSetting<bool>("Interpreter","LF_BF93_SUPPORT",   false, "Befunge-93 is not supported");
            config.AddSetting<bool>("Interpreter","LF_BF98_SUPPORT",    true, "Befunge is supported");
            config.AddSetting<bool>("Interpreter","LF_TF_SUPPORT",     false,"Trefunge is not supported");
            config.AddSetting<int> ("Interpreter","LF_DIMENSIONS",         2,"The current number of dimensions to use");
            config.AddSetting<int> ("Interpreter","LF_SPEC_VERSION",      98,"The current spec to use, must be supported");
            
            //RT stands for runtime behaviors
            config.AddSetting<BoardMode>("Interpreter","RT_DEFAULT_MODE",BoardMode.Edit,"The default mode the program goes into after opening a file");
            //FS stands for FungeSpace
            config.AddSetting<int>      ("Interpreter","FS_DEFAULT_AREA_WIDTH", 80,"The default width of pre-allocated FungeSpace, must be atleast 80 and a multiple of 16");
            config.AddSetting<int>      ("Interpreter","FS_DEFAULT_AREA_HEIGHT",25,"The default width of pre-allocated FungeSpace, must be atleast 25 and a multiple of 5");
            return config;
        }
    }
}
