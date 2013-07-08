using System;
using System.Drawing;
using Clpp.Core.Utilities;
using ImageEvolver.Algorithms.EvoLisa;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core;
using ImageEvolver.Core.Engines;
using ImageEvolver.Core.Fitness;
using ImageEvolver.Core.Random;
using ImageEvolver.Fitness.Bitmap;
using ImageEvolver.Rendering.Bitmap;

namespace ImageEvolver.Apps.ConsoleTestApp
{
    public class SimpleEvolutionSystemBitmap : IDisposable
    {
        private BasicPseudoRandomProvider _basicPseudoRandomProvider;
        private CandidateFitnessEvaluator<Bitmap> _candidateEvaluator;
        private ICandidateGenerator<EvoLisaImageCandidate> _candidateGenerator;
        private EvoLisaAlgorithm _evoLisaAlgorithm;
        private EvoLisaAlgorithmSettings _evoLisaAlgorithmSettings;
        private BasicEngine<EvoLisaImageCandidate> _evolutionEngine;
        private FitnessEvaluatorBitmap _fitnessEvalutor;
        private IImageCandidateRenderer<IImageCandidate, Bitmap> _renderer;
        private Bitmap _renderBuffer;

        public SimpleEvolutionSystemBitmap(Bitmap sourceImage)
        {
            _renderer = new GenericFeaturesRendererBitmap(sourceImage.Size);
            //            _renderer = new GenericFeaturesRendererOpenGL(sourceImage.Size);
            _fitnessEvalutor = new FitnessEvaluatorBitmap(sourceImage, FitnessEquation.SimpleSE);

            _evoLisaAlgorithmSettings = new EvoLisaAlgorithmSettings();
            _basicPseudoRandomProvider = new BasicPseudoRandomProvider(0);
            _evoLisaAlgorithm = new EvoLisaAlgorithm(sourceImage, _evoLisaAlgorithmSettings, _basicPseudoRandomProvider);
            _candidateGenerator = _evoLisaAlgorithm.CreateCandidateGenerator();

            _renderBuffer = new Bitmap(sourceImage.Width, sourceImage.Height);
            _candidateEvaluator = new CandidateFitnessEvaluator<Bitmap>(_renderer, _fitnessEvalutor, _renderBuffer);
            _evolutionEngine = new BasicEngine<EvoLisaImageCandidate>(_candidateGenerator, _candidateEvaluator);
        }

        ~SimpleEvolutionSystemBitmap()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose managed resources
                DisposeHelper.Dispose(ref _evolutionEngine);
                DisposeHelper.Dispose(ref _candidateGenerator);
                DisposeHelper.Dispose(ref _evoLisaAlgorithm);
                DisposeHelper.Dispose(ref _basicPseudoRandomProvider);
                DisposeHelper.Dispose(ref _fitnessEvalutor);
                DisposeHelper.Dispose(ref _renderer);
            }
            // free native resources if there are any.
        }

        public BasicEngine<EvoLisaImageCandidate> Engine
        {
            get { return _evolutionEngine; }
        }

        public Bitmap RenderToBitmap(EvoLisaImageCandidate currentBestCandidate)
        {
            _renderer.Render(currentBestCandidate, _renderBuffer);
            return _renderBuffer;
        }

        public void SaveBitmap(EvoLisaImageCandidate candidate, string filePath)
        {
            RenderToBitmap(candidate)
                .Save(filePath);
        }
    }
}