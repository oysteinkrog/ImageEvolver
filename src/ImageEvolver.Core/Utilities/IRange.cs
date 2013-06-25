using System;
using JetBrains.Annotations;

namespace ImageEvolver.Core.Utilities
{
    public interface IRange<out T> where T : IComparable<T>
    {
        /// <summary>
        ///     Max value of the range
        /// </summary>
        [PublicAPI]
        T Max
        {
            get;
        }

        /// <summary>
        ///     Min value of the range
        /// </summary>
        [PublicAPI]
        T Min
        {
            get;
        }
    }
}