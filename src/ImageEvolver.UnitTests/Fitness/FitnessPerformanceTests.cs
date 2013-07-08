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

using System;
using System.Diagnostics;
using System.Drawing;
using ImageEvolver.Core.Fitness;
using ImageEvolver.Fitness.Bitmap;
using ImageEvolver.Fitness.OpenCL;
using ImageEvolver.Rendering.OpenGL;
using ImageEvolver.Resources.Images;
using Koeky3D.Textures;
using NUnit.Framework;

namespace ImageEvolver.UnitTests.Fitness
{
    [TestFixture]
    public class FitnessPerformanceTests
    {
        [Test]
        public static void TestPerformanceBitmap([Values(FitnessEquation.AE, FitnessEquation.SimpleSE, FitnessEquation.MSE)] FitnessEquation fitnessEquation, [Values(1000)] int times)
        {
            var sw = new Stopwatch();
            Bitmap imageA = Images.Resize(Images.MonaLisa_EvoLisa200x200, 2.0);
            Bitmap imageB = Images.Resize(Images.MonaLisa_EvoLisa200x200_TestApproximation, 2.0);
            using (var fitnessEvaluator = new FitnessEvaluatorBitmap(imageA, fitnessEquation))
            {
                // warmup
                for (int i = 0; i < 100; i++)
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
            Console.WriteLine("{0:0.000}ms / fitness test", time.TotalMilliseconds);
        }

        [Test]
        public static void TestPerformanceOpenCL([Values(1000)] int times)
        {
            var sw = new Stopwatch();
            Bitmap imageA = Images.Resize(Images.MonaLisa_EvoLisa200x200, 2.0);
            Bitmap imageB = Images.Resize(Images.MonaLisa_EvoLisa200x200_TestApproximation, 2.0);

            using (var openGlContext = new RenderingContextBase())
            {
                Texture2D imageBTexture = null;
                openGlContext.GLTaskFactory.StartNew(() =>
                {
                    imageBTexture = new Texture2D(imageB, false);
                })
                             .Wait();
                using (var fitnessEvaluator = new FitnessEvaluatorOpenCL(openGlContext.GLTaskFactory, imageA, openGlContext.GraphicsContext))
                {                
                    // warmup
                    for (int i = 0; i < 100; i++)
                    {
                        double fitness = fitnessEvaluator.EvaluateFitness(imageBTexture);
                    }

                    sw.Start();
                    for (int i = 0; i < times; i++)
                    {
                        double fitness = fitnessEvaluator.EvaluateFitness(imageBTexture);
                    }
                    sw.Stop();
                }
            }
            TimeSpan time = TimeSpan.FromTicks(sw.Elapsed.Ticks / times);
            Console.WriteLine("{0:0.000}ms / fitness test", time.TotalMilliseconds);
        }
    }
}