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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using ImageEvolver.Core;
using Koeky3D;
using Koeky3D.BufferHandling;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ImageEvolver.Rendering.OpenGL
{
    public sealed class GenericFeaturesRendererOpenGL : RenderingContextBase, IDisposable, IImageCandidateRenderer<IImageCandidate, Bitmap>
    {
        private readonly Size _size;
        private FrameBuffer _frameBuffer;
        private GLManager _glManager;
        private RenderOptions _renderOptions;
        private RenderingTechnique _technique;

        public GenericFeaturesRendererOpenGL(Size size)
        {
            _size = size;
            // init, run on the gl thread
            GLTaskFactory.StartNew(() =>
            {
                _renderOptions = new RenderOptions(_size.Width, _size.Height, Window.WindowState, VSyncMode.Off);
                _glManager = new GLManager(_renderOptions);
                _frameBuffer = new FrameBuffer(_size.Width, _size.Height, 1, true);
                _technique = new RenderingTechnique();
                if (!_technique.Initialise())
                {
                    throw new Exception("Technique failed to initialize");
                }

                _glManager.PushRenderState();

                _glManager.DepthTestEnabled = true;
                _glManager.DepthTestFunction = DepthFunction.Less;

                _glManager.BlendingEnabled = true;
                _glManager.BlendingSource = BlendingFactorSrc.SrcAlpha;
                _glManager.BlendingDestination = BlendingFactorDest.OneMinusSrcAlpha;

                _glManager.PushFrameBuffer(_frameBuffer);
            });
        }
        
        public Bitmap Render(IImageCandidate candidate)
        {
            var renderTask = RenderCandidateAsync(candidate);
            return renderTask.Result;
        }

        public Task<Bitmap> RenderCandidateAsync(IImageCandidate candidate)
        {
            return GLTaskFactory.StartNew(() =>
            {
                RenderCandidateInternal(candidate);

                var bitmap = new Bitmap(_size.Width, _size.Height);
                var data = bitmap.LockBits(new Rectangle(0, 0, _size.Width, _size.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
                try
                {
                    GL.ReadPixels(0, 0, _size.Width, _size.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                return bitmap;
            });
        }


        private void RenderCandidateInternal(IImageCandidate candidate)
        {
            // all the features are given their own "layer", startin with negative z-index and building up to our znear (0)
            var numLayers = candidate.Features.Count();

            var zLayerDelta = 1f;

            // Use a orthographic projection
            var zNear = 0f;
            var zFar = -(numLayers*zLayerDelta);

            // use pixel offset of 0.5f, due to how the opengl rasterization rules specify that a pixel position is in it's center (0.5f)
            // we want gdi/direct3d behaviour, where the pixel position is in the top left corner

            const float pixelOffset = -0.5f;
            _glManager.Projection = Matrix4.CreateOrthographicOffCenter(0f + pixelOffset,
                                                                        _size.Width + pixelOffset,
                                                                        _size.Height + pixelOffset,
                                                                        0f + pixelOffset,
                                                                        -(zNear + zLayerDelta),
                                                                        -(zFar - zLayerDelta));
            _glManager.World = Matrix4.Identity;
            _glManager.View = Matrix4.Identity;

            // Set the background color
            _glManager.ClearColor = candidate.BackgroundColor;
            _glManager.ClearScreen(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _glManager.BindTechnique(_technique);

            var vertexes = new List<Vector3>();
            var indices = new List<ushort>();
            var colors = new List<Color4>();

            var zIndex = zFar;
            foreach (var feature in candidate.Features)
            {
                var result = TriangleGeometryGenerator.GenerateGeometry(feature);

                indices.AddRange(result.IndexList.Select(a => (ushort) (a + vertexes.Count)));
                vertexes.AddRange(result.VertexList.Select(a => new Vector3(a.X, a.Y, zIndex)));
                colors.AddRange(result.ColorList);

                zIndex += zLayerDelta;
                Debug.Assert(zIndex <= zNear);
            }

            // Create three vertex buffers. One for each data type (vertex, texture coordinate and normal)
            var vertexBuffer = new VertexBuffer(BufferUsageHint.StaticDraw, (int) BufferAttribute.Vertex, vertexes.ToArray());
            var colorBuffer = new VertexBuffer(BufferUsageHint.StaticDraw, (int) BufferAttribute.Color, colors.ToArray());
            var indexBuffer = new IndexBuffer(BufferUsageHint.StaticDraw, indices.ToArray());

            // Create the vertex array which encapsulates the state changes needed to enable the vertex buffers
            var vertexArray = new VertexArray(indexBuffer, vertexBuffer, colorBuffer);

            // Draw the data
            _glManager.BindVertexArray(vertexArray);
            _glManager.DrawElementsIndexed(BeginMode.Triangles, indexBuffer.Count, 0);

            vertexArray.ClearResources(true);
        }
    }
}