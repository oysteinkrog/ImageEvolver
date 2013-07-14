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
using System.Diagnostics;
using System.Drawing;
using ImageEvolver.Core.Fitness;
using ImageEvolver.Fitness.Bitmap;
using ImageEvolver.Fitness.OpenCL;
using ImageEvolver.Rendering.OpenGL;
using ImageEvolver.Resources.Images;
using Koeky3D.BufferHandling;
using Koeky3D.Textures;
using NUnit.Framework;

namespace ImageEvolver.UnitTests.Fitness
{
    [TestFixture]
    public class FitnessPerformanceTests
    {
        [Test]
        public static void TestPerformanceBitmap([Values(FitnessEquation.AE, FitnessEquation.SimpleSE, FitnessEquation.MSE)] FitnessEquation fitnessEquation,
                                                 [Values(100)] int times,
                                                 [Values(1.0, 20)] double scaleFactor)
        {
            var sw = new Stopwatch();
            Bitmap imageA = Images.Resize(Images.MonaLisa_EvoLisa200x200, scaleFactor);
            Bitmap imageB = Images.Resize(Images.MonaLisa_EvoLisa200x200_TestApproximation, scaleFactor);
            using (var fitnessEvaluator = new FitnessEvaluatorBitmap(imageA, fitnessEquation))
            {
                // warmup
                for (int i = 0; i < 5; i++)
                {
                    double fitness = fitnessEvaluator.EvaluateFitness(imageB);
                }

                sw.Start();
                for (int i = 0; i < times; i++)
                {
                    double fitness = fitnessEvaluator.EvaluateFitness(imageB);
                }
                sw.Stop();
            }
            TimeSpan time = TimeSpan.FromTicks(sw.Elapsed.Ticks/times);
            Console.WriteLine("{0}x{1} {2:0.000}ms / fitness test", imageA.Width, imageA.Width, time.TotalMilliseconds);
        }

        [Test]
        public static void TestPerformanceOpenCL([Values(100)] int times, [Values(1.0, 20)] double scaleFactor)
        {
            var sw = new Stopwatch();
            Bitmap imageA = Images.Resize(Images.MonaLisa_EvoLisa200x200, scaleFactor);
            Bitmap imageB = Images.Resize(Images.MonaLisa_EvoLisa200x200_TestApproximation, scaleFactor);

            using (var openGlContext = new OpenGlContext())
            {
                FrameBuffer imageBFrameBuffer = null;
                openGlContext.TaskFactory.StartNew(() =>
                {
                    var imageBTexture = new Texture2D(imageB, false);
                    imageBFrameBuffer = new FrameBuffer(imageBTexture.Width, imageBTexture.Width, new Texture[] {imageBTexture}, null);
                })
                             .Wait();
                using (var fitnessEvaluator = new FitnessEvaluatorOpenCL(imageA, openGlContext))
                {
                    // warmup
                    for (int i = 0; i < 5; i++)
                    {
                        double fitness = fitnessEvaluator.EvaluateFitness(imageBFrameBuffer);
                    }

                    sw.Start();
                    for (int i = 0; i < times; i++)
                    {
                        double fitness = fitnessEvaluator.EvaluateFitness(imageBFrameBuffer);
                    }
                    sw.Stop();
                }
            }
            TimeSpan time = TimeSpan.FromTicks(sw.Elapsed.Ticks/times);
            Console.WriteLine("{0}x{1} {2:0.000}ms / fitness test", imageA.Width, imageA.Width, time.TotalMilliseconds);
        }
    }
}