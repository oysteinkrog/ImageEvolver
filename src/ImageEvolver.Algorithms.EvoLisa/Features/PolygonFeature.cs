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
using System.Linq;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Core.Utilities;

namespace ImageEvolver.Algorithms.EvoLisa.Features
{
    public sealed class PolygonFeature : IFeatureWithSubFeatures
    {
        private PolygonFeature(IEnumerable<PointFeature> points, ColorFeature color)
        {
            Points = points.ToList();
            Color = color;
        }

        internal ColorFeature Color { get; private set; }
        internal List<PointFeature> Points { get; private set; }

        public IFeature Clone()
        {
            var clonedPoints = Points.Select(point => point.Clone())
                                     .Cast<PointFeature>();
            var clonedColor = Color.Clone();
            return new PolygonFeature(clonedPoints, (ColorFeature) clonedColor);
        }

        public IEnumerable<IFeature> Features
        {
            get
            {
                yield return Color;
                foreach (var pointFeature in Points)
                {
                    yield return pointFeature;
                }
            }
        }

        public static PolygonFeature GetRandom(IRandomProvider randomProvider, EvoLisaAlgorithmSettings settings, int maxX, int maxY)
        {
            var points = new List<PointFeature>();

            //int count = randomProvider.NextInt(3, 3);
            PointFeature origin = PointFeature.GetRandom(randomProvider, maxX, maxY);

            for (int i = 0; i < settings.PointsPerPolygonRange.Min; i++)
            {
                int clampedX = MathUtils.Clamp(origin.X + randomProvider.NextInt(-3, 3), 0, maxX);
                int clampedY = MathUtils.Clamp(origin.Y + randomProvider.NextInt(-3, 3), 0, maxY);
                var clampedPoint = new PointFeature(clampedX, clampedY);

                points.Add(clampedPoint);
            }

            ColorFeature brush = ColorFeature.GetRandom(randomProvider);

            return new PolygonFeature(points, brush);
        }
    }
}