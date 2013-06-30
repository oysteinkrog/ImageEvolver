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

using System.Drawing;
using ImageEvolver.Core.Utilities;
using JetBrains.Annotations;

namespace ImageEvolver.Core.Extensions
{
    public static class RandomProviderExtensions
    {
        [PublicAPI]
        public static double NextDouble(this IRandomProvider randomProvider, IRange<double> range)
        {
            return randomProvider.NextDouble(range.Min, range.Max);
        }

        [PublicAPI]
        public static int NextInt(this IRandomProvider randomProvider, IRange<int> range)
        {
            return randomProvider.NextInt(range.Min, range.Max);
        }

        [PublicAPI]
        public static Point NextPoint(this IRandomProvider randomProvider, IRange<int> rangeX, IRange<int> rangY)
        {
            return new Point(randomProvider.NextInt(rangeX), randomProvider.NextInt(rangY));
        }

        [PublicAPI]
        public static Point NextPoint(this IRandomProvider randomProvider, Size max)
        {
            return new Point(randomProvider.NextInt(0, max.Width), randomProvider.NextInt(0, max.Height));
        }

        [PublicAPI]
        public static Size NextSize(this IRandomProvider randomProvider, Point max)
        {
            return new Size(randomProvider.NextInt(0, max.X), randomProvider.NextInt(0, max.Y));
        }

        [PublicAPI]
        public static Size NextSize(this IRandomProvider randomProvider, Size max)
        {
            return new Size(randomProvider.NextInt(0, max.Width), randomProvider.NextInt(0, max.Height));
        }

        [PublicAPI]
        public static Size NextSize(this IRandomProvider randomProvider, Point min, Point max)
        {
            return new Size(randomProvider.NextInt(min.X, max.X), randomProvider.NextInt(min.Y, max.Y));
        }

    }
}