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
        static int Main(string[] args)
        {
            //Size of field + border + width_of_sidebar
            int width_of_sidebar = 36;//Seems right?
            var k = OptionsManager.DefaultOptions;
            Console.Title = "BefungeSharp, the Premier Funge-98 IDE for Windows";
            ConEx.ConEx_Draw.Init(80 + 1 + width_of_sidebar, 32);
            
            ConEx.ConEx_Input.Init(1000);
            ConEx.ConEx_Input.TreatControlCAsInput = true;

            Menus.MainMenu mainMenu = new Menus.MainMenu();
            mainMenu.RunLoop();
            return 0;
        }//Main(string[]args)
    }//class Program
}//Namespace BefungeSharp