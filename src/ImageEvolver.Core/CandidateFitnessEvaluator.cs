using ImageEvolver.Core.Fitness;

namespace ImageEvolver.Core
{
    public class CandidateFitnessEvaluator<T> : IFitnessEvaluator<IImageCandidate>
    {
        private readonly IImageCandidateRenderer<IImageCandidate, T> _renderer;
        private readonly IFitnessEvaluator<T> _bitmapFitnessEvalutor;
        private readonly T _renderBuffer;

        public CandidateFitnessEvaluator(IImageCandidateRenderer<IImageCandidate, T> renderer, IFitnessEvaluator<T> bitmapFitnessEvalutor, T renderBuffer)
        {
            _renderer = renderer;
            _bitmapFitnessEvalutor = bitmapFitnessEvalutor;
            _renderBuffer = renderBuffer;
        }

        public double EvaluateFitness(IImageCandidate candidate)
        {
            _renderer.Render(candidate, _renderBuffer);
            return _bitmapFitnessEvalutor.EvaluateFitness(_renderBuffer);
        }
    }
}