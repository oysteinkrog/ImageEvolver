#region Copyright

//     ImageEvolver
//     Copyright (C) 2013-2013 Øystein Krog
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU Affero General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU Affero General Public License for more details.
// 
//     You should have received a copy of the GNU Affero General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using JetBrains.Annotations;

namespace ImageEvolver.Core.Utilities
{
    /// <summary>
    /// A basic Range for representing e.g. an integer range 
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/5343006/is-there-a-c-sharp-type-for-representing-an-integer-range
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [PublicAPI]
    public struct Range<T> : IRange<T> where T : IComparable<T>
    {
        public Range(T min, T max) : this()
        {
            Min = min;
            Max = max;
            IsValid();
        }

        /// <summary>
        ///     Max value of the range
        /// </summary>
        [PublicAPI]
        public T Max { get; private set; }

        /// <summary>
        ///     Min value of the range
        /// </summary>
        [PublicAPI]
        public T Min { get; private set; }

        /// <summary>
        ///     Determines if another range is inside the bounds of this range
        /// </summary>
        /// <param name="range">The child range to test</param>
        /// <returns>True if range is inside, else false</returns>
        [PublicAPI]
        public Boolean ContainsRange(Range<T> range)
        {
            return IsValid() && range.IsValid() && ContainsValue(range.Min) && ContainsValue(range.Max);
        }

        /// <summary>
        ///     Determines if the provided value is inside the range
        /// </summary>
        /// <param name="value">The value to test</param>
        /// <returns>True if the value is inside range, else false</returns>
        [PublicAPI]
        public Boolean ContainsValue(T value)
        {
            return (Min.CompareTo(value) <= 0) && (value.CompareTo(Max) <= 0);
        }

        /// <summary>
        ///     Determines if this range is inside the bounds of another range
        /// </summary>
        /// <param name="range">The parent range to test on</param>
        /// <returns>True if range is inclusive, else false</returns>
        [PublicAPI]
        public Boolean IsInsideRange(Range<T> range)
        {
            return IsValid() && range.IsValid() && range.ContainsValue(Min) && range.ContainsValue(Max);
        }

        /// <summary>
        ///     Determines if the range is valid
        /// </summary>
        /// <returns>True if range is valid, else false</returns>
        [PublicAPI]
        public Boolean IsValid()
        {
            return IsValid(Min, Max);
        }

        [PublicAPI]
        public static bool IsValid(T min, T max)
        {
            return min.CompareTo(max) <= 0;
        }

        /// <summary>
        ///     Presents the range in readable format
        /// </summary>
        /// <returns>String representation of the range</returns>
        public override string ToString()
        {
            return String.Format("[{0} - {1}]", Min, Max);
        }
    }
}