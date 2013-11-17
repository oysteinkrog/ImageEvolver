﻿using System;
using System.Drawing;
using System.Windows.Forms;
using Clpp.Core.Utilities;
using GLFrameWork.Shapes;
using ImageEvolver.Algorithms.EvoLisa;
using ImageEvolver.Apps.ConsoleTestApp;
using ImageEvolver.Core.Engines;
using ImageEvolver.Rendering.OpenGL;
using Koeky3D;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ImageEvolver.Apps.OpenGLTestApp
{
    public class TestWindow : GameWindow
    {
        private readonly Bitmap _sourceImage;
        private BasicEngine<EvoLisaImageCandidate>.CandidateDetails _bestCandidate;

        /// <summary>
        ///     The glManager takes care of opengl state changes. It is also used to bind opengl object
        /// </summary>
        private GLManager _glManager;

        private BasicEngine<EvoLisaImageCandidate>.PerformanceDetails _perfDetails;

        /// <summary>
        ///     Contains some render options
        /// </summary>
        private RenderOptions _renderOptions;

        private SimpleEvolutionSystemOpenCL _simpleEvolutionSystem;


        private RenderTechnique _technique;
        private bool _updateRender;

        public TestWindow(Bitmap sourceImage, SimpleEvolutionSystemOpenCL evolutionSystem, OpenGlContext openGlContext)
            : base(
                800,
                600,
                openGlContext.GraphicsMode,
                "ImageEvolder OpenGL Test",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                1,
                0,
                GraphicsContextFlags.Default,
                openGlContext.GraphicsContext)
        {
            _sourceImage = sourceImage;
            _simpleEvolutionSystem = evolutionSystem;
            _bestCandidate = _simpleEvolutionSystem.Engine.BestCandidate;
        }

        protected override void Dispose(bool managed)
        {
            if (managed)
            {
                DisposeHelper.Dispose(ref _simpleEvolutionSystem);
            }

            base.Dispose(managed);
        }

        public void NotifyUpdateRender()
        {
            _updateRender = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Initialise the GLFramework managers
            _renderOptions = new RenderOptions(Width, Height, base.WindowState, VSync);
            _glManager = new GLManager(_renderOptions)
                         {
                             ClearColor = Color4.LightBlue
                         };

            // Create the render technique and initialize it
            _technique = new RenderTechnique();
            if (!_technique.Initialise())
            {
                MessageBox.Show(_technique.ErrorMessage);
            }

            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (_updateRender)
            {
                // By calling PushRenderState we save the OpenGL settings exposed by the GLManager class
                _glManager.PushRenderState();

                // Bind the current render technique
                _glManager.BindTechnique(_technique);

                // Clear the screen using a blue color
                _glManager.ClearColor = Color4.Gray;
                _glManager.ClearScreen(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                _glManager.BlendingEnabled = false;

                _perfDetails = _simpleEvolutionSystem.Engine.GetPerformanceDetails();
                _bestCandidate = _simpleEvolutionSystem.Engine.BestCandidate;

                Console.WriteLine(
                                  "Selected {0}, Generation {1}, BestFit {2:0.000}, Mutation {3:0.000}, Rendering  {4:0.000}, Fitness {5:0.000}",
                                  _simpleEvolutionSystem.Engine.Selected,
                                  _bestCandidate.Generation,
                                  _bestCandidate.Fitness,
                                  _perfDetails.RelativeMutationTime,
                                  _perfDetails.RelativeFitnessEvaluationTime*_perfDetails.FitnessEvaluationDetails.RelativeRenderingTime,
                                  _perfDetails.RelativeFitnessEvaluationTime*
                                  _perfDetails.FitnessEvaluationDetails.RelativeFitnessEvaluationTime);

                _glManager.BindTexture(_simpleEvolutionSystem.RenderBuffer.PrimaryTexture, TextureUnit.Texture0);

                // Use a orthographic projection
                _glManager.Projection = _renderOptions.Ortho;

                // Set the view to the identity.
                _glManager.View = Matrix4.Identity;

                // We can use the ShapeDrawer class to easily draw default shapes. 
                // Not all shapes are implemented yet tough.
                ShapeDrawer.Begin(_glManager);

                // We draw a quad at the top left corner with the framebuffer's texture
                ShapeDrawer.DrawQuad(_glManager,
                                     new Vector2(0, 0),
                                     new Vector2(_simpleEvolutionSystem.RenderBuffer.Width, _simpleEvolutionSystem.RenderBuffer.Height));

                ShapeDrawer.End(_glManager);

                // Restore the settings saved by calling PushRenderState at the start of this method
                _glManager.PopRenderState();

                // Display the image to the user
                SwapBuffers();

                base.OnRenderFrame(e);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            // Update the render options resolution
            _renderOptions.Resolution = base.Size;

            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Set the title to show the fps
            base.Title = string.Format("Candidates/s: {0:0.00000} Generations/s: {1:0.00000}",
                                       _simpleEvolutionSystem.Engine.Candidates/
                                       _simpleEvolutionSystem.Engine.TotalSimulationTime.TotalSeconds,
                                       _bestCandidate.Generation/_simpleEvolutionSystem.Engine.TotalSimulationTime.TotalSeconds);

            base.OnUpdateFrame(e);
        }
    }
}