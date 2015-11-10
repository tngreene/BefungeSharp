using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace BefungeSharp.Menus
{
    public class HelpMenu : Menu
    {
        public override void OnOpening()
        {
            Console.CursorTop = Console.WindowHeight - 2;
            Console.WriteLine("\t\t\t\tBefungeSharp Copyright 2015 by tedngreene. Enjoy!");
            Console.WriteLine("Press any key to go back");
            Console.CursorVisible = false;
        }

        public override void RunLoop()
        {
            OnOpening();
            Console.ReadKey(true);
            OnClosing();
        }

        public override void OnClosing()
        {
            return;
        }
    }
}
