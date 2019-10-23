/* 
    Copyright 2019 Dennis Geller.

    This file is part of FancyCandles.

    FancyCandles is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FancyCandles is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FancyCandles.  If not, see<https://www.gnu.org/licenses/>. */

using System;

namespace FancyCandles
{
    /// <summary>Defines the range of consequent integer numbers.</summary>
    public struct IntRange
    {
        /// <summary>Defines the first integer number in the range.</summary>
        public int Start_i;

        /// <summary>Defines the length the range.</summary>
        public int Count;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.IntRange(int, int)'
        public IntRange(int start_i, int count)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.IntRange(int, int)'
        {
            Start_i = start_i;
            Count = count;
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.Equals(object)'
        public override bool Equals(Object obj)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.Equals(object)'
        {
            return obj is IntRange && this == (IntRange)obj;
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.GetHashCode()'
        public override int GetHashCode()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.GetHashCode()'
        {
            return Start_i.GetHashCode() ^ Count.GetHashCode();
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.operator ==(IntRange, IntRange)'
        public static bool operator ==(IntRange c1, IntRange c2)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.operator ==(IntRange, IntRange)'
        {
            return (c1.Start_i == c2.Start_i && c1.Count == c2.Count);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.operator !=(IntRange, IntRange)'
        public static bool operator !=(IntRange c1, IntRange c2)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.operator !=(IntRange, IntRange)'
        {
            return (c1.Start_i != c2.Start_i || c1.Count != c2.Count);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.Undefined'
        public static IntRange Undefined { get { return new IntRange(int.MinValue, int.MinValue); } }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.Undefined'

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.IsUndefined(IntRange)'
        public static bool IsUndefined(IntRange intRange)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.IsUndefined(IntRange)'
        {
            return (intRange.Start_i == int.MinValue && intRange.Count == int.MinValue);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.CreateContainingOnlyStart_i(int)'
        public static IntRange CreateContainingOnlyStart_i(int start_i)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.CreateContainingOnlyStart_i(int)'
        {
            return new IntRange(start_i, -6666);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.IsContainsOnlyStart_i(IntRange)'
        public static bool IsContainsOnlyStart_i(IntRange intRange)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.IsContainsOnlyStart_i(IntRange)'
        {
            return (intRange.Count == -6666);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.CreateContainingOnlyCount(int)'
        public static IntRange CreateContainingOnlyCount(int count)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.CreateContainingOnlyCount(int)'
        {
            return new IntRange(-6666, count);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member 'IntRange.IsContainsOnlyCount(IntRange)'
        public static bool IsContainsOnlyCount(IntRange intRange)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member 'IntRange.IsContainsOnlyCount(IntRange)'
        {
            return (intRange.Start_i == -6666);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------
    }
}
