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
using Koeky3D.BufferHandling;
using Koeky3D.Textures;

namespace ImageEvolver.Apps.ConsoleTestApp
{
    public class SimpleEvolutionSystemOpenCL : IDisposable
    {
        private BasicPseudoRandomProvider _basicPseudoRandomProvider;
        private CandidateFitnessEvaluator<FrameBuffer> _candidateEvaluator;
        private ICandidateGenerator<EvoLisaImageCandidate> _candidateGenerator;
        private EvoLisaAlgorithm _evoLisaAlgorithm;
        private EvoLisaAlgorithmSettings _evoLisaAlgorithmSettings;
        private BasicEngine<EvoLisaImageCandidate> _evolutionEngine;
        private FitnessEvaluatorOpenCL _fitnessEvalutor;
        private GenericFeaturesRendererOpenGL _renderer;
        private FrameBuffer _renderBuffer;

        public SimpleEvolutionSystemOpenCL(Bitmap sourceImage)
        {
            //_renderer = new GenericFeaturesRendererBitmap(sourceImage.Size);
            var openGlContext = new OpenGlContext(sourceImage.Size);
            var genericFeaturesRendererOpenGL = new GenericFeaturesRendererOpenGL(sourceImage.Size, openGlContext);
            _renderer = genericFeaturesRendererOpenGL;
            openGlContext.TaskFactory.StartNew(() =>
            {
                _renderBuffer = new FrameBuffer(sourceImage.Width, sourceImage.Height, 1, false);
                _fitnessEvalutor = new FitnessEvaluatorOpenCL(sourceImage, _renderBuffer, openGlContext);
            })
                         .Wait();

            _evoLisaAlgorithmSettings = new EvoLisaAlgorithmSettings();
            _basicPseudoRandomProvider = new BasicPseudoRandomProvider(0);
            _evoLisaAlgorithm = new EvoLisaAlgorithm(sourceImage, _evoLisaAlgorithmSettings, _basicPseudoRandomProvider);
            _candidateGenerator = _evoLisaAlgorithm.CreateCandidateGenerator();
            _candidateEvaluator = new CandidateFitnessEvaluator<FrameBuffer>(_renderer, _fitnessEvalutor, _renderBuffer);
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


        public void RenderToBitmap(EvoLisaImageCandidate currentBestCandidate, Bitmap outputBuffer)
        {
            _renderer.Render(currentBestCandidate, outputBuffer);
        }

        public void SaveBitmap(EvoLisaImageCandidate candidate, string filePath)
        {
            using (var output = new Bitmap(_renderBuffer.Width, _renderBuffer.Height))
            {
                RenderToBitmap(candidate, output);
                output.Save(filePath);
            }
        }
    }
}