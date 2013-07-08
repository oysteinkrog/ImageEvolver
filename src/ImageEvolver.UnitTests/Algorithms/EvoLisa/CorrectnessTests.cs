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
using ImageEvolver.Core.Fitness;
using ImageEvolver.Core.Random;
using ImageEvolver.Fitness.Bitmap;
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
            using (var bitmapFitnessEvalutor = new FitnessEvaluatorBitmap(sourceImage, FitnessEquation.SimpleSE))
            {
                using (ICandidateGenerator<EvoLisaImageCandidate> candidateGenerator = evoLisaAlgorithm.CreateCandidateGenerator())
                {
                    var candidateFitnessEvaluator = new CandidateFitnessEvaluator<Bitmap>(renderer, bitmapFitnessEvalutor);

                    using (var evolutionEngine = new BasicEngine<EvoLisaImageCandidate>(candidateGenerator, candidateFitnessEvaluator))
                    {
                        RunEngine(evolutionEngine, renderer);
                    }
                }
            }
        }

        private static void RunEngine(BasicEngine<EvoLisaImageCandidate> evolutionEngine, IImageCandidateRenderer<IImageCandidate, Bitmap> renderer)
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

        private static bool CheckEngineResults(BasicEngine<EvoLisaImageCandidate> evolutionEngine, IImageCandidateRenderer<IImageCandidate, Bitmap> renderer)
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
                    var candidateInfo = evolutionEngine.BestCandidate;
                    Bitmap bitmap;
                    renderer.Render(candidateInfo.Candidate, out bitmap);
                    bitmap.Save(string.Format("select_{0}_{1}.bmp",
                                              evolutionEngine.Selected,
                                              renderer.GetType()
                                                      .Name));
                    Assert.AreEqual(1224598761, candidateInfo.Fitness);
                    break;
                }
                case 800:
                {
                    var candidateInfo = evolutionEngine.BestCandidate;
                    Bitmap bitmap;
                    renderer.Render(candidateInfo.Candidate, out bitmap);
                    bitmap.Save(string.Format("select_{0}_{1}.bmp",
                                              evolutionEngine.Selected,
                                              renderer.GetType()
                                                      .Name));
                    Assert.AreEqual(334468501, candidateInfo.Fitness);
                    break;
                }
                case 1150:
                {
                    var candidateInfo = evolutionEngine.BestCandidate;
                    Bitmap bitmap;
                    renderer.Render(candidateInfo.Candidate, out bitmap);
                    bitmap.Save(string.Format("select_{0}_{1}.bmp",
                                              evolutionEngine.Selected,
                                              renderer.GetType()
                                                      .Name));
                    Assert.AreEqual(224646270, candidateInfo.Fitness);
                    break;
                }
                case 1432:
                {
                    var candidateInfo = evolutionEngine.BestCandidate;
                    Bitmap bitmap;
                    renderer.Render(candidateInfo.Candidate, out bitmap);
                    bitmap.Save(string.Format("select_{0}_{1}.bmp",
                                              evolutionEngine.Selected,
                                              renderer.GetType()
                                                      .Name));
                    Assert.AreEqual(191361415, candidateInfo.Fitness);
                    return true;
                }
            }
            return false;
        }

        [Test]
        public void TestBitmapRenderer()
        {
            Bitmap sourceImage = Images.MonaLisa_EvoLisa200x200;
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
            Bitmap sourceImage = Images.MonaLisa_EvoLisa200x200;
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