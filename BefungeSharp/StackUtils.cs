using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp
{
    public static class StackUtils
    {
        public static void StringPush(Stack<int> stack, string inString)
        {
            for (int i = inString.Length - 1; i >= 0 ; i--)
			{
                stack.Push(inString[i]);
			}
        }

        /// <summary>
        /// (Attempts to) Pop a Funge98 null terminated string off the stack
        /// </summary>
        /// <param name="stack">The input stack</param>
        /// <param name="isPath">If the intended string is a file path it will do clean up to ensure it is executable</param>
        /// <returns>Returns the string from the stack or an empty string if there was not enough characters on the stack</returns>
        public static string StringPop(Stack<int> stack, bool isPath = false)
        {
            string outString = "";
            char c;

            while(stack.Count > 0)
            {
                c = (char)stack.PopOrDefault();
                
                if (c != '\0')
                {
                    outString += c;
                    //TODO:Is this useful or good anymore?
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

        public static void VectorPush(Stack<int> stack, Vector2 v)
        {
            int b = v.y;
            int a = v.x;

            stack.Push(a);
            stack.Push(b);
        }

        public static Vector2 VectorPop(Stack<int> stack)
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
                vector[i] = stack.PopOrDefault();
                i++;
            }
            return new Vector2(vector[1], vector[0]);
        }

        public static void TransferCells(Stack<int> source_stack, Stack<int> destination_stack, int count, bool reverseOrder, bool removeFromSource)
        {
            if (count <= 0)
            {
                return;
            }
            
            Stack<int> workingSource;

            if (removeFromSource == true)
            {
                //workingSource is a reference to the original source
                workingSource = source_stack;
            }
            else
            {
                //workingSource is a reference to a new stack
                workingSource = new Stack<int>(source_stack.Reverse());
            }

            
            List<int> items = workingSource.GetRange(0,count);
            
            if (reverseOrder == true)
            {
                items.Reverse();
            }

            for (int i = items.Count - 1; i >= 0; i--)
            {
                destination_stack.Push(items[i]);
            }

            if (removeFromSource == true)
            {
                for (int i = 0; i < count; i++)
                {
                    workingSource.PopOrDefault();
                }
            }
        }

        public static List<T> GetRange<T>(this Stack<T> stack, int index, int count)
        {
            List<T> list = new List<T>();
            int i = index;
            while (i < count)
            {
                list.Add(stack.ElementAtOrDefault(i));
                i++;
            }
            return list;
        }

        /// <summary>
        /// Ensures that one can pop off the stack without ever causing an empty stack exception
        /// </summary>
        /// <param name="stack">The Stack object to pop off of</param>
        /// <returns>The popped element</returns>
        public static T PopOrDefault<T>(this Stack<T> stack)
        {
            if (stack.Count > 0)
            {
                return stack.Pop();
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Ensures that one can peek the top of the stack without ever causing an empty stack exception
        /// </summary>
        /// <param name="stack">The Stack object to peek at</param>
        /// <returns>The peeked element</returns>
        public static T PeekOrDefault<T>(this Stack<T> stack)
        {
            if (stack.Count > 0)
            {
                return stack.Peek();
            }
            else
            {
                return default(T);
            }
        }
        public static void EnsureStackSafety(Stack<int> stack, int required)
        {
            if (required > stack.Count)
            {
                int toAdd = required - stack.Count;
                int toStore = stack.Count;

                Stack<int> holder = new Stack<int>();
                for (int i = 0; i < toStore; i++)
                {
                    holder.Push(stack.Pop());
                }

                //Ensure that there will always be enough in the stack
                while (stack.Count < toAdd)
                {
                    //TODO - find out if we are at max stack capacity
                    //Insert behind the top of the stack
                    stack.Push(0);
                }

                for (int i = holder.Count; i > 0; i--)
                {
                    stack.Push(holder.Pop());
                }
            }
        }
    }
}
