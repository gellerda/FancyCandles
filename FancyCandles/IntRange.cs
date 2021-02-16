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
    /// <summary>Represents the range of consequent integer numbers.</summary>
    public struct IntRange
    {
        /// <summary>The first integer number in the range.</summary>
        ///<value>The first integer number in the range.</value>
        public int Start_i;

        /// <summary>The length of the range.</summary>
        ///<value>The length of the range.</value>
        public int Count;

        /// <summary>Initializes a new instance of the IntRange structure that has the specified Start_i and Count.</summary>
        /// <param name="start_i">The Start_i of the IntRange</param>
        /// <param name="count">The Count of the IntRange</param>
        public IntRange(int start_i, int count)
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
        /// <summary>Returns a specific IntRange instance which is denoted as Undefined.</summary>
        ///<seealso cref = "IsUndefined">IsUndefined</seealso>
        public static IntRange Undefined { get { return new IntRange(int.MinValue, int.MinValue); } }

        ///<summary>Determines whether the specified parameter intRange is Undefined.</summary>
        ///<seealso cref = "Undefined">Undefined</seealso>
        public static bool IsUndefined(IntRange intRange)
        {
            return (intRange.Start_i == int.MinValue && intRange.Count == int.MinValue);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        /// <summary>Creates an IntRange instance with specified Start_i and undefined Count. We denote such instances as ContainingOnlyStart_i.</summary>
        ///<seealso cref = "IsContainsOnlyStart_i">IsContainsOnlyStart_i</seealso>
        public static IntRange CreateContainingOnlyStart_i(int start_i)
        {
            return new IntRange(start_i, int.MinValue);
        }

        ///<summary>Determines whether the specified parameter intRange is ContainingOnlyStart_i.</summary>
        ///<seealso cref = "CreateContainingOnlyStart_i">CreateContainingOnlyStart_i</seealso>
        public static bool IsContainsOnlyStart_i(IntRange intRange)
        {
            return (intRange.Count == int.MinValue && intRange.Start_i != int.MinValue);
        }
        //------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------
    }
}
