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

using ImageEvolver.Algorithms.EvoLisa.Features;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Algorithms.EvoLisa.Utilities;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;

namespace ImageEvolver.Algorithms.EvoLisa.Mutation
{
    internal class EvoLisaImageCandidateMutation : IImageCandidateMutation<EvoLisaImageCandidate>
    {
        private readonly IRandomProvider _randomProvider;
        private readonly EvoLisaAlgorithmSettings _settings;

        public EvoLisaImageCandidateMutation(EvoLisaAlgorithmSettings settings, IRandomProvider randomProvider)
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

            if (_randomProvider.WillMutate(_settings.RemovePolygonMutationRate.Value))
            {
                mutated |= RemovePolygon(candidate, _settings, _randomProvider);
            }

            if (_randomProvider.WillMutate(_settings.MovePolygonMutationRate.Value))
            {
                mutated |= MovePolygon(candidate, _randomProvider);
            }

            return mutated;
        }

        internal static bool AddPolygon(EvoLisaImageCandidate evoLisaImageCandidate, EvoLisaAlgorithmSettings settings, IRandomProvider randomProvider)
        {
            if (evoLisaImageCandidate.Polygons.Count < settings.PolygonsRange.Max)
            {
                PolygonFeature newPolygonFeature = PolygonFeature.GetRandom(randomProvider,
                                                                            settings,
                                                                            evoLisaImageCandidate.Size.Width,
                                                                            evoLisaImageCandidate.Size.Height);
                int index = randomProvider.NextInt(0, evoLisaImageCandidate.Polygons.Count);

                evoLisaImageCandidate.Polygons.Insert(index, newPolygonFeature);
                return true;
            }
            return false;
        }

        internal static bool MovePolygon(EvoLisaImageCandidate evoLisaImageCandidate, IRandomProvider randomProvider)
        {
            if (evoLisaImageCandidate.Polygons.Count < 1)
            {
                return false;
            }

            int index = randomProvider.NextInt(0, evoLisaImageCandidate.Polygons.Count);
            PolygonFeature poly = evoLisaImageCandidate.Polygons[index];
            evoLisaImageCandidate.Polygons.RemoveAt(index);
            index = randomProvider.NextInt(0, evoLisaImageCandidate.Polygons.Count);
            evoLisaImageCandidate.Polygons.Insert(index, poly);
            return true;
        }

        internal static bool RemovePolygon(EvoLisaImageCandidate evoLisaImageCandidate, EvoLisaAlgorithmSettings settings, IRandomProvider randomProvider)
        {
            if (evoLisaImageCandidate.Polygons.Count > settings.PolygonsRange.Min)
            {
                int index = randomProvider.NextInt(0, evoLisaImageCandidate.Polygons.Count);
                evoLisaImageCandidate.Polygons.RemoveAt(index);
                return true;
            }
            return false;
        }
    }
}