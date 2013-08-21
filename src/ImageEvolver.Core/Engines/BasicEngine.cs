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
        private readonly IProfilingFitnessEvaluator<TCandidate> _fitnessEvaluator;
        private readonly Stopwatch _fitnessStopwatch = new Stopwatch();
        private readonly object _syncRoot = new object();
        private readonly Stopwatch _totalStopwatch = new Stopwatch();
        private IRenderingFitnessEvalutionProfilingDetails _fitnessEvaluatorProfilingDetails;

        public BasicEngine(ICandidateGenerator<TCandidate> candidateGenerator, IProfilingFitnessEvaluator<TCandidate> fitnessEvaluator)
        {
            if (candidateGenerator == null)
            {
                throw new ArgumentNullException("candidateGenerator");
            }

            _candidateGenerator = candidateGenerator;
            _fitnessEvaluator = fitnessEvaluator;

            // initial candidate
            TCandidate startCandidate = _candidateGenerator.GenerateStartCandidate();

            BestCandidate = new CandidateDetails
                            {
                                Candidate = startCandidate,
                                Fitness = double.MaxValue,
                            };
        }

        public void Dispose() {}

        [PublicAPI]
        public CandidateDetails BestCandidate { get; private set; }

        [PublicAPI]
        public long Candidates { get; set; }


        [PublicAPI]
        public long Selected { get; set; }

        [PublicAPI]
        public TimeSpan TotalSimulationTime { get; private set; }

        [PublicAPI]
        public PerformanceDetails GetPerformanceDetails()
        {
            var total = (double) _totalStopwatch.ElapsedMilliseconds;

            return new PerformanceDetails
                   {
                       RelativeMutationTime = _candidateStopwatch.ElapsedMilliseconds/total,
                       RelativeFitnessEvaluationTime = _fitnessStopwatch.ElapsedMilliseconds/total,
                       FitnessEvaluationDetails = _fitnessEvaluatorProfilingDetails
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
                    CandidateDetails parentCandidateInfo = BestCandidate;

                    var newCandidateInfo = new CandidateDetails();

                    TCandidate newCandidate;
                    bool mutated = GenerateNewCandidateFromParent(parentCandidateInfo.Candidate, out newCandidate);
                    newCandidateInfo.Candidate = newCandidate;

                    Candidates++;

                    if (mutated)
                    {
                        newCandidateInfo.Generation = parentCandidateInfo.Generation + 1;

                        IProfilingFitnessEvaluationResult profilingFitnessEvaluationResult = EvaluateCandidateFitness(newCandidateInfo.Candidate);
                        _fitnessEvaluatorProfilingDetails = profilingFitnessEvaluationResult.ProfilingDetails;
                        newCandidateInfo.Fitness = profilingFitnessEvaluationResult.Fitness;

                        if (newCandidateInfo.Fitness <= BestCandidate.Fitness)
                        {
                            BestCandidate = newCandidateInfo;
                            Selected++;
                            return true;
                        }
                    }

                    return false;
                }
                finally
                {
                    _totalStopwatch.Stop();
                    TotalSimulationTime += _totalStopwatch.Elapsed;
                }
            }
        }

        private IProfilingFitnessEvaluationResult EvaluateCandidateFitness(TCandidate candidate)
        {
            _fitnessStopwatch.Start();
            try
            {
                // evaluate fitness of the new candidate
                return _fitnessEvaluator.EvaluateFitness(candidate);
            }
            finally
            {
                _fitnessStopwatch.Stop();
            }
        }

        private bool GenerateNewCandidateFromParent(TCandidate parentCandidate, out TCandidate newCandidate)
        {
            bool mutated;
            _candidateStopwatch.Start();
            newCandidate = _candidateGenerator.GenerateCandidate(parentCandidate, out mutated);
            _candidateStopwatch.Stop();
            return mutated;
        }

        [PublicAPI]
        public struct CandidateDetails
        {
            [PublicAPI]
            public TCandidate Candidate { get; set; }

            [PublicAPI]
            public double Fitness { get; set; }

            [PublicAPI]
            public long Generation { get; set; }
        }

        [PublicAPI]
        public struct PerformanceDetails
        {
            public double RelativeFitnessEvaluationTime;
            public double RelativeMutationTime;
            public IRenderingFitnessEvalutionProfilingDetails FitnessEvaluationDetails { get; set; }
        }
    }
}