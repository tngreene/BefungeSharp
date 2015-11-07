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
        /// The global program options chosen for this session
        /// </summary>
        public static SharpConfig.Configuration SessionOptions { get; private set; }
        
        /// <summary>
        /// The global program option defaults
        /// </summary>
        public static SharpConfig.Configuration DefaultOptions { get; private set; }
        
        static OptionsManager()
        {
            //0. Create the default options dictionary
            DefaultOptions = SessionOptions = CreateDefaultOptions();
            //1. Attempt to open the options file
            try
            {
                //SessionOptions = SharpConfig.Configuration.LoadFromFile("options.ini");


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                SessionOptions.SaveToStream(File.Create("options.ini"));
            }
            
            
            //If the options file does not exist
            if (SessionOptions == null)
            {
                //Create options.ini
                
            }
            else
            {
                //2. Read, validate, and store the existing file rules
                //  - Use default (for this session) in any place
                
            }           
        }

        /// <summary>
        /// Creates the default options configuration
        /// </summary>
        /// <returns>
        /// The default options configuration
        /// </returns>
        private static Configuration CreateDefaultOptions()
        {
            //General - Whole Program Settings
            //  DEFAULT_ENCODING
            //Editor - For code editing, saving, and opening
            //  SNIPPET - string, a snipped of code that can be inserted
            //Visualizer - Showing FungeSpace and WindowUI to the user
            //  SYNTAX_HIGHLIGHT_<instruction_type> - int, corresponding to color
            //  GRID_XOFFSET - int, width of snapping grid in cells
            //  GRID_YOFFSET - int, height of snapping grid in cells
            //Interpreter - Executing Funge instructions and running the language
            //  LANGUAGE AND FEATURES - string (UF,BF93,F98,TF), sets many presets of other options
            //  FUNGESPACE SETTINGS - for optimization and sizes
            //Debugger
            Configuration defaultOptions = new Configuration();
            
            defaultOptions["General"].Comment = new Comment("Settings that affect the whole program",';');
            defaultOptions["General"]["DEFAULT_ENCODING"].SetValue<string>("UTF8");
            defaultOptions["General"]["DEFAULT_ENCODING"].Comment = new Comment("string, Encoding of the program",';');
            
            defaultOptions["Editor"].Comment = new Comment("Settings that affect edit mode",';');
            defaultOptions["Editor"]["SNIPPET"].SetValue<string>(">:\\#,_");
            defaultOptions["Editor"]["SNIPPET"].Comment = new Comment("string, a single line of Funge code",';');

            defaultOptions["Visualizer"].Comment = new Comment("Settings the control how FungeSpace and data is visualized", ';');
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
                defaultOptions["Visualizer"].Add(setting); 
            }

            defaultOptions["Visualizer"]["GRID_XOFFSET"].SetValue<int>(16);
            defaultOptions["Visualizer"]["GRID_XOFFSET"].Comment = new Comment("The number of cells to shift when shifting the view port horizontally, should be a multiple of RuntimeFeatures.6", ';');
            
            defaultOptions["Visualizer"]["GRID_YOFFSET"].SetValue<int>(5);
            defaultOptions["Visualizer"]["GRID_YOFFSET"].Comment = new Comment("The number of cells to shift when shifting the view port vertically, should be a multiple of 5", ';');

            
            defaultOptions["Interpreter"].Comment = new Comment("Settings for the interpreter to use at runtime\r\n" +
                                                                "\t\t\t  ;Only enable a single dimension and a single version",';');
                                                                                                            
            //LF stands for Languages and Features, a sort of namespace-ing
            defaultOptions["Interpreter"]["LF_CONCURRENT_FUNGE"].SetValue<bool>(true);
            defaultOptions["Interpreter"]["LF_CONCURRENT_FUNGE"].Comment = new Comment("Enables the 't' instruction", ';');
            defaultOptions["Interpreter"]["LF_FILE_INPUT"].SetValue<bool>(true);
            defaultOptions["Interpreter"]["LF_FILE_INPUT"].Comment = new Comment("Enables the 'i' instruction. Potentially unsafe", ';');
            defaultOptions["Interpreter"]["LF_FILE_OUTPUT"].SetValue<bool>(true);
            defaultOptions["Interpreter"]["LF_FILE_OUTPUT"].Comment = new Comment("Enables the 'o' instruction. Potentially unsafe", ';');
            defaultOptions["Interpreter"]["LF_EXECUTE_STYLE"].SetValue<int>(1);
            defaultOptions["Interpreter"]["LF_EXECUTE_STYLE"].Comment = new Comment("0 for none, 1 for system() calls, 2 specific programs, 3 for this running shell. Currently using 1",';');
            defaultOptions["Interpreter"]["LF_STD_INPUT_STYLE"].SetValue<int>(1);
            defaultOptions["Interpreter"]["LF_STD_INPUT_STYLE"].Comment = new Comment("0 for unbuffered, 1 for buffered. For now use 1", ';');
            defaultOptions["Interpreter"]["LF_STD_OUTPUT_STYLE"].SetValue<int>(0);
            defaultOptions["Interpreter"]["LF_STD_OUTPUT_STYLE"].Comment = new Comment("0 for unbuffered, 1 for buffered. For now use 0", ';');
            defaultOptions["Interpreter"]["LF_NETWORKING"].SetValue<bool>(false);
            defaultOptions["Interpreter"]["LF_NETWORKING"].Comment = new Comment("Enables BefungeSharp to make Network connections. Currently unimplemented", ';');
            
            defaultOptions["Interpreter"]["LF_UF"].SetValue<bool>(true);
            defaultOptions["Interpreter"]["LF_UF"].Comment = new Comment("Unfunge, 1D funge, is supported", ';');
            defaultOptions["Interpreter"]["LF_BF"].SetValue<bool>(true);
            defaultOptions["Interpreter"]["LF_BF"].Comment = new Comment("Befunge, 2D funge, is supported", ';');
            defaultOptions["Interpreter"]["LF_TF"].SetValue<bool>(false);//One day...
            defaultOptions["Interpreter"]["LF_TF"].Comment = new Comment("Trefunge, 3D funge, is currently not supported", ';');
            defaultOptions["Interpreter"]["LF_DIMENSIONS"].SetValue<int>(2);

            //Choose 93 or 98, not both
            defaultOptions["Interpreter"]["LF_SPEC_VERSION"].SetValue<int>(98);
            defaultOptions["Interpreter"]["LF_SPEC_VERSION"].Comment = new Comment("Possible values are 93 or 98. \"93\" is a Befunge-93 compatability mode", ';');

            //RT_ is out Runtime options
            defaultOptions["Interpreter"]["RT_DEFAULT_MODE"].SetValue<BoardMode>(BoardMode.Edit);
            
            //FS_ is our FungeSpace option namespace
            defaultOptions["Interpreter"]["FS_DEFAULT_AREA_WIDTH"].SetValue<int>(80);
            defaultOptions["Interpreter"]["FS_DEFAULT_AREA_WIDTH"].Comment = new Comment("The default width of pre-allocated FungeSpace, must be atleast 80 and a multiple of 16", ';');
            defaultOptions["Interpreter"]["FS_DEFAULT_AREA_HEIGHT"].SetValue<int>(25);
            defaultOptions["Interpreter"]["FS_DEFAULT_AREA_WIDTH"].Comment = new Comment("The default width of pre-allocated FungeSpace, must be atleast 25 and a multiple of 5", ';');
            
            return defaultOptions;
        }
    }
}
