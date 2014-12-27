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
        public static Selection FromWindowsClipboard(Vector2 origin)
        {
            Selection s = new Selection();
            s.active = false;
            s.dimensions.Left = s.dimensions.Right = (short)origin.x;
            s.dimensions.Top = s.dimensions.Bottom = (short)origin.y;
            
            string input = Clipboard.GetText();

            s.content = new List<string>();
            s.content.Add("");
            
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                
                //If the charecter is a new line
                if (c == '\n' || c == '\r')
                {
                    //Add another line if there is text after this
                    if (i + 1 < input.Length)
                    {
                        s.content.Add("");
                        //Increase the dimensions
                        s.dimensions.Bottom++;
                    }
                    
                    //If we are less than the 2nd from the last and the next charecter
                    //is \n, making this \r\n (aka Windows Line Ending)
                    if (i + 1 < input.Length && input[i + 1] == '\n')
                    {
                        //Go onto the rest
                        i++;
                    }
                }
                else
                {
                    //If it is not a valid charecter
                    if (c < '\0' || c > (char)255)
                    {
                        //Replace it with a space
                        c = ' ';
                    }

                    s.content[s.content.Count - 1] += c;
                    s.dimensions.Right++;
                }
                
            }
            
            return s;

        }
        public static void ToWindowsClipboard(Selection selection)
        {
            if (selection.active == true)
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
}
