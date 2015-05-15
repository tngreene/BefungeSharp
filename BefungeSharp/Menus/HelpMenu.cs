using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace BefungeSharp.Menus
{
    public class HelpMenu : IMenu
    {
        public string[] InputRestriction
        {
            get { return new string[] { "" }; }
        }

        public void OnOpening()
        {
            Console.Clear();
            Console.CursorTop = Console.WindowHeight - 2;
            Console.WriteLine("\t\t\t\tBefungeSharp Copyright 2015 by tedngreene. Enjoy!");
            Console.WriteLine("Press any key to go back");
        }

        public void OnClosing()
        {
            return;
        }

        public void RunLoop()
        {
            OnOpening();
            Console.ReadKey(true);
            OnClosing();
        }
    }
}
