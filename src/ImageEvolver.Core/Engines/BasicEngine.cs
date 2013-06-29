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
using ImageEvolver.Core.Fitness;
using JetBrains.Annotations;

namespace ImageEvolver.Core.Engines
{
    /// <summary>
    ///     Basic evolutionary engine
    /// </summary>
    [PublicAPI]
    public sealed class BasicEngine<TCandidate, TEval> : IEvolutionEngine where TCandidate : IImageCandidate
    {
        private readonly ICandidateGenerator<TCandidate> _candidateGenerator;
        private readonly IFitnessEvaluator<TEval> _fitnessEvaluator;
        private readonly IImageCandidateRenderer<TCandidate, TEval> _renderer;
        private readonly object _syncRoot = new object();

        public BasicEngine(ICandidateGenerator<TCandidate> candidateGenerator,
                           IImageCandidateRenderer<TCandidate, TEval> renderer,
                           IFitnessEvaluator<TEval> fitnessEvaluator)
        {
            if (candidateGenerator == null)
            {
                throw new ArgumentNullException("candidateGenerator");
            }

            _candidateGenerator = candidateGenerator;
            _renderer = renderer;
            _fitnessEvaluator = fitnessEvaluator;

            CurrentBestFitness = double.MaxValue;
            Generation = 0;
            Selected = 0;

            // initial candidate
            CurrentBestCandidate = _candidateGenerator.GenerateCandidate();
        }

        public void Dispose() {}

        [PublicAPI]
        public long Candidates
        {
            get;
            set;
        }

        [PublicAPI]
        public TCandidate CurrentBestCandidate
        {
            get;
            private set;
        }

        [PublicAPI]
        public double CurrentBestFitness
        {
            get;
            private set;
        }

        [PublicAPI]
        public long Generation
        {
            get;
            private set;
        }

        [PublicAPI]
        public long Selected
        {
            get;
            private set;
        }

        [PublicAPI]
        public bool Step()
        {
            lock (_syncRoot)
            {
                // generate a new candidate from the single parent (no crossover/recombination)
                bool mutated;
                TCandidate newCandidate = _candidateGenerator.GenerateCandidate(CurrentBestCandidate, out mutated);

                Candidates++;
                if (mutated)
                {
                    Generation++;

                    var newRender = _renderer.Render(newCandidate);

                    // evaluate fitness of the new candidate
                    double newFitness = _fitnessEvaluator.EvaluateFitness(newRender);

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
        }
    }
}