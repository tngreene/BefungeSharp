using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BefungeSharp.UI
{
    public struct Selection
    {
        public ConEx.ConEx_Draw.SmallRect dimensions;
        public List<string> content;
        public bool active;
    }

    public static class ClipboardTools
    {
        /*public static Selection FromWindowsClipboard()
        {


            return new Selection();
            string input = Clipboard.GetText();

        }*/
        public static void ToWindowsClipboard(Selection selection)
        {
            string output = "";
            for (int i = 0; i < selection.content.Count; i++)
            {
                output += selection.content[i];
                output += "\n";
            }
            Clipboard.SetText(output);
        }
    }
}
