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

using ImageEvolver.Core.Fitness;
using ImageEvolver.Fitness.Bitmap;
using ImageEvolver.Resources.Images;
using NUnit.Framework;

namespace ImageEvolver.UnitTests.Fitness
{
    [TestFixture]
    public class FitnessTests
    {
        [Test]
        public static void TestFitnessWithEvaluatorBitmap()
        {
            using (var fitnessEvaluator = new FitnessEvaluatorBitmap(Images.MonaLisa_EvoLisa200x200, FitnessEquation.SimpleSE))
            {
                double fitness = fitnessEvaluator.EvaluateFitness(Images.MonaLisa_EvoLisa200x200_TestApproximation);

                Assert.AreEqual(46865993.0, fitness);
            }
        }
    }
}