using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BefungeSharp.UI
{
    public struct Selection
    {
        /// <summary>
        /// The origin of the selection
        /// </summary>
        public Vector2 origin;

        /// <summary>
        /// The moveable handle of the selection
        /// </summary>
        public Vector2 handle;

        /// <summary>
        /// The cached content of the selection
        /// </summary>
        public List<string> content;

        /// <summary>
        /// Create a FungeSpaceArea from the selection
        /// </summary>
        /// <returns>The generated FungeSpaceArea</returns>
        public FungeSpace.FungeSpaceArea GenerateArea()
        {
            //We need to figure out which quadrent our handle has ended up in
            //and where to choose our top, left, bottom, and right from
            //3 | 4  
            //__|___
            //2 | 1
            //  |

            //Q1
            if(handle.x >= origin.x && handle.y >= origin.y)
            {
                return new FungeSpace.FungeSpaceArea(origin.y,origin.x,handle.y,handle.x);
            }

            //Q2
            if(handle.x < origin.x && handle.y >= origin.y)
            {
                return new FungeSpace.FungeSpaceArea(origin.y,handle.x,handle.y,origin.x);
            }

            //Q3
            if(handle.x < origin.x && handle.y < origin.y)
            {
                return new FungeSpace.FungeSpaceArea(handle.y,handle.x,origin.y,origin.x);
            }

            //Q4
            if(handle.x >= origin.x && handle.y < origin.y)
            {
                return new FungeSpace.FungeSpaceArea(handle.y,origin.x,origin.y,handle.x);
            }
            return new FungeSpace.FungeSpaceArea(0, 0, 0, 0);
        }

        /// <summary>
        /// Resets the selection
        /// </summary>
        public void Clear()
        {
            origin = handle = Vector2.Zero;
            content = new List<string>();
        }
    }

    public static class ClipboardTools
    {
        public static Selection FromWindowsClipboard(Vector2 origin)
        {
            Selection s = new Selection();
            s.content = new List<string>();
            s.content.Add("");
            s.origin = s.handle = origin;
            
            string input;

            try
            {
                input = System.Windows.Clipboard.GetText();
            }
            catch
            {
                //If this fails for anyreason then leave
                return s;
            }

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
                    s.handle.y++;
                }
                else
                {
                    //TODO - This is only for Befunge-93
                    //If it is not a valid charecter
                    //if (c < '\0' || c > (char)255)
                    {
                        //Replace it with a space
                      //  c = ' ';
                    }

                    s.content[s.content.Count - 1] += c;
                    
                    int width = s.handle.x - s.origin.x + 1;
                    //Make sure the bounds of the selection always capture
                    //The longest line
                    if (s.content[s.content.Count - 1].Length > width)
                    {
                        s.handle.x++;
                    }
                }
            }
            
            return s;
        }

        public static void ToWindowsClipboard(Selection selection)
        {
            if (selection.content.Count > 0)
            {
                string output = "";
                for (int i = 0; i < selection.content.Count; i++)
                {
                    output += selection.content[i];
                    output += "\n";
                }
                try
                {
                    System.Windows.Clipboard.SetText(output);
                }
                catch
                {
                    return;
                }
            }
        }
    }
}
