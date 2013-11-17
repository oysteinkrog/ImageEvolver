using System;
using System.Drawing;
using System.Threading.Tasks;
using Clpp.Core.Utilities;
using ImageEvolver.Algorithms.EvoLisa;
using ImageEvolver.Algorithms.EvoLisa.Settings;
using ImageEvolver.Core;
using ImageEvolver.Core.Engines;
using ImageEvolver.Core.Random;
using ImageEvolver.Fitness.OpenCL;
using ImageEvolver.Rendering.OpenGL;
using Koeky3D.BufferHandling;

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

        private SimpleEvolutionSystemOpenCL(Bitmap sourceImage,
                                            OpenGlContext openGlContext,
                                            FrameBuffer renderBuffer,
                                            FitnessEvaluatorOpenCL fitnessEvalutor,
            GenericFeaturesRendererOpenGL renderer)
        {
            OpenGlContext = openGlContext;
            RenderBuffer = renderBuffer;
            _fitnessEvalutor = fitnessEvalutor;
            _renderer = renderer;
            _evoLisaAlgorithmSettings = new EvoLisaAlgorithmSettings();
            _basicPseudoRandomProvider = new BasicPseudoRandomProvider(0);
            _evoLisaAlgorithm = new EvoLisaAlgorithm(sourceImage, _evoLisaAlgorithmSettings, _basicPseudoRandomProvider);
            _candidateGenerator = _evoLisaAlgorithm.CreateCandidateGenerator();
            _candidateEvaluator = new CandidateFitnessEvaluator<FrameBuffer>(_renderer, _fitnessEvalutor, RenderBuffer);
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

        public OpenGlContext OpenGlContext { get; private set; }
        public FrameBuffer RenderBuffer { get; private set; }

        public static async Task<SimpleEvolutionSystemOpenCL> Create(Bitmap sourceImage)
        {
            OpenGlContext openGlContext = await OpenGlContext.Create(sourceImage.Size);
            return await await openGlContext.TaskFactory.StartNew(async () =>
            {
                var frameBuffer = new FrameBuffer(sourceImage.Width, sourceImage.Height, 1, false);
                var fitnessEvalutor = await FitnessEvaluatorOpenCL.Create(sourceImage, frameBuffer, openGlContext);
                var renderer = await GenericFeaturesRendererOpenGL.Create(sourceImage.Size, openGlContext);
                return new SimpleEvolutionSystemOpenCL(sourceImage, openGlContext, frameBuffer, fitnessEvalutor, renderer);
            });
        }


        public void RenderToBitmap(EvoLisaImageCandidate currentBestCandidate, Bitmap outputBuffer)
        {
            _renderer.RenderAsync(currentBestCandidate, outputBuffer);
        }

        public void SaveBitmap(EvoLisaImageCandidate candidate, string filePath)
        {
            using (var output = new Bitmap(RenderBuffer.Width, RenderBuffer.Height))
            {
                RenderToBitmap(candidate, output);
                output.Save(filePath);
            }
        }
    }
}