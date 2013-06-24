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
using ImageEvolver.Core.Utilities;

namespace ImageEvolver.Algorithms.EvoLisa.Mutation
{
    internal class PointFeatureMutation : IFeatureMutation<PointFeature, EvoLisaImageCandidate>
    {
        private readonly IRandomProvider _randomProvider;
        private readonly EvoLisaAlgorithmSettings _settings;

        public PointFeatureMutation(EvoLisaAlgorithmSettings settings, IRandomProvider randomProvider)
        {
            _settings = settings;
            _randomProvider = randomProvider;
        }

        public bool MutateFeature(PointFeature pointFeature, EvoLisaImageCandidate candidate)
        {
            bool mutated = false;
            if (_randomProvider.WillMutate(_settings.MovePointMutationRate.Max))
            {
                pointFeature.X = _randomProvider.NextInt(0, candidate.Size.Width);
                pointFeature.Y = _randomProvider.NextInt(0, candidate.Size.Height);
                mutated = true;
            }

            if (_randomProvider.WillMutate(_settings.MovePointMutationRate.Mid))
            {
                pointFeature.X = MathUtils.Clamp(pointFeature.X + _randomProvider.NextInt(-_settings.MovePointRange.Mid, _settings.MovePointRange.Mid),
                                                 0,
                                                 candidate.Size.Width);

                pointFeature.Y = MathUtils.Clamp(pointFeature.Y + _randomProvider.NextInt(-_settings.MovePointRange.Mid, _settings.MovePointRange.Mid),
                                                 0,
                                                 candidate.Size.Height);
                mutated = true;
            }

            if (_randomProvider.WillMutate(_settings.MovePointMutationRate.Min))
            {
                pointFeature.X = MathUtils.Clamp(pointFeature.X + _randomProvider.NextInt(-_settings.MovePointRange.Min, _settings.MovePointRange.Min),
                                                 0,
                                                 candidate.Size.Width);

                pointFeature.Y = MathUtils.Clamp(pointFeature.Y + _randomProvider.NextInt(-_settings.MovePointRange.Min, _settings.MovePointRange.Min),
                                                 0,
                                                 candidate.Size.Height);
                mutated = true;
            }

            return mutated;
        }
    }
}