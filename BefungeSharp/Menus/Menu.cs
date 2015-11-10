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

        /// <summary>
        /// Waits for input from the user based on range of allowed values
        /// Good for situations like getting a number between x and y
        /// </summary>
        /// <param name="lower_bound">The lowest character accepted (by value)</param>
        /// <param name="upper_bound">The highest character accepted (by value)</param>
        /// <returns>The chosen input</returns>
        internal static char WaitForInput(char lower_bound, char upper_bound)
        {
            char input = '\0';
            //Get their input
            do
            {
                input = Console.ReadKey().KeyChar;
                Console.CursorLeft = 0;
            }
            while (input < lower_bound || input > upper_bound);
            return input;
        }
    }
}
