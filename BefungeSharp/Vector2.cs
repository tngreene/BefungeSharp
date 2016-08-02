using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp
{
    /* A simply 2D Vector struct which allows you to add, subtract, and negate with ease
     * */
    public struct Vector2
    {
        //A vector of 0,0
        public static readonly Vector2 Zero = new Vector2(0, 0);

        public static readonly Vector2 North = new Vector2(0, -1);
        //The default direction for Funge
        public static readonly Vector2 East = new Vector2(1, 0);
        public static readonly Vector2 South = new Vector2(0, 1);
        public static readonly Vector2 West = new Vector2(-1, 0);

        /// <summary>
        /// An array of the standard Cardinal Directions for easy accessings via intergers
        /// </summary>
        public static readonly Vector2[] CardinalDirections = { North, East, South, West };
        //X value of our vector
        public int x;
        //Y value of our vector
        public int y;

        public Vector2(Vector2 vec)
        {
            this.x = vec.x;
            this.y = vec.y;
        }

        //Takes in an x and y
        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        //Negates this vector
        public void Negate()
        {
            //Simply multiply x and y components by -1
            x = -x;
            y = -y;
        }

        //Quickly sets the vector to 0
        public void Clear()
        {
            x = 0;
            y = 0;
        }
        
        public override string ToString()
        {
            return x + "," + y;
        }
        
        //Add's this vector and another vector together
        //Returns the result
        public static Vector2 operator+(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x + rhs.x, lhs.y + rhs.y);
        }

        //Subtracts this vector and another vector together
        //Returns the result
        public static Vector2 operator-(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.x - rhs.x, lhs.y - rhs.y);
        }

        //Checks to see that every member is the same
        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            if (lhs.x == rhs.x)
            {
                if (lhs.y == rhs.y)
                {
                    return true;
                }
            }
            return false;
        }

        //Checks to see if any component is not alike
        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            if (lhs.x == rhs.x && lhs.y == rhs.y)
            {
                return false;
            }
            return true;
        }
        
    }

}