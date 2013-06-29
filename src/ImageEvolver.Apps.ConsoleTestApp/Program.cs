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
using ImageEvolver.Algorithms.EvoLisa;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core.Engines;
using ImageEvolver.Core.Random;
using ImageEvolver.Fitness;
using ImageEvolver.Rendering.Bitmap;
using ImageEvolver.Resources.Images;

namespace ImageEvolver.Apps.ConsoleTestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Bitmap sourceImage = Images.MonaLisa;
            var evoLisaAlgorithmSettings = new EvoLisaAlgorithmSettings();

            using (var basicPseudoRandomProvider = new BasicPseudoRandomProvider(0))
            {
                using (var evoLisaAlgorithm = new EvoLisaAlgorithm(sourceImage, evoLisaAlgorithmSettings, basicPseudoRandomProvider))
                {
                    using (var renderer = new GenericFeaturesRendererBitmap(sourceImage.Size))
                    {
                        using (var fitnessEvaluator = new FitnessEvaluatorBitmap(sourceImage, FitnessEquation.PSNR))
                        {
                            using (var candidateGenerator = evoLisaAlgorithm.CreateCandidateGenerator())
                            {
                                using (var evolutionEngine = new BasicEngine<EvoLisaImageCandidate, Bitmap>(candidateGenerator, renderer, fitnessEvaluator))
                                {
                                    while (!Console.KeyAvailable)
                                    {
                                        if (evolutionEngine.Step())
                                        {
                                            Console.WriteLine("{0} {1} {2}",
                                                              evolutionEngine.Selected,
                                                              evolutionEngine.Generation,
                                                              evolutionEngine.CurrentBestFitness);

                                            // print every 100 better-fitness selection
                                            if (evolutionEngine.Selected%100 == 0)
                                            {
                                                renderer.Render(evolutionEngine.CurrentBestCandidate);
                                                renderer.Value.Save(string.Format("MonaLisa-test-{0}-{1}.jpg",
                                                                                  evolutionEngine.Selected,
                                                                                  evolutionEngine.Generation));
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