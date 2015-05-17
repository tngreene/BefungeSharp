using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace BefungeSharp.Menus
{
    class MainMenu : IMenu
    {
        public string[] InputRestriction
        {
            get { return new[] { "[123456]" }; }
        }
        
        public void OnOpening()
        {
            Console.Clear();

            //A silly bit of fun to make the menu look cooler
            Random rnd = new Random();
            Console.ForegroundColor = (ConsoleColor)rnd.Next(7, 16);

            //Ask what they would like to do
            Console.WriteLine("Welcome to BefungeSharp! Please select an option:\n");
            Console.WriteLine("1.) New File");
            Console.WriteLine("2.) Open File");
            Console.WriteLine("3.) Advanced Open File");
            Console.WriteLine("4.) Options");
            Console.WriteLine("5.) Help");
            Console.WriteLine("6.) Exit");
        }

        public void OnClosing()
        {
            //Prevent the main menu from closing unless one explictly 
            //inputs the exit command
            RunLoop();
        }

        public void RunLoop()
        {
            OnOpening();
            string input = "";
            //Get their input
            while (true)
            {
                input = Console.ReadKey().KeyChar.ToString();
                Console.WriteLine();
                bool valid = Regex.IsMatch(input, this.InputRestriction[0]);
                if (valid == false)
                {
                    string numbers = this.InputRestriction[0].Substring(1,this.InputRestriction[0].Length-2);
                    Console.Write("Please enter a number " + numbers.First() + '-' + numbers.Last() + '\n');
                }
                else
                {
                    break;
                }
            }
            Console.Clear();
            switch (input[0] - 48)
            {
                case 1://New File
                        List<string> s = null;// new List<string>();
                        //s.Add(@"""dlroW olleH"">:#,_@");
                        //s.Add(@"12341234");
                        //s_board_manager = new BoardManager(25, 80, s);
                        Program.BoardManager = new BoardManager(0, 0, s);
                        Program.BoardManager.UpdateBoard();
                    break;
                case 2://Open File
                    Menus.OpenSimpleMenu openSimple = new OpenSimpleMenu();
                    openSimple.RunLoop();
                    break;
                case 3://Advanced Open File
                    
                    break;
                case 4://Options
                    break;
                case 5://Help
                    Menus.HelpMenu helpMenu = new HelpMenu();
                    helpMenu.RunLoop();
                    break;
                case 6://Exist
                    return;
            }
            OnClosing();
        }
    }
}
