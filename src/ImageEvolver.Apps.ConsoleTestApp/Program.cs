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
using System.Drawing;
using System.Threading.Tasks;
using ImageEvolver.Algorithms.EvoLisa;
using ImageEvolver.Core.Engines;
using ImageEvolver.Resources.Images;
using Nito.AsyncEx;

namespace ImageEvolver.Apps.ConsoleTestApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            AsyncContext.Run(() => RunSimulation());
        }

        private static async void RunSimulation()
        {
            Bitmap sourceImage = Images.MonaLisa_EvoLisa200x200;

            using (var simpleEvolutionSystem = await SimpleEvolutionSystemOpenCL.Create(sourceImage))
            {
                while (!Console.KeyAvailable)
                {
                    if (await simpleEvolutionSystem.Engine.StepAsync())
                    {
                        BasicEngine<EvoLisaImageCandidate>.CandidateDetails bestCandidate = simpleEvolutionSystem.Engine.BestCandidate;
                        BasicEngine<EvoLisaImageCandidate>.PerformanceDetails perfDetails = simpleEvolutionSystem.Engine.GetPerformanceDetails();

                        Console.WriteLine(
                                          "Selected {0}, Generation {1}, BestFit {2:0.000}, Mutation {3:0.000}, Rendering  {4:0.000}, Fitness {5:0.000}",
                                          simpleEvolutionSystem.Engine.Selected,
                                          bestCandidate.Generation,
                                          bestCandidate.Fitness,
                                          perfDetails.RelativeMutationTime,
                                          perfDetails.RelativeFitnessEvaluationTime*perfDetails.FitnessEvaluationDetails.RelativeRenderingTime,
                                          perfDetails.RelativeFitnessEvaluationTime*
                                          perfDetails.FitnessEvaluationDetails.RelativeFitnessEvaluationTime);

                        // print every 100 better-fitness selection
                        if (simpleEvolutionSystem.Engine.Selected%100 == 0)
                        {
                            simpleEvolutionSystem.SaveBitmap(bestCandidate.Candidate,
                                                             string.Format("MonaLisa-test-{0}-{1}.jpg",
                                                                           simpleEvolutionSystem.Engine.Selected,
                                                                           bestCandidate.Generation));
                        }
                    }
                }
            }
        }
    }
}