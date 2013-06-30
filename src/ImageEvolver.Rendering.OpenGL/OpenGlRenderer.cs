﻿#region Copyright

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
        private RenderingTechnique _technique;

        public OpenGlRenderer(Size size)
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
            });
        }

        public Task<Bitmap> RenderCandidateAsync(IImageCandidate candidate)
        {
            return GLTaskFactory.StartNew(() => RenderCandidateInternal(candidate));
        }

        private FeatureGeometry GenerateGeometry(IFeature feature)
        {
            var polygonFeature = feature as PolygonFeature;
            if (polygonFeature != null)
            {
                return GeneratePolygonGeometry(polygonFeature);
            }
            throw new NotSupportedException(string.Format("Rendering feature of type {0} is not supported", feature.GetType()));
        }

        private FeatureGeometry GeneratePolygonGeometry(PolygonFeature feature)
        {
            var vertexList = feature.Points.Select(a => new Vector2(a.X, a.Y))
                                    .ToList();

            List<ushort> indexList;
            var edges = new Delaunay2D.DelaunayTriangulator();
            if (edges.Initialize(vertexList, 0.00001f))
            {
                edges.Process();
            }

            edges.ToIndexBuffer(out indexList);

            var color = new Color4(feature.Color.Red/255f, feature.Color.Green/255f, feature.Color.Blue/255f, feature.Color.Alpha/255f);
            var colorList = new List<Color4>();
            for (var i = 0; i < vertexList.Count; i++)
            {
                colorList.Add(color);
            }

            return new FeatureGeometry
                   {
                       VertexList = vertexList,
                       IndexList = indexList,
                       ColorList = colorList,
                   };
        }

        private Bitmap RenderCandidateInternal(IImageCandidate candidate)
        {
            _glManager.PushRenderState();

            _glManager.DepthTestEnabled = true;
            _glManager.DepthTestFunction = DepthFunction.Less;

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

            var vertexes = new List<Vector3>();
            var indices = new List<ushort>();
            var colors = new List<Color4>();

            var zIndex = -(float) candidate.Features.Count();

            foreach (var feature in candidate.Features)
            {
                var result = GenerateGeometry(feature);

                indices.AddRange(result.IndexList.Select(a => (ushort) (a + vertexes.Count)));
                vertexes.AddRange(result.VertexList.Select(a => new Vector3(a.X, a.Y, zIndex)));
                colors.AddRange(result.ColorList);

                zIndex++;
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

        private struct FeatureGeometry
        {
            public List<Color4> ColorList { get; set; }
            public List<ushort> IndexList { get; set; }
            public List<Vector2> VertexList { get; set; }
        }
    }
}