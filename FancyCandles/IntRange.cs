using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyCandles
{
    /// <summary>Defines the range of consequent integer numbers.</summary>
    public struct IntRange
    {
        /// <summary>Defines the first integer number in the range.</summary>
        public int Start_i;

        /// <summary>Defines the length the range.</summary>
        public int Count;

        public IntRange(int start_i, int count)
        {
            Start_i = start_i;
            Count = count;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        public override bool Equals(Object obj)
        {
            return obj is IntRange && this == (IntRange)obj;
        }

        public override int GetHashCode()
        {
            return Start_i.GetHashCode() ^ Count.GetHashCode();
        }

        public static bool operator ==(IntRange c1, IntRange c2)
        {
            return (c1.Start_i == c2.Start_i && c1.Count == c2.Count);
        }

        public static bool operator !=(IntRange c1, IntRange c2)
        {
            return (c1.Start_i != c2.Start_i || c1.Count != c2.Count);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        public static IntRange Undefined { get { return new IntRange(int.MinValue, int.MinValue); } }

        public static bool IsUndefined(IntRange intRange)
        {
            return (intRange.Start_i == int.MinValue && intRange.Count == int.MinValue);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        public static IntRange CreateContainingOnlyStart_i(int start_i)
        {
            return new IntRange(start_i, -6666);
        }

        public static bool IsContainsOnlyStart_i(IntRange intRange)
        {
            return (intRange.Count == -6666);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        public static IntRange CreateContainingOnlyCount(int count)
        {
            return new IntRange(-6666, count);
        }

        public static bool IsContainsOnlyCount(IntRange intRange)
        {
            return (intRange.Start_i == -6666);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------
    }
}
