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
using ImageEvolver.Algorithms.EvoLisa;
using ImageEvolver.Algorithms.EvoLisa.Features;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core.Engines;
using ImageEvolver.Core.Random;
using ImageEvolver.Fitness;
using ImageEvolver.Resources.Images;
using NUnit.Framework;

namespace ImageEvolver.UnitTests.Algorithms.EvoLisa
{
    [TestFixture]
    public class CorrectnessTests
    {
        /// <summary>
        ///     Verify our implementation of the EvoLisa algorithm against the original EvoLisa code
        ///     Constants in the code were collected from the original using a random provider with a known seed (0)
        /// </summary>
        [Test]
        public void BasicFitnessCorrectTest()
        {
            Bitmap sourceImage = Images.MonaLisa;
            var evoLisaAlgorithmSettings = new EvoLisaAlgorithmSettings();

            using (var basicPseudoRandomProvider = new BasicPseudoRandomProvider(0))
            {
                using (var evoLisaAlgorithm = new EvoLisaAlgorithm(sourceImage, evoLisaAlgorithmSettings, basicPseudoRandomProvider))
                {
                    using (var renderer = evoLisaAlgorithm.CreateRenderer())
                    {
                        using (var fitnessEvaluator = new FitnessEvaluatorBitmap(sourceImage, FitnessEquation.SimpleSE))
                        {
                            using (var candidateGenerator = evoLisaAlgorithm.CreateCandidateGenerator())
                            {
                                using (var evolutionEngine = new BasicEngine<EvoLisaImageCandidate, Bitmap>(candidateGenerator, renderer, fitnessEvaluator))
                                {
                                    while (evolutionEngine.Selected < 10000)
                                    {
                                        if (evolutionEngine.Step())
                                        {
                                            switch (evolutionEngine.Selected)
                                            {
                                                case 800:
                                                {
                                                    Assert.AreEqual(334468501, evolutionEngine.CurrentBestFitness);
                                                    break;
                                                }
                                                case 1150:
                                                {
                                                    Assert.AreEqual(224646270, evolutionEngine.CurrentBestFitness);
                                                    break;
                                                }
                                                case 1432:
                                                {
                                                    Assert.AreEqual(191361415, evolutionEngine.CurrentBestFitness);
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}