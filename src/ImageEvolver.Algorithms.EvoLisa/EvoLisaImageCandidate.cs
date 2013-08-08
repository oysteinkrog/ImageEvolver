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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ImageEvolver.Algorithms.EvoLisa.Mutation;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Features;

namespace ImageEvolver.Algorithms.EvoLisa
{
    public sealed class EvoLisaImageCandidate : IImageCandidate
    {
        private EvoLisaImageCandidate(Size size)
        {
            Size = size;
            Polygons = new List<PolygonFeature>();
        }

        private EvoLisaImageCandidate(Size size, IEnumerable<PolygonFeature> polygons)
        {
            Size = size;
            Polygons = polygons.ToList();
        }

        public int PointCount
        {
            get { return Polygons.Sum(polygon => polygon.Points.Count); }
        }

        public List<PolygonFeature> Polygons { get; private set; }

        public IEnumerable<IFeature> Features
        {
            get { return Polygons; }
        }

        public Color BackgroundColor
        {
            get
            {
                return Color.Black;
            }
        }

        public Size Size { get; private set; }

        public EvoLisaImageCandidate Clone()
        {
            IEnumerable<PolygonFeature> clonedPolygons = Polygons.Select(polygon => polygon.Clone())
                                                                 .Cast<PolygonFeature>();
            return new EvoLisaImageCandidate(Size, clonedPolygons);
        }

        public static EvoLisaImageCandidate GetRandom(IRandomProvider randomProvider, EvoLisaAlgorithmSettings settings, Size size)
        {
            var candidate = new EvoLisaImageCandidate(size);
            for (int i = 0; i < settings.PolygonsRange.Min; i++)
            {
                AddPolygonMutation.AddPolygon(candidate, settings, randomProvider);
            }
            return candidate;
        }
    }
}