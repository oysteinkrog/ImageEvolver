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
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Algorithms.EvoLisa.Utilities;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Core.Utilities;
using ImageEvolver.Features;

namespace ImageEvolver.Algorithms.EvoLisa.Mutation
{
    internal class AddPolygonMutation : IImageCandidateMutation<EvoLisaImageCandidate>
    {
        private readonly IRandomProvider _randomProvider;
        private readonly EvoLisaAlgorithmSettings _settings;

        public AddPolygonMutation(EvoLisaAlgorithmSettings settings, IRandomProvider randomProvider)
        {
            _settings = settings;
            _randomProvider = randomProvider;
        }

        public bool MutateCandidate(EvoLisaImageCandidate candidate)
        {
            bool mutated = false;

            if (_randomProvider.WillMutate(_settings.AddPolygonMutationRate.Value))
            {
                mutated |= AddPolygon(candidate, _settings, _randomProvider);
            }
            return mutated;
        }

        internal static bool AddPolygon(EvoLisaImageCandidate evoLisaImageCandidate, EvoLisaAlgorithmSettings settings, IRandomProvider randomProvider)
        {
            if (evoLisaImageCandidate.Polygons.Count < settings.PolygonsRange.Max)
            {
                PolygonFeature newPolygonFeature = GetRandomPolygonFeature(randomProvider,
                                                                           settings.PointsPerPolygonRange.Min,
                                                                           evoLisaImageCandidate.Size.Width,
                                                                           evoLisaImageCandidate.Size.Height);
                int index = randomProvider.NextInt(0, evoLisaImageCandidate.Polygons.Count);

                evoLisaImageCandidate.Polygons.Insert(index, newPolygonFeature);
                return true;
            }
            return false;
        }


        private static ColorFeature GetRandomColorFeature(IRandomProvider randomProvider)
        {
            return new ColorFeature(randomProvider.NextInt(0, 255),
                                    randomProvider.NextInt(0, 255),
                                    randomProvider.NextInt(0, 255),
                                    randomProvider.NextInt(10, 60));
        }

        private static PointFeature GetRandomPointFeature(IRandomProvider randomProvider, int maxX, int maxY)
        {
            return new PointFeature(randomProvider.NextInt(0, maxX), randomProvider.NextInt(0, maxY));
        }

        private static PolygonFeature GetRandomPolygonFeature(IRandomProvider randomProvider, int numPoints, int maxX, int maxY)
        {
            var points = new List<PointFeature>();

            PointFeature origin = GetRandomPointFeature(randomProvider, maxX, maxY);

            for (int i = 0; i < numPoints; i++)
            {
                int clampedX = MathUtils.Clamp(origin.X + randomProvider.NextInt(-3, 3), 0, maxX);
                int clampedY = MathUtils.Clamp(origin.Y + randomProvider.NextInt(-3, 3), 0, maxY);
                var clampedPoint = new PointFeature(clampedX, clampedY);

                points.Add(clampedPoint);
            }

            ColorFeature brush = GetRandomColorFeature(randomProvider);

            return new PolygonFeature(points, brush);
        }
    }
}