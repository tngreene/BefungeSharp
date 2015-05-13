using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Menus
{
    interface IMenu
    {
        /// <summary>
        /// What is allowed to be inputed into then menu when prompted for input,
        /// a collection of RegEx strings
        /// </summary>
        string[] InputRestriction { get; }

        /// <summary>
        /// Runs the loop of the menu
        /// </summary>
        void RunLoop();

        /// <summary>
        /// Called when the loop starts
        /// </summary>
        void OnOpening();

        /// <summary>
        /// Called when the loop finishes
        /// </summary>
        void OnClosing();

    }
}
