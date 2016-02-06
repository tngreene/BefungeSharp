using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using ConEx;

namespace BefungeSharp.Menus
{
    public class MainMenu : Menu
    {
        public override void OnOpening()
        {
            //A silly bit of fun to make the menu look cooler
            Console.ForegroundColor = (ConsoleColor)new Random().Next(10, 15);
            base.OnOpening();

            DecorateWindowTop("Main Menu");
            
            //Ask what they would like to do
            Console.WriteLine("Welcome to BefungeSharp!\r\n");

            int menu_index = 0;
            Console.WriteLine(++menu_index + ".) New File");
            Console.WriteLine(++menu_index + ".) Open File");
            Console.WriteLine(++menu_index + ".) Options");
            Console.WriteLine(++menu_index + ".) Help");
            Console.WriteLine(++menu_index + ".) New Menu Color");
            Console.WriteLine(++menu_index + ".) Exit");
            
            Console.WriteLine("\r\nEnter a number between 1 - " + menu_index);
        }

        public override void RunLoop()
        {
            OnOpening();
            
            int input = ConEx_Input.WaitForIntInRange(1,6, true, "Value must be between 1 and 6");

            Console.Clear();
            switch (input)
            {
                case 1://New File
                        List<List<int>> s = new List<List<int>>();
                        //s.Add(@"""dlroW olleH"">:#,_@");
                        //s.Add(@"12341234");
                        //s_board_manager = new BoardManager(25, 80, s);
                        Program.BoardManager = new BoardManager(s,null,OptionsManager.Get<BoardMode>("I","RT_DEFAULT_MODE"));
                        Program.BoardManager.UpdateBoard();
                    break;
                case 2://Open File
                    Menus.OpenSimpleMenu openSimple = new OpenSimpleMenu();
                    openSimple.RunLoop();
                    break;
                case 3://Options
                    new OptionsMenu().RunLoop();
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
