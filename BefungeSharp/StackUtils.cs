using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp
{
    public static class StackUtils
    {
        public static string PopString(Stack<int> stack)
        {
            string outString = "";

            char c;
            while(stack.Count > 0)
            {
                c = (char)stack.Pop();
                if (c != '\0')
                {
                    outString += c;
                }
                else
                {
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
             * Elagent no?
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
