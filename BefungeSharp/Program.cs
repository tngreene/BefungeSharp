using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using BefungeSharp.FungeSpace;
namespace BefungeSharp
{
    public class Program
    {
        private static BoardManager s_board_manager;
        public static BoardManager BoardManager { get { return s_board_manager; } set { s_board_manager = value; } }

        //TODO - remove the setters from these things!
        private static WindowUI s_window_UI;
        public static WindowUI WindowUI { get { return s_window_UI; } set { s_window_UI = value; } }

        private static WindowSideBar s_window_side_bar;
        public static WindowSideBar WindowSideBar { get { return s_window_side_bar; } set { s_window_side_bar = value; } }

        private static Interpreter s_interpreter;
        public static Interpreter Interpreter 
        { 
            get { return s_interpreter; }
            set { s_interpreter = value; }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            //Initiallize static classes in order
            //1. OptionsManger
            //2. FileUtils
            Console.WriteLine("Loading " + OptionsManager.OptionsFileName);
            FileUtils.DisplayCurrentDirectory();

            //Size of field + border + width_of_sidebar
            int width_of_sidebar = 36;//Seems right?
            Console.Title = "BefungeSharp, the Premier Funge-98 IDE for Windows";
            ConEx.ConEx_Draw.Init(80 + 1 + width_of_sidebar, 32);
            
            ConEx.ConEx_Input.Init(1000);
            ConEx.ConEx_Input.TreatControlCAsInput = true;

            Menus.MainMenu mainMenu = new Menus.MainMenu();
            mainMenu.RunLoop();
            QuitProgram(0);
        }//Main(string[]args)

        //TODO:Is this necissarly the best solution or is it a hack?
        //Feels like a little bit of a hack
        /// <summary>
        /// Quits the program early,
        /// allowing for any last minute clean-up or actions to be run
        /// </summary>
        /// <param name="exit_code">What code you want to program to exit with</param>
        public static void QuitProgram(int exit_code)
        {
            //Clear the screen
            ConEx.ConEx_Draw.FillScreen(' ');
            ConEx.ConEx_Draw.DrawScreen();

            //Ask if the person would like to save their options
            if (OptionsManager.SessionOptionsChanged == true)
            {
                Console.WriteLine("Save changed options? Y/N");
                char input = Console.ReadKey().KeyChar.ToString().ToLower()[0];
                Console.WriteLine();
                if (input == 'y')
                {
                    try
                    {
                        OptionsManager.SessionOptions.SaveToFile("options.ini", Encoding.UTF8);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Could not save options: " + e.Message);
                    }
                }
            }
            Environment.Exit(exit_code);
        }
    }//class Program
}//Namespace BefungeSharp