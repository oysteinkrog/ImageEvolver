﻿#region Copyright

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

using System.Diagnostics;
using ImageEvolver.Core.Utilities;
using JetBrains.Annotations;

namespace ImageEvolver.Core.Random
{
    [PublicAPI]
    public sealed class CryptoPseudoRandomProvider : IRandomProvider
    {
        private readonly CryptoRandom _random;

        [PublicAPI]
        public CryptoPseudoRandomProvider()
        {
            _random = new CryptoRandom();
        }

        public void Dispose() {}

        [PublicAPI]
        public double NextDouble(double minValue, double maxValue)
        {
            Debug.Assert(minValue <= maxValue);
            return _random.NextDouble()*(maxValue - minValue) + minValue;
        }

        [PublicAPI]
        public int NextInt(int minValue, int maxValue)
        {
            Debug.Assert(minValue <= maxValue);
            return _random.Next(minValue, maxValue);
        }
    }
}