using System;
using System.Drawing;
using Clpp.Core.Utilities;
using ImageEvolver.Algorithms.EvoLisa;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core;
using ImageEvolver.Core.Engines;
using ImageEvolver.Core.Random;
using ImageEvolver.Fitness.OpenCL;
using ImageEvolver.Rendering.OpenGL;
using Koeky3D.Textures;

namespace ImageEvolver.Apps.ConsoleTestApp
{
    public class SimpleEvolutionSystemOpenCL : IDisposable
    {
        private BasicPseudoRandomProvider _basicPseudoRandomProvider;
        private ICandidateGenerator<EvoLisaImageCandidate> _candidateGenerator;
        private EvoLisaAlgorithm _evoLisaAlgorithm;
        private EvoLisaAlgorithmSettings _evoLisaAlgorithmSettings;
        private BasicEngine<EvoLisaImageCandidate> _evolutionEngine;
        private FitnessEvaluatorOpenCL _fitnessEvalutor;
        private GenericFeaturesRendererOpenGL _renderer;
        private CandidateFitnessEvaluator<Texture2D> _candidateEvaluator;

        public SimpleEvolutionSystemOpenCL(Bitmap sourceImage)
        {
            //_renderer = new GenericFeaturesRendererBitmap(sourceImage.Size);
            var genericFeaturesRendererOpenGL = new GenericFeaturesRendererOpenGL(sourceImage.Size);
            _renderer = genericFeaturesRendererOpenGL;
            _fitnessEvalutor = new FitnessEvaluatorOpenCL(genericFeaturesRendererOpenGL.GLTaskFactory, sourceImage, genericFeaturesRendererOpenGL.GraphicsContext);

            _evoLisaAlgorithmSettings = new EvoLisaAlgorithmSettings();
            _basicPseudoRandomProvider = new BasicPseudoRandomProvider(0);
            _evoLisaAlgorithm = new EvoLisaAlgorithm(sourceImage, _evoLisaAlgorithmSettings, _basicPseudoRandomProvider);
            _candidateGenerator = _evoLisaAlgorithm.CreateCandidateGenerator();
            _candidateEvaluator = new CandidateFitnessEvaluator<Texture2D>(_renderer, _fitnessEvalutor);
            _evolutionEngine = new BasicEngine<EvoLisaImageCandidate>(_candidateGenerator, _candidateEvaluator);
        }

        ~SimpleEvolutionSystemOpenCL()
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

        public void SaveBitmap(EvoLisaImageCandidate currentBestCandidate, string filePath)
        {
            Bitmap bitmap;
            _renderer.Render(currentBestCandidate, out bitmap);
            bitmap.Save(filePath);
        }
    }
}