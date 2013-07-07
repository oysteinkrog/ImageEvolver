using System.Drawing;
using ImageEvolver.Core;
using ImageEvolver.Core.Fitness;

namespace ImageEvolver.Fitness
{
    internal class FitnessEvaluatorCandidateBitmap : IFitnessEvaluator<IImageCandidate>
    {
        private readonly IImageCandidateRenderer<IImageCandidate, Bitmap> _renderer;
        private readonly FitnessEvaluatorBitmap _bitmapFitnessEvalutor;

        public FitnessEvaluatorCandidateBitmap(IImageCandidateRenderer<IImageCandidate, Bitmap> renderer, FitnessEvaluatorBitmap bitmapFitnessEvalutor)
        {
            _renderer = renderer;
            _bitmapFitnessEvalutor = bitmapFitnessEvalutor;
        }

        public double EvaluateFitness(IImageCandidate candidate)
        {
            var bitmap = _renderer.Render(candidate);
            return _bitmapFitnessEvalutor.EvaluateFitness(bitmap);
        }
    }
}