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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Features;
using ImageEvolver.Rendering.OpenGL.Triangulation;
using Koeky3D;
using Koeky3D.BufferHandling;
using Koeky3D.Shaders;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Config = OpenTK.Configuration;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ImageEvolver.Rendering.OpenGL
{
    public sealed class GenericFeaturesRendererOpenGL : IDisposable, IImageCandidateRenderer<IImageCandidate, Bitmap>
    {
        private BlockingCollection<ProcessingItem> _renderQueueBlocking;
        private readonly Size _size;
        private FrameBuffer _frameBuffer;
        private GLManager _glManager;
        private Task _processingTask;
        private Rectangle _rectangle;
        private RenderOptions _renderOptions;
        private DefaultTechnique _technique;

        public GenericFeaturesRendererOpenGL(Size size)
        {
            _size = size;
            _rectangle = new Rectangle(0, 0, _size.Width, _size.Height);

            _processingTask = Task.Factory.StartNew(ProcessTasks, TaskCreationOptions.LongRunning);
            _renderQueueBlocking = new BlockingCollection<ProcessingItem>(new ConcurrentQueue<ProcessingItem>());
        }

        ~GenericFeaturesRendererOpenGL()
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
                // free managed resources
                if (_renderQueueBlocking != null)
                {
                    _renderQueueBlocking.CompleteAdding();
                    _renderQueueBlocking.Dispose();
                    _renderQueueBlocking = null;
                }

                if (_processingTask != null)
                {
                    _processingTask.Wait();
                    _processingTask.Dispose();
                    _processingTask = null;
                }
            }
            // free native resources if there are any.
        }

        public Bitmap Render(IImageCandidate candidate)
        {
            var processingItem = new ProcessingItem
                       {
                           Candidate = candidate,
                           Event = new ManualResetEventSlim()
                       };
            _renderQueueBlocking.Add(processingItem);
            processingItem.Event.Wait();
            return processingItem.Bitmap;
        }

        private Bitmap ProcessCandidate(IImageCandidate candidate)
        {
            // Set the background color
            _glManager.ClearColor = Color4.White;
            _glManager.ClearScreen(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _glManager.PushRenderState();

            _glManager.Projection = _renderOptions.Projection;
            _glManager.Projection = _renderOptions.Ortho;
            _glManager.View = Matrix4.Identity;

            _glManager.PushFrameBuffer(_frameBuffer);

            // Clear the screen using a blue color
            _glManager.ClearColor = Color4.Black;
            _glManager.ClearScreen(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (IFeature feature in candidate.Features)
            {
                Render(feature);
            }

            var bitmap = new Bitmap(_size.Width, _size.Height);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, _size.Width, _size.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            try
            {
                GL.ReadPixels(0, 0, _size.Width, _size.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

//            bitmap.Save("test.bmp");

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            // Restore the previous frame buffer
            _glManager.PopFrameBuffer();

            // Restore the settings saved by calling PushRenderState at the start of this method
            _glManager.PopRenderState();

            return bitmap;
        }

        private void ProcessTasks()
        {
            // create a new opengl context, this works because this method is run on a new thread..
            INativeWindow window = new NativeWindow();
            var graphicsMode = new GraphicsMode(32, 24, 0, 0);
            IGraphicsContext context = new GraphicsContext(graphicsMode, window.WindowInfo);
            context.MakeCurrent(window.WindowInfo);

            (context as IGraphicsContextInternal).LoadAll();

            _renderOptions = new RenderOptions(_size.Width, _size.Height, window.WindowState, VSyncMode.Off);
            _glManager = new GLManager(_renderOptions);
            _frameBuffer = new FrameBuffer(_size.Width, _size.Height, 1, false);
            _technique = new DefaultTechnique();
            if (!_technique.Initialise())
            {
                throw new Exception("Technique failed to initialize");
            }

            foreach (ProcessingItem processingItem in _renderQueueBlocking.GetConsumingEnumerable())
            {
                processingItem.Bitmap = ProcessCandidate(processingItem.Candidate);
                processingItem.Event.Set();
            }
        }

        private void Render(IFeature feature)
        {
            var polygonFeature = feature as PolygonFeature;
            if (polygonFeature != null)
            {
                RenderPolygon(polygonFeature);
            }
            else
            {
                throw new NotSupportedException(string.Format("Rendering feature of type {0} is not supported", feature.GetType()));
            }
        }

        private void RenderPolygon(PolygonFeature feature)
        {
            List<Vector2> points = feature.Points.Select(a => new Vector2(a.X, a.Y))
                                          .ToList();
            List<ushort> indices = null;
            var edges = new Delaunay2D.DelaunayTriangulator();
            if (edges.Initialize(points, 0.00001f))
            {
                edges.Process();
            }

            IntegerTriangle[] e = edges.ToArray();
            int numTriangles = edges.ToIndexBuffer(out indices);
            
            // Use a orthographic projection
            _glManager.Projection = _renderOptions.Ortho;
            _glManager.World = Matrix4.Identity;
            _glManager.View = Matrix4.Identity;

            ushort[] indicesArray = indices.ToArray();
            Vector2[] vectorPointsArray = points.ToArray();

            // Create three vertex buffers. One for each data type (vertex, texture coordinate and normal)
            var verticesBuffer = new VertexBuffer(BufferUsageHint.StaticDraw, (int) BufferAttribute.Vertex, vectorPointsArray);
            var indexBuffer = new IndexBuffer(BufferUsageHint.StaticDraw, indicesArray);

            // Create the vertex array which encapsulates the state changes needed to enable the vertex buffers
            var vertexArray = new VertexArray(verticesBuffer);

            _glManager.BindTechnique(_technique);

            _glManager.BlendingEnabled = true;
            _glManager.BlendingSource = BlendingFactorSrc.SrcAlpha;;
            _glManager.BlendingDestination = BlendingFactorDest.OneMinusSrcAlpha;

            _technique.UseTexture = false;
            _technique.DrawColor = new Color4(feature.Color.Red / 255f, feature.Color.Green / 255f, feature.Color.Blue / 255f, feature.Color.Alpha / 255f);
            
            // Draw the data
            _glManager.BindVertexArray(vertexArray);
            _glManager.BindIndexBuffer(indexBuffer);
            _glManager.DrawElementsIndexed(BeginMode.Triangles, indicesArray.Length, 0);

            verticesBuffer.ClearResources();
            indexBuffer.ClearResources();
            vertexArray.ClearResources(true);
        }

        private class ProcessingItem
        {
            internal Bitmap Bitmap;
            internal IImageCandidate Candidate;
            internal ManualResetEventSlim Event;
        }
    }
}