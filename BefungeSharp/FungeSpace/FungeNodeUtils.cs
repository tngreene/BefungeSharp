using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.FungeSpace
{
	public static class FungeNodeUtils
	{
		/// <summary>
		/// Tests if a position is on or after the edge of a quadrent, and left in the void. WARNING: Uses GetOrCreateNode!
		/// </summary>
		/// <param name="matrix">The matrix to test</param>
		/// <param name="position">The position to test</param>
		/// <param name="search_direction">The direction to search if there is anything non-void ahead</param>
		/// <param name="space_is_void">True if you want to consider a squence of spaces as non-void</param>
		/// <returns>True if on or after non-void, flase if inside non-void space, including inner holes</returns>
		public static bool IsOnOrAfterEdge(FungeSparseMatrix matrix, Vector2 position, Vector2 search_direction, bool space_is_void=true)
		{
			//Cases, where x is non space and non void, ? is the one we're testing, and ... is the edge, the void
			//We always start in the middle
			//Space is void
			//['x'][' ']['x']...
			//['x']['x'][' ']...
			//[' ']['x']['x']...

			//If wraps around before start node changes you are in a gap
			//TODO: What if position is? What if it is in a gap?
			FungeNode start_node = null;
			FungeNode traverse = start_node = matrix.GetOrCreateNode(position);
			
			FungeNode last_known_good = null;

			//How to tell if space is strech in the middle or on edge
			//If discover space, set last known good to the one before it
			//if we wrap around before that changes again you're on a big section of space before the void
			//otherwise we're in a gap
			do
			{
				if (traverse.Value == ' ' && space_is_void == false)
				{
					last_known_good = traverse;
				}
				else if (traverse.Value != ' ')
				{
					last_known_good = traverse;
				}

				//Get the next node
				traverse = FungeNodeUtils.GetNodeAtOrWrap(traverse, search_direction);

				double last_known_mag = Math.Sqrt(Math.Pow(last_known_good.X, 2.0) + Math.Pow(last_known_good.Y, 2.0));
				double dot = (last_known_good.X * traverse.X) + (last_known_good.Y * traverse.Y);

				if (dot <= 0)
				{
					//We've wrapped around without finding something good!
					if (last_known_good == null)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else if (last_known_good != null) //We found non-void before wrapping around
				{
					return false;
				}
				else
				{
					continue;
				}
			}
			while (traverse != start_node);

			return false;
		}

		public static bool IsInGap(FungeSparseMatrix matrix, Vector2 position, Vector2 search_direction)
		{
			return true;
		}

		/// <summary>
		/// Move's an object's position node to a specific place, creating a node there if need be
		/// </summary>
		/// <param name="position">The position node of an object</param>
		/// <param name="row">The intented row to go to</param>
		/// <param name="column">The intended column to go to</param>
		/// <param name="value">The value to put if a node is created (default ' ')</param>
		/// <return>The new position of the FungeNode</return>
		public static FungeNode GetNodeAtOrCreate(FungeNode position, int row, int column, int value=' ')
		{
			return position.ParentMatrix.GetOrCreateNode(row, column, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="delta"></param>
		/// <returns></returns>
		public static FungeNode GetNodeAtOrCreate(FungeNode position, Vector2 delta, int value = ' ')
		{
			return GetNodeAtOrCreate(position, delta.y, delta.x, value);
		}

		/// <summary>
		/// MoveBy is an IP's move and wrap function
		/// </summary>
		/// <param name="position">The start position of the object</param>
		/// <param name="delta">The delta with which to move it</param>
		/// <return>The new position of the FungeNode</return>
		public static FungeNode GetNodeAtOrWrap(FungeNode position, Vector2 delta)
		{
			//If we are traveling one of the "easy" directions
			if (delta.x == 0 || delta.y == 0)
			{
				//Attempt to move |x| positions
				for (int x = Math.Abs(delta.x); x > 0; x--)
				{
					//If we are moving in the positive (East) direction
					if (delta.x > 0)
					{
						position = position.East;
					}
					else if (delta.x < 0)
					{
						position = position.West;
					}
				}

				//Attempt to move |y| positions
				for (int y = Math.Abs(delta.y); y > 0; y--)
				{
					//If we are moving in the positive (South) direction
					if (delta.y > 0)
					{
						position = position.South;
					}
					else if (delta.y < 0)
					{
						position = position.North;
					}
				}
			}
			else
			{
				//If we have a complicated delta where we are moving in two directions it means 
				//we have to attempt to move along it until we've reached the matrix bounds.
				//If we still haven't found another non-empty spot at that point we'll need to

				Vector2 origin = new Vector2(position.Data.x, position.Data.y);
				Vector2 traverse = origin;

				Vector2[] bounds = position.ParentMatrix.MatrixBounds;
				do
				{
					//Test if our next move will actually be out of bounds
					int nextX = traverse.x + (delta.x);
					int nextY = traverse.y + (delta.y);

					//If the next X is less than the least X OR greater than the most X
					if (nextX <= bounds[0].x)
					{
						nextX = bounds[1].x + (nextX);
					}
					else if (nextX >= bounds[1].x)
					{
						nextX = nextX - bounds[1].x;
					}

					//If the next Y is less than the least Y OR greater than the most Y
					if (nextY <= bounds[0].y)
					{
						nextY = bounds[1].y + (nextY);
					}
					else if (nextY >= bounds[1].y)
					{
						nextY = nextY - bounds[1].y;
					}

					//Attempt to get the next node at the next delta
					FungeNode lookup = position.ParentMatrix.GetNode(nextY, nextX);

					//If we finally found something
					if (lookup != null)
					{
						if (lookup.Data.value != ' ')
						{
							position = lookup;
							return position;
						}
					}

					//Insert and travel to it
					position = position.ParentMatrix.InsertCell(new FungeCell(nextX, nextY, ' '));
					traverse = position.Data;
				}
				while (traverse != origin);
			}
			return position;
		}
	}
}
