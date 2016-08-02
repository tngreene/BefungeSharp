using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BefungeSharp.Instructions.Delta
{
    public abstract class DeltaInstruction : Instruction
    {
        /// <summary>
        /// A new delta which may be applied to the IP, avaible for use. Those not using it
        /// will use Vector2.Zero to represent null
        /// </summary>
        protected Vector2 newDelta;

        /// <summary>
        /// An instruction which changes the delta of an IP
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        public DeltaInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, CommandType.Movement, minimum_flags) { }
    }

    /// <summary>
    /// The cardinal instruction, sets the IP's delta to North, East, South, or West
    /// </summary>
    public class CardinalInstruction : DeltaInstruction
    {
        /// <summary>
        /// The cardinal instruction, sets the IP's delta to North, East, South, or West
        /// </summary>
        /// <param name="inName">The name of the instruction</param>
        /// <param name="minimum_flags">The required interpreter flags needed for this instruction to work</param>
        /// <param name="value">Must be North, East, South, or West or an exception will be throw</param>
        public CardinalInstruction(char inName, RuntimeFeatures minimum_flags, Vector2 delta) : base(inName, minimum_flags) 
        {
            if (delta != Vector2.North &&
                delta != Vector2.East &&
                delta != Vector2.South &&
                delta != Vector2.West)
            {
                throw new Exception("Delta " + delta.x + ", " + delta.y + " is not a cardinal direction!");
            }
            else
            {
                newDelta = delta;
            }
        }

        public override bool Preform(IP ip)
        {
            ip.Delta = newDelta;
            return true;
        }
    }

    /// <summary>
    /// The random delta instruction, sets the IP's delta to a random cardinal direction
    /// </summary>
    public class RandomDeltaInstruction : DeltaInstruction
    {
        public RandomDeltaInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            Random rnd = new Random();
            ip.Delta = newDelta = Vector2.CardinalDirections[rnd.Next(0, 4)];
            return true;
        }
    }

    /// <summary>
    /// The Set Delta (absolute delta) instruction, sets the IP's delta exactly to the specified delta
    /// </summary>
    public class SetDeltaInstruction : DeltaInstruction, IRequiresPop
    {
        public SetDeltaInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            StackUtils.EnsureStackSafety(ip.Stack, this.RequiredCells());
            
            //Set the new delta to the a vector popped off the stack
            newDelta = StackUtils.VectorPop(ip.Stack);
            
            //Set the Delta
            ip.Delta = newDelta;

            //Zero out this Instructions newDelta member to reset this instruction object
            newDelta = Vector2.Zero;
            return true;
        }

        public int RequiredCells()
        {
            return 2;
        }
    }

    /// <summary>
    /// The reverse delta instruction, sets the IP's delta to the oposite of what it is
    /// </summary>
    public class ReverseDeltaInstruction : DeltaInstruction
    {
        public ReverseDeltaInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }

        public override bool Preform(IP ip)
        {
            newDelta = ip.Delta;
            newDelta.Negate();
            ip.Delta = newDelta;

            newDelta = Vector2.Zero;
            return true;
        }
    }

    /// <summary>
    /// Rotates the IP's delta by -90 degrees
    /// </summary>
    public class TurnLeftInstruction : DeltaInstruction//, IPartnerSwappable
    {
        public TurnLeftInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }     
        
        public override bool Preform(IP ip)
        {
            ip.Delta = new Vector2(ip.Delta.y, ip.Delta.x * -1);
            return true;
        }
    }

    /// <summary>
    /// Rotates the IP's delta by 90 degrees
    /// </summary>
    public class TurnRightInstruction : DeltaInstruction//, IPartnerSwappable
    {
        public TurnRightInstruction(char inName, RuntimeFeatures minimum_flags) : base(inName, minimum_flags) { }      
        
        public override bool Preform(IP ip)
        {
            ip.Delta = new Vector2(ip.Delta.y * -1, ip.Delta.x);
            return true;
        }
    }
}
