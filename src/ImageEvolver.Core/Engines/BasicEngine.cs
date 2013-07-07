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
using ImageEvolver.Core.Fitness;
using JetBrains.Annotations;

namespace ImageEvolver.Core.Engines
{
    /// <summary>
    ///     Basic evolutionary engine
    /// </summary>
    [PublicAPI]
    public sealed class BasicEngine<TCandidate> : IEvolutionEngine where TCandidate : IImageCandidate
    {
        private readonly ICandidateGenerator<TCandidate> _candidateGenerator;
        private readonly Stopwatch _candidateStopwatch = new Stopwatch();
        private readonly IFitnessEvaluator<TCandidate> _fitnessEvaluator;
        private readonly Stopwatch _fitnessStopwatch = new Stopwatch();
        private readonly object _syncRoot = new object();
        private readonly Stopwatch _totalStopwatch = new Stopwatch();

        public BasicEngine(ICandidateGenerator<TCandidate> candidateGenerator, IFitnessEvaluator<TCandidate> fitnessEvaluator)
        {
            if (candidateGenerator == null)
            {
                throw new ArgumentNullException("candidateGenerator");
            }

            _candidateGenerator = candidateGenerator;
            _fitnessEvaluator = fitnessEvaluator;

            CurrentBestFitness = double.MaxValue;
            Generation = 0;
            Selected = 0;

            // initial candidate
            CurrentBestCandidate = _candidateGenerator.GenerateCandidate();
        }

        public void Dispose() {}

        [PublicAPI]
        public long Candidates { get; set; }

        [PublicAPI]
        public TCandidate CurrentBestCandidate { get; private set; }

        [PublicAPI]
        public double CurrentBestFitness { get; private set; }

        [PublicAPI]
        public long Generation { get; private set; }

        [PublicAPI]
        public long Selected { get; private set; }

        [PublicAPI]
        public PerformanceDetails GetPerformanceDetails()
        {
            var total = (double) _totalStopwatch.ElapsedMilliseconds;

            return new PerformanceDetails
                   {
                       RelativeMutationTime = _candidateStopwatch.ElapsedMilliseconds/total,
                       RelativeFitnessEvaluationTime = _fitnessStopwatch.ElapsedMilliseconds/total
                   };
        }

        [PublicAPI]
        public bool Step()
        {
            lock (_syncRoot)
            {
                _totalStopwatch.Start();

                try
                {
                    // generate a new candidate from the single parent (no crossover/recombination)
                    bool mutated;
                    _candidateStopwatch.Start();
                    TCandidate newCandidate = _candidateGenerator.GenerateCandidate(CurrentBestCandidate, out mutated);
                    _candidateStopwatch.Stop();

                    Candidates++;
                    if (mutated)
                    {
                        Generation++;

                        _fitnessStopwatch.Start();
                        // evaluate fitness of the new candidate
                        double newFitness = _fitnessEvaluator.EvaluateFitness(newCandidate);
                        _fitnessStopwatch.Stop();

                        if (newFitness <= CurrentBestFitness)
                        {
                            CurrentBestCandidate = newCandidate;
                            CurrentBestFitness = newFitness;
                            Selected++;
                            return true;
                        }
                    }

                    return false;
                }
                finally
                {
                    _totalStopwatch.Stop();
                }
            }
        }

        public struct PerformanceDetails
        {
            public double RelativeFitnessEvaluationTime;
            public double RelativeMutationTime;
        }
    }
}