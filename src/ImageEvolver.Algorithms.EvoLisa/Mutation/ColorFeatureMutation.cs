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

namespace ImageEvolver.Algorithms.EvoLisa.Mutation
{
    internal class ColorFeatureMutation : IFeatureMutation<ColorFeature, EvoLisaImageCandidate>
    {
        private readonly IRandomProvider _randomProvider;
        private readonly EvoLisaAlgorithmSettings _settings;

        public ColorFeatureMutation(EvoLisaAlgorithmSettings settings, IRandomProvider randomProvider)
        {
            _settings = settings;
            _randomProvider = randomProvider;
        }

        public bool MutateFeature(ColorFeature pointFeature, EvoLisaImageCandidate candidate)
        {
            bool mutated = false;

            if (_randomProvider.WillMutate(_settings.RedMutationRate.Value))
            {
                pointFeature.Red = _randomProvider.NextInt(_settings.RedRange.Min, _settings.RedRange.Max);
                mutated = true;
            }

            if (_randomProvider.WillMutate(_settings.GreenMutationRate.Value))
            {
                pointFeature.Green = _randomProvider.NextInt(_settings.GreenRange.Min, _settings.GreenRange.Max);
                mutated = true;
            }

            if (_randomProvider.WillMutate(_settings.BlueMutationRate.Value))
            {
                pointFeature.Blue = _randomProvider.NextInt(_settings.BlueRange.Min, _settings.BlueRange.Max);
                mutated = true;
            }

            if (_randomProvider.WillMutate(_settings.AlphaMutationRate.Value))
            {
                pointFeature.Alpha = _randomProvider.NextInt(_settings.AlphaRange.Min, _settings.AlphaRange.Max);
                mutated = true;
            }

            return mutated;
        }
    }
}