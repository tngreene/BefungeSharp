using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Menus
{
    public abstract class Menu
    {
        /// <summary>
        /// Called when the loop starts
        /// </summary>
        public virtual void OnOpening()
        {
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = true;

            Console.Clear();
            Console.WriteLine();
        }

        /// <summary>
        /// Runs the loop of the menu
        /// </summary>
        public abstract void RunLoop();

        /// <summary>
        /// Called when the loop finishes
        /// </summary>
        public virtual void OnClosing()
        {
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
            Console.Clear();
            Console.CursorVisible = false;
        }

        internal static void DecorateWindowTop(string window_decoration)
        {
            Console.CursorLeft = (Console.WindowWidth / 2) - (window_decoration.Length / 2);
            Console.WriteLine(window_decoration);
            Console.WriteLine(new String('_', Console.WindowWidth) + "\r\n\r\n");
        }

    }
}
