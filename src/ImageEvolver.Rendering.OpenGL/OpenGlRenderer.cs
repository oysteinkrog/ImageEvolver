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
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ImageEvolver.Rendering.OpenGL
{
    internal sealed class OpenGlRenderer : RenderingContextBase
    {
        private readonly Size _size;
        private FrameBuffer _frameBuffer;
        private GLManager _glManager;
        private RenderOptions _renderOptions;
        private DefaultTechnique _technique;

        public OpenGlRenderer(Size size)
        {
            _size = size;
            // init, run on the gl thread
            GLTaskFactory.StartNew(() =>
            {
                _renderOptions = new RenderOptions(_size.Width, _size.Height, Window.WindowState, VSyncMode.Off);
                _glManager = new GLManager(_renderOptions);
                _frameBuffer = new FrameBuffer(_size.Width, _size.Height, 1, false);
                _technique = new DefaultTechnique();
                if (!_technique.Initialise())
                {
                    throw new Exception("Technique failed to initialize");
                }
            });
        }

        public Task<Bitmap> RenderCandidateAsync(IImageCandidate candidate)
        {
            return GLTaskFactory.StartNew(() => RenderCandidateInternal(candidate));
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

        private Bitmap RenderCandidateInternal(IImageCandidate candidate)
        {
            _glManager.PushRenderState();

            _glManager.BlendingEnabled = true;
            _glManager.BlendingSource = BlendingFactorSrc.SrcAlpha;
            _glManager.BlendingDestination = BlendingFactorDest.OneMinusSrcAlpha;

            // Use a orthographic projection
            _glManager.Projection = _renderOptions.Ortho;
            _glManager.World = Matrix4.Identity;
            _glManager.View = Matrix4.Identity;
            
            _glManager.PushFrameBuffer(_frameBuffer);

            // Set the background color
            _glManager.ClearColor = candidate.BackgroundColor;
            _glManager.ClearScreen(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _glManager.BindTechnique(_technique);

            foreach (var feature in candidate.Features)
            {
                Render(feature);
            }

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

            //            bitmap.Save("test.bmp");

            bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);

            // Restore the previous frame buffer
            _glManager.PopFrameBuffer();

            // Restore the settings saved by calling PushRenderState at the start of this method
            _glManager.PopRenderState();

            return bitmap;
        }

        private void RenderPolygon(PolygonFeature feature)
        {
            var vertices2 = feature.Points.Select(a => new Vector2(a.X, a.Y))
                                   .ToList();

            List<ushort> indices = null;
            var edges = new Delaunay2D.DelaunayTriangulator();
            if (edges.Initialize(vertices2, 0.00001f))
            {
                edges.Process();
            }

            var e = edges.ToArray();
            var numTriangles = edges.ToIndexBuffer(out indices);

            var indicesArray = indices.ToArray();
            var vectorPointsArray = vertices2.ToArray();

            // Create three vertex buffers. One for each data type (vertex, texture coordinate and normal)
            var verticesBuffer = new VertexBuffer(BufferUsageHint.StaticDraw, (int) BufferAttribute.Vertex, vectorPointsArray);
            var indexBuffer = new IndexBuffer(BufferUsageHint.StaticDraw, indicesArray);

            // Create the vertex array which encapsulates the state changes needed to enable the vertex buffers
            var vertexArray = new VertexArray(indexBuffer, verticesBuffer);

            _technique.UseTexture = false;
            _technique.DrawColor = new Color4(feature.Color.Red/255f, feature.Color.Green/255f, feature.Color.Blue/255f, feature.Color.Alpha/255f);

            // Draw the data
            _glManager.BindVertexArray(vertexArray);
            _glManager.DrawElementsIndexed(BeginMode.Triangles, indicesArray.Length, 0);

            verticesBuffer.ClearResources();
            indexBuffer.ClearResources();
            vertexArray.ClearResources(true);
        }
    }
}