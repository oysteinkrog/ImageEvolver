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

using System;
using System.Drawing;
using ImageEvolver.Algorithms.EvoLisa.Mutation;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Features;

namespace ImageEvolver.Algorithms.EvoLisa
{
    internal sealed class EvoLisaCandidateGenerator : ICandidateGenerator<EvoLisaImageCandidate>
    {
        private readonly IImageCandidateMutation<EvoLisaImageCandidate>[] _imageCandidateMutations;
        private readonly IFeatureMutation<ColorFeature, EvoLisaImageCandidate>[] _colorFeatureMutations;
        private readonly IFeatureMutation<PointFeature, EvoLisaImageCandidate>[] _pointFeatureMutations;
        private readonly IFeatureMutation<PolygonFeature, EvoLisaImageCandidate>[] _polygonFeatureMutations;
        private readonly IRandomProvider _randomProvider;
        private readonly EvoLisaAlgorithmSettings _settings;
        private readonly Bitmap _sourceImage;
        private readonly object _syncRoot = new object();

        public EvoLisaCandidateGenerator(Bitmap sourceImage, IRandomProvider randomProvider, EvoLisaAlgorithmSettings settings)
        {
            _sourceImage = sourceImage;
            _randomProvider = randomProvider;
            _settings = settings;

            _imageCandidateMutations = new IImageCandidateMutation<EvoLisaImageCandidate>[]
                                       {
                                           new AddPolygonMutation(_settings, _randomProvider), new RemovePolygonMutation(_settings, _randomProvider),
                                           new MovePolygonMutation(_settings, _randomProvider)
                                       };

            _polygonFeatureMutations = new IFeatureMutation<PolygonFeature, EvoLisaImageCandidate>[]
                                       {
                                           new AddPointPolygonFeatureMutation(_settings, _randomProvider),
                                           new RemovePointPolygonFeatureMutation(_settings, _randomProvider)
                                       };

            _pointFeatureMutations = new IFeatureMutation<PointFeature, EvoLisaImageCandidate>[]
                                     {
                                         new MovePointMaxPointFeatureMutation(_settings, _randomProvider),
                                         new MovePointMidPointFeatureMutation(_settings, _randomProvider),
                                         new MovePointMinPointFeatureMutation(_settings, _randomProvider)
                                     };

            _colorFeatureMutations = new IFeatureMutation<ColorFeature, EvoLisaImageCandidate>[]
                                     {
                                         new RedColorFeatureMutation(_settings, _randomProvider), new GreenColorFeatureMutation(_settings, _randomProvider),
                                         new BlueColorFeatureMutation(_settings, _randomProvider), new AlphaColorFeatureMutation(_settings, _randomProvider)
                                     };
        }

        ~EvoLisaCandidateGenerator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }
            // free native resources if there are any.
        }

        /// <summary>
        ///     Generate a new candidate from a single parent (no crossover/recombination, just mutation)
        /// </summary>
        /// <param name="parentCandidate"></param>
        /// <returns></returns>
        public EvoLisaImageCandidate GenerateCandidate(EvoLisaImageCandidate parentCandidate, out bool mutated)
        {
            lock (_syncRoot)
            {
                mutated = false;

                if (parentCandidate == null)
                {
                    return GenerateStartCandidate();
                }

                EvoLisaImageCandidate newCandidate = parentCandidate.Clone();

                mutated = Mutate(newCandidate);

                return newCandidate;
            }
        }

        public EvoLisaImageCandidate GenerateStartCandidate()
        {
            var candidate = new EvoLisaImageCandidate(_sourceImage.Size);
            for (int i = 0; i < _settings.PolygonsRange.Min; i++)
            {
                AddPolygonMutation.AddPolygon(candidate, _settings, _randomProvider);
            }
            return candidate;
        }

        private bool Mutate(EvoLisaImageCandidate newCandidate)
        {
            bool mutated = false;

            foreach (var evoLisaImageCandidateMutation in _imageCandidateMutations)
            {
                mutated |= evoLisaImageCandidateMutation.MutateCandidate(newCandidate);
            }

            foreach (PolygonFeature polygonFeature in newCandidate.Polygons)
            {
                foreach (var polygonFeatureMutation in _polygonFeatureMutations)
                {
                    mutated |= polygonFeatureMutation.MutateFeature(polygonFeature, newCandidate);
                }

                foreach (var colorFeatureMutation in _colorFeatureMutations)
                {
                    mutated |= colorFeatureMutation.MutateFeature(polygonFeature.Color, newCandidate);
                }

                foreach (PointFeature pointFeature in polygonFeature.Points)
                {
                    foreach (var pointFeatureMutation in _pointFeatureMutations)
                    {
                        mutated |= pointFeatureMutation.MutateFeature(pointFeature, newCandidate);
                    }
                }
            }
            return mutated;
        }
    }
}