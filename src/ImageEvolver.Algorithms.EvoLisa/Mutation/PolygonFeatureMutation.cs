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

using ImageEvolver.Algorithms.EvoLisa.Features;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Algorithms.EvoLisa.Utilities;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Features;

namespace ImageEvolver.Algorithms.EvoLisa.Mutation
{
    internal class PolygonFeatureMutation : IFeatureMutation<PolygonFeature, EvoLisaImageCandidate>
    {
        private readonly IRandomProvider _randomProvider;
        private readonly EvoLisaAlgorithmSettings _settings;

        public PolygonFeatureMutation(EvoLisaAlgorithmSettings settings, IRandomProvider randomProvider)
        {
            _settings = settings;
            _randomProvider = randomProvider;
        }

        public bool MutateFeature(PolygonFeature pointFeature, EvoLisaImageCandidate candidate)
        {
            bool mutated = false;
            if (_randomProvider.WillMutate(_settings.AddPointMutationRate.Value))
            {
                mutated |= AddPoint(pointFeature, candidate, _settings, _randomProvider);
            }

            if (_randomProvider.WillMutate(_settings.RemovePointMutationRate.Value))
            {
                mutated |= RemovePoint(pointFeature, candidate, _settings, _randomProvider);
            }

            return mutated;
        }

        private static bool AddPoint(PolygonFeature polygonFeature,
                                     EvoLisaImageCandidate evoLisaImageCandidate,
                                     EvoLisaAlgorithmSettings settings,
                                     IRandomProvider randomProvider)
        {
            if (polygonFeature.Points.Count < settings.PointsPerPolygonRange.Max)
            {
                if (evoLisaImageCandidate.PointCount < settings.PointsRange.Max)
                {
                    int index = randomProvider.NextInt(1, polygonFeature.Points.Count - 1);

                    PointFeature prev = polygonFeature.Points[index - 1];
                    PointFeature next = polygonFeature.Points[index];

                    int newPointX = (prev.X + next.X)/2;
                    int newPointY = (prev.Y + next.Y)/2;

                    var newPoint = new PointFeature(newPointX, newPointY);

                    polygonFeature.Points.Insert(index, newPoint);

                    return true;
                }
            }
            return false;
        }

        private static bool RemovePoint(PolygonFeature polygonFeature,
                                        EvoLisaImageCandidate evoLisaImageCandidate,
                                        EvoLisaAlgorithmSettings settings,
                                        IRandomProvider randomProvider)
        {
            if (polygonFeature.Points.Count > settings.PointsPerPolygonRange.Min)
            {
                if (evoLisaImageCandidate.PointCount > settings.PointsRange.Min)
                {
                    int index = randomProvider.NextInt(0, polygonFeature.Points.Count);
                    polygonFeature.Points.RemoveAt(index);
                    return true;
                }
            }
            return false;
        }
    }
}