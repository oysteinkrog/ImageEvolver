using ImageEvolver.Core.Fitness;

namespace ImageEvolver.Core
{
    public class CandidateFitnessEvaluator<T> : IFitnessEvaluator<IImageCandidate>
    {
        private readonly IImageCandidateRenderer<IImageCandidate, T> _renderer;
        private readonly IFitnessEvaluator<T> _bitmapFitnessEvalutor;

        public CandidateFitnessEvaluator(IImageCandidateRenderer<IImageCandidate, T> renderer, IFitnessEvaluator<T> bitmapFitnessEvalutor)
        {
            _renderer = renderer;
            _bitmapFitnessEvalutor = bitmapFitnessEvalutor;
        }

        public double EvaluateFitness(IImageCandidate candidate)
        {
            T output;
            _renderer.Render(candidate, out output);
            return _bitmapFitnessEvalutor.EvaluateFitness(output);
        }
    }
}