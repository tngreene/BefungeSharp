using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpConfig;

namespace BefungeSharp
{
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
            
            //  - If the file does not exist, create the default options file
            //2. Read, validate, and store the existing file rules
            //  - Use default (for this session) in any place
        }

        /// <summary>
        /// Creates the default options configuration
        /// </summary>
        /// <returns>
        /// 
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
            //  LANGUAGE - string (UF,BF93,F98,TF), sets many presets of other options
            //  SANDBOX_ENABLED - bool, if true i,o,= and the like are disabled
            //Debugger
            Configuration defaultOptions = new Configuration();
            
            defaultOptions["General"].Comment = new Comment("Settings that affect the whole program",'#');
            defaultOptions["General"]["DEFAULT_ENCODING"].SetValue<string>("UTF8");
            defaultOptions["General"]["DEFAULT_ENCODING"].Comment = new Comment("string, Encoding of the program",'#');
            
            defaultOptions["Editor"].Comment = new Comment("Settings that affect edit mode",'#');
            defaultOptions["Editor"]["SNIPPET"].SetValue<string>(">:#,_");
            defaultOptions["Editor"]["SNIPPET"].Comment = new Comment("string, a single line of Funge code",'#');

            defaultOptions["Visualizer"].Comment = new Comment("Settings the control how FungeSpace and data is visualized", '#');
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
            defaultOptions["Visualizer"]["GRID_YOFFSET"].SetValue<int>(5);

            return defaultOptions;
        }
    }
}
