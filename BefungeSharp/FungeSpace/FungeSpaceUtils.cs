using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.FungeSpace
{
    /// <summary>
    /// FungeSpace is a utility class for finding out information about and changing the SparseMatrix
    /// </summary>
    public static class FungeSpaceUtils
    {
        public static void TestMatrix()
        {
            FungeNode traverse = null;
            if (false)
            {
                //Many columns, 1 row
                FungeSparseMatrix matrix1 = new FungeSparseMatrix(4, 4);
                matrix1.PrintFungeSpace();

                traverse = matrix1.Origin;
                while (traverse != null)
                {
                    char delta = Console.ReadKey(true).KeyChar;

                    switch (delta)
                    {
                        case 'i':
                            traverse = traverse.North;
                            break;
                        case 'l':
                            traverse = traverse.East;
                            break;
                        case 'k':
                            traverse = traverse.South;
                            break;
                        case 'j':
                            traverse = traverse.West;
                            break;
                        case 'q':
                            traverse = null;
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine(traverse);
                }
            }
            FungeSparseMatrix matrix2 = new FungeSparseMatrix();
            ////Many rows, 1 column
            //FungeSparseMatrix matrix3 = new FungeSparseMatrix(125000,100);
            //FungeSparseMatrix matrix4 = new FungeSparseMatrix(500000,100);
            //FungeSparseMatrix matrix = new FungeSparseMatrix();
            Console.WriteLine("----------------------------------------");

            char v = 'a';
            while (true)
            {
                int x = 0;
                int y = 0;
                char value = v;
                try
                {
                    x = Convert.ToInt32(Console.ReadLine());
                    y = Convert.ToInt32(Console.ReadLine());
                    //value = Convert.ToChar(Console.ReadKey(true).KeyChar);
                }
                catch (Exception e)
                {
                    break;
                }

                FungeCell _cell = new FungeCell(x, y, value);
                matrix2.InsertCell(_cell);
                matrix2.PrintFungeSpace();
                v++;
            }

            traverse = matrix2.Origin;
            while (traverse != null)
            {
                char delta = Console.ReadKey(true).KeyChar;

                switch (delta)
                {
                    case 'i':
                        traverse = traverse.North;
                        break;
                    case 'l':
                        traverse = traverse.East;
                        break;
                    case 'k':
                        traverse = traverse.South;
                        break;
                    case 'j':
                        traverse = traverse.West;
                        break;
                    case 'q':
                        traverse = null;
                        break;
                    default:
                        break;
                }
                Console.WriteLine(traverse);
            }
        }
        
    }
}
