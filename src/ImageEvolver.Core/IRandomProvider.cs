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
using ImageEvolver.Core.Utilities;
using JetBrains.Annotations;

namespace ImageEvolver.Core
{
    public interface IRandomProvider : IDisposable
    {
        [PublicAPI]
        double NextDouble(double minValue, double maxValue);

        [PublicAPI]
        double NextDouble(Range<double> range);

        [PublicAPI]
        int NextInt(int minValue, int maxValue);

        [PublicAPI]
        int NextInt(Range<int> range);
    }
}