using System.Diagnostics;
using ImageEvolver.Core.Fitness;

namespace ImageEvolver.Core
{
    public class CandidateFitnessEvaluator<T> : IFitnessEvaluator<IImageCandidate>, IProfilingFitnessEvaluator<IImageCandidate>
    {
        private readonly IFitnessEvaluator<T> _bitmapFitnessEvalutor;
        private readonly Stopwatch _fitnessStopwatch;
        private readonly T _renderBuffer;
        private readonly Stopwatch _renderStopwatch;
        private readonly IImageCandidateRenderer<IImageCandidate, T> _renderer;
        private readonly Stopwatch _totalTimeStopwatch;

        public CandidateFitnessEvaluator(IImageCandidateRenderer<IImageCandidate, T> renderer, IFitnessEvaluator<T> bitmapFitnessEvalutor, T renderBuffer)
        {
            _renderer = renderer;
            _bitmapFitnessEvalutor = bitmapFitnessEvalutor;
            _renderBuffer = renderBuffer;

            _totalTimeStopwatch = new Stopwatch();
            _renderStopwatch = new Stopwatch();
            _fitnessStopwatch = new Stopwatch();
        }

        public double EvaluateFitness(IImageCandidate candidate)
        {
            _renderer.Render(candidate, _renderBuffer);
            return _bitmapFitnessEvalutor.EvaluateFitness(_renderBuffer);
        }

        IProfilingFitnessEvaluationResult IProfilingFitnessEvaluator<IImageCandidate>.EvaluateFitness(IImageCandidate candidate)
        {
            _totalTimeStopwatch.Start();

            _renderStopwatch.Start();
            _renderer.Render(candidate, _renderBuffer);
            _renderStopwatch.Stop();

            _fitnessStopwatch.Start();
            double fitness = _bitmapFitnessEvalutor.EvaluateFitness(_renderBuffer);
            _fitnessStopwatch.Stop();

            _totalTimeStopwatch.Stop();
            return new ProfilingCandidateProfilingFitnessEvaluationResult
                   {
                       Fitness = fitness,
                       ProfilingDetails = new ProfilingDetails
                                          {
                                              RelativeRenderingTime = _renderStopwatch.Elapsed.TotalSeconds/_totalTimeStopwatch.Elapsed.TotalSeconds,
                                              RelativeFitnessEvaluationTime = _fitnessStopwatch.Elapsed.TotalSeconds/_totalTimeStopwatch.Elapsed.TotalSeconds
                                          }
                   };
        }
    }

    internal class ProfilingDetails : IRenderingFitnessEvalutionProfilingDetails
    {
        public double RelativeRenderingTime { get; internal set; }
        public double RelativeFitnessEvaluationTime { get; internal set; }
    }

    internal struct ProfilingCandidateProfilingFitnessEvaluationResult : IProfilingFitnessEvaluationResult
    {
        public double Fitness { get; internal set; }
        public IRenderingFitnessEvalutionProfilingDetails ProfilingDetails { get; internal set; }
    }
}