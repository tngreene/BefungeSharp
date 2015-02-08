using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp
{
    public static class StackUtils
    {
        /// <summary>
        /// (Attempts to) Pop a Funge98 null terminated string off the stack
        /// </summary>
        /// <param name="stack">The input stack</param>
        /// <param name="isPath">If the intended string is a file path it will do clean up to ensure it is executable</param>
        /// <returns>Returns the string from the stack or an empty string if there was not enough characters on the stack</returns>
        public static string PopString(Stack<int> stack, bool isPath = false)
        {
            string outString = "";
            char c;

            while(stack.Count > 0)
            {
                c = (char)stack.Pop();
                
                if (c != '\0')
                {
                    outString += c;
                    if (isPath == true)
                    {
                        //If we just added a \ and its time to add a "
                        if (outString.Last() == '\\')
                        {
                            outString += '\"';
                        }

                        //If the next thing to add is a \, add a " before it
                        if (stack.Peek() == '\\' && outString.Last() != ':')
                        {
                            outString += '\"';
                        }
                    }
                }
                else
                {
                    if (isPath == true)
                    {
                        //Find the last pattern of \" and remove it
                        int filenameStart = outString.LastIndexOf(new string(new char[] { '\\', '\"' }));
                        outString = outString.Remove(filenameStart + 1, 1);
                    }
                    break;
                }
            }
                        
            return outString;
        }

        public static Vector2 PopVector(Stack<int> stack)
        {
            int stackOriginalCount = stack.Count;

            //A vector where vector[0] is Vb and vector[1] is Va
            int[] vector = new int[2];
            vector[0] = 0;
            vector[1] = 0;

            /* if stack.Count == 0 the while loop does not execute
             * and Va and Vb are equal to 0
             * if stack.Count == 1 the while loop executes once
             * and Va = 0, Vb = stack.Pop()
             * if stack.Count >= 2 the while loop executes atleast twice
             * and Va and Vb equals the top two contents of the stack.
             */
            int i = 0;
            while (stack.Count >= 1 && i < 2)
            {
                vector[i] = stack.Pop();
                i++;
            }
            return new Vector2(vector[1], vector[0]);
        }
    }
}
