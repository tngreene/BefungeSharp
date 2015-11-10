using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BefungeSharp.Menus
{
    public class MainMenu : Menu
    {
        public override void OnOpening()
        {
            base.OnOpening();
                      
            //A silly bit of fun to make the menu look cooler
            Console.ForegroundColor = (ConsoleColor) new Random().Next(10,15);
            /*Console.CursorLeft = 51;
            Console.WriteLine("Main Menu");
            Console.WriteLine(new String('_', Console.WindowWidth));
            Console.WriteLine("\r\n");*/
            //Ask what they would like to do
            Console.WriteLine("Welcome to BefungeSharp! Please select an option:\n");

            int menu_index = 0;
            Console.WriteLine(++menu_index + ".) New File");
            Console.WriteLine(++menu_index + ".) Open File");
            Console.WriteLine(++menu_index + ".) Current Language Version: " + OptionsManager.Get<int>("Interpreter","LF_SPEC_VERSION"));
            Console.WriteLine(++menu_index + ".) Help");
            Console.WriteLine(++menu_index + ".) New Menu Color");
            Console.WriteLine(++menu_index + ".) Exit");


            Console.WriteLine("\r\nEnter a number between 1 - 6");
        }

        public override void RunLoop()
        {
            OnOpening();
            
            char input = Menu.WaitForInput('1', '6');

            Console.Clear();
            switch (input - 48)
            {
                case 1://New File
                        List<List<int>> s = new List<List<int>>();
                        //s.Add(@"""dlroW olleH"">:#,_@");
                        //s.Add(@"12341234");
                        //s_board_manager = new BoardManager(25, 80, s);
                        Program.BoardManager = new BoardManager(s,null,OptionsManager.Get<BoardMode>("Interpreter","RT_DEFAULT_MODE"));
                        Program.BoardManager.UpdateBoard();
                    break;
                case 2://Open File
                    Menus.OpenSimpleMenu openSimple = new OpenSimpleMenu();
                    openSimple.RunLoop();
                    break;
                case 3://Options
                    //new OptionsMenuTree.OptionsMenuTop().RunLoop();
                    break;
                case 4://Help
                    Menus.HelpMenu helpMenu = new HelpMenu();
                    helpMenu.RunLoop();
                    break;
                case 5://Advanced Open File, currently "New Menu Color
                    break;
                case 6://Exit
                    return;
            }
            OnClosing();
        }

        public override void OnClosing()
        {
            //Prevent the main menu from closing unless one explictly 
            //inputs the exit command
            RunLoop();
        }
    }
}
