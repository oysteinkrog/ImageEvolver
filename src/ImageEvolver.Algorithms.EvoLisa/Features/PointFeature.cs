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

using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;

namespace ImageEvolver.Algorithms.EvoLisa.Features
{
    public class PointFeature : IFeature
    {
        public PointFeature(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; internal set; }
        public int Y { get; internal set; }

        public IFeature Clone()
        {
            return new PointFeature(X, Y);
        }

        public static PointFeature GetRandom(IRandomProvider randomProvider, int maxX, int maxY)
        {
            return new PointFeature(randomProvider.NextInt(0, maxX), randomProvider.NextInt(0, maxY));
        }
    }
}