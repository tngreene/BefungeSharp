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
            
            s.content = new List<string>();
            s.content.Add("");
            
            s.dimensions.Left = s.dimensions.Right = (short)origin.x;
            s.dimensions.Top = s.dimensions.Bottom = (short)origin.y;
            
            string input = Clipboard.GetText();

            if (input == "")
            {
                return s;
            }

            input = input.Replace("\r\n", "\n");
            if (input.EndsWith("\n"))
            {
                input = input.Remove(input.Length - 1);
            }
            
            

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                
                //If the charecter is a new line
                if (c == '\n')
                {
                    //We are garunteed no new line at the end of this
                    //So always add a new line
                    s.content.Add("");
                    //Increase the dimensions
                    s.dimensions.Bottom++;
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
                    
                    int width = s.dimensions.Right - s.dimensions.Left + 1;
                    //Make sure the bounds of the selection always capture
                    //The longest line
                    if (s.content[s.content.Count - 1].Length > width)
                    {
                        s.dimensions.Right++;
                    }
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
