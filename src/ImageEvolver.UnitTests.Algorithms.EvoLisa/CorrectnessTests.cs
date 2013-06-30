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
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core;
using ImageEvolver.Core.Engines;
using ImageEvolver.Core.Random;
using ImageEvolver.Fitness;
using ImageEvolver.Rendering.Bitmap;
using ImageEvolver.Rendering.OpenGL;
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
        private static void TestEvoLisaWithRenderer(IImageCandidateRenderer<IImageCandidate, Bitmap> renderer,
                                                      Bitmap sourceImage,
                                                      BasicPseudoRandomProvider basicPseudoRandomProvider)
        {
            var evoLisaAlgorithmSettings = new EvoLisaAlgorithmSettings();
            using (var evoLisaAlgorithm = new EvoLisaAlgorithm(sourceImage, evoLisaAlgorithmSettings, basicPseudoRandomProvider))
            {
                RunEngine(sourceImage, evoLisaAlgorithm, renderer);
            }
        }

        private static void RunEngine(Bitmap sourceImage, EvoLisaAlgorithm evoLisaAlgorithm, IImageCandidateRenderer<IImageCandidate, Bitmap> renderer)
        {
            using (var fitnessEvaluator = new FitnessEvaluatorBitmap(sourceImage, FitnessEquation.SimpleSE))
            {
                using (var candidateGenerator = evoLisaAlgorithm.CreateCandidateGenerator())
                {
                    using (var evolutionEngine = new BasicEngine<EvoLisaImageCandidate, Bitmap>(candidateGenerator, renderer, fitnessEvaluator))
                    {
                        RunEngine(evolutionEngine, renderer);
                    }
                }
            }
        }

        private static void RunEngine(BasicEngine<EvoLisaImageCandidate, Bitmap> evolutionEngine, IImageCandidateRenderer<IImageCandidate, Bitmap> renderer)
        {
            while (evolutionEngine.Selected < 10000)
            {
                if (evolutionEngine.Step())
                {
                    if (CheckEngineResults(evolutionEngine, renderer))
                    {
                        return;
                    }
                }
            }
        }

        private static bool CheckEngineResults(BasicEngine<EvoLisaImageCandidate, Bitmap> evolutionEngine,
                                               IImageCandidateRenderer<IImageCandidate, Bitmap> renderer)
        {
            switch (evolutionEngine.Selected)
            {
                    //                case 10:
                    //                {
                    //                    renderer.Render(evolutionEngine.CurrentBestCandidate)
                    //                            .Save(string.Format("select_{0}_{1}.bmp",
                    //                                                evolutionEngine.Selected,
                    //                                                renderer.GetType()
                    //                                                        .Name));
                    //                    Assert.AreEqual(1540687076, evolutionEngine.CurrentBestFitness);
                    //                    break;
                    //                }
                case 100:
                {
                    renderer.Render(evolutionEngine.CurrentBestCandidate)
                            .Save(string.Format("select_{0}_{1}.bmp",
                                                evolutionEngine.Selected,
                                                renderer.GetType()
                                                        .Name));
                    Assert.AreEqual(1224598761, evolutionEngine.CurrentBestFitness);
                    break;
                }
                case 800:
                {
                    renderer.Render(evolutionEngine.CurrentBestCandidate)
                            .Save(string.Format("select_{0}_{1}.bmp",
                                                evolutionEngine.Selected,
                                                renderer.GetType()
                                                        .Name));
                    Assert.AreEqual(334468501, evolutionEngine.CurrentBestFitness);
                    break;
                }
                case 1150:
                {
                    renderer.Render(evolutionEngine.CurrentBestCandidate)
                            .Save(string.Format("select_{0}_{1}.bmp",
                                                evolutionEngine.Selected,
                                                renderer.GetType()
                                                        .Name));
                    Assert.AreEqual(224646270, evolutionEngine.CurrentBestFitness);
                    break;
                }
                case 1432:
                {
                    renderer.Render(evolutionEngine.CurrentBestCandidate)
                            .Save(string.Format("select_{0}_{1}.bmp",
                                                evolutionEngine.Selected,
                                                renderer.GetType()
                                                        .Name));
                    Assert.AreEqual(191361415, evolutionEngine.CurrentBestFitness);
                    return true;
                }
            }
            return false;
        }

        [Test]
        public void TestBitmapRenderer()
        {
            var sourceImage = Images.MonaLisa;
            using (var renderer = new GenericFeaturesRendererBitmap(sourceImage.Size))
            {
                using (var basicPseudoRandomProvider = new BasicPseudoRandomProvider(0))
                {
                    TestEvoLisaWithRenderer(renderer, sourceImage, basicPseudoRandomProvider);
                }
            }
        }

        [Test]
        public void TestOpenGLRenderer()
        {
            var sourceImage = Images.MonaLisa;
            using (var renderer = new GenericFeaturesRendererOpenGL(sourceImage.Size))
            {
                using (var basicPseudoRandomProvider = new BasicPseudoRandomProvider(0))
                {
                    TestEvoLisaWithRenderer(renderer, sourceImage, basicPseudoRandomProvider);
                }
            }
        }
    }
}