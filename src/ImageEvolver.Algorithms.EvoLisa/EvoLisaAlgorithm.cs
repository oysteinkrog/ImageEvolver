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

using System.Drawing;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core;
using ImageEvolver.Rendering.Bitmap;

namespace ImageEvolver.Algorithms.EvoLisa
{
    public class EvoLisaAlgorithm : IImageApproximationAlgorithm<EvoLisaImageCandidate>
    {
        private readonly IAlgorithmDetails _details = new EvoLisaAlgorithmDetails();
        private readonly IRandomProvider _randomProvider;
        private readonly IAlgorithmSettings _settings;
        private readonly Bitmap _sourceImage;

        public EvoLisaAlgorithm(Bitmap sourceImage, IAlgorithmSettings settings, IRandomProvider randomProvider)
        {
            _sourceImage = sourceImage;
            _settings = settings;
            _randomProvider = randomProvider;
        }

        public void Dispose() {}

        public string Name
        {
            get { return "EvoLisaAlgorithm (Reimplementation)"; }
        }

        public IAlgorithmDetails Details
        {
            get { return _details; }
        }

        public ICandidateGenerator<EvoLisaImageCandidate> CreateCandidateGenerator()
        {
            return new EvoLisaCandidateGenerator(_sourceImage, _randomProvider, (EvoLisaAlgorithmSettings) _settings);
        }

        public IImageCandidateRenderer<EvoLisaImageCandidate, Bitmap> CreateRenderer()
        {
            return new GenericFeaturesRendererBitmap(_sourceImage.Size);
        }
    }
}