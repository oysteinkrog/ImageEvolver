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
    }
}