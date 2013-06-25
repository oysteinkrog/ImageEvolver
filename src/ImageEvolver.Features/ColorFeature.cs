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

using ImageEvolver.Core.Mutation;

namespace ImageEvolver.Features
{
    public sealed class ColorFeature : IFeature
    {
        public ColorFeature(int red, int green, int blue, int alpha)
        {
            Red = red;
            Green = green;
            Blue = blue;
            Alpha = alpha;
        }

        public int Alpha { get; set; }
        public int Blue { get; set; }
        public int Green { get; set; }
        public int Red { get; set; }

        public IFeature Clone()
        {
            return new ColorFeature(Red, Green, Blue, Alpha);
        }
    }
}