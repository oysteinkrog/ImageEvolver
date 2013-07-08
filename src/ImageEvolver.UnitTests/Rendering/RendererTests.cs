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

using System.Collections.Generic;
using System.Drawing;
using ImageEvolver.Core;
using ImageEvolver.Core.Mutation;
using ImageEvolver.Features;
using ImageEvolver.Rendering.Bitmap;
using ImageEvolver.Rendering.OpenGL;
using NUnit.Framework;

namespace ImageEvolver.UnitTests.Rendering
{
    [TestFixture]
    public class RendererTests
    {
        [Test]
        public void SinglePolygonTest_Bitmap()
        {
            var size = new Size(400, 400);
            var candidate = new TestCandidate(size);
            using (var renderer = new GenericFeaturesRendererBitmap(size))
            {
                renderer.Render(candidate)
                        .Save(@"SinglePolygonTest_Bitmap.bmp");
            }
        }

        [Test]
        public void SinglePolygonTest_OpenGL()
        {
            var size = new Size(400, 400);
            var candidate = new TestCandidate(size);
            using (var renderer = new GenericFeaturesRendererOpenGL(size))
            {
                renderer.Render(candidate)
                        .Save(@"SinglePolygonTest_OpenGL.bmp");
            }
        }
    }

    internal class TestCandidate : IImageCandidate
    {
        public TestCandidate(Size size)
        {
            Size = size;
        }

        public IEnumerable<IFeature> Features
        {
            get
            {
                yield return new PolygonFeature(new List<PointFeature>
                                                {
                                                    new PointFeature(100, 20),
                                                    new PointFeature(250, 20),
                                                    new PointFeature(180, 200),
                                                    new PointFeature(80, 200)
                                                },
                                                new ColorFeature(0, 0, 255, 128));

                yield return new PolygonFeature(new List<PointFeature>
                                                {
                                                    new PointFeature(50, 10),
                                                    new PointFeature(300, 150),
                                                    new PointFeature(200, 300),
                                                    new PointFeature(20, 400)
                                                },
                                                new ColorFeature(255, 0, 0, 128));

                yield return new PolygonFeature(new List<PointFeature>
                                                {
                                                    new PointFeature(150, 50),
                                                    new PointFeature(400, 150),
                                                    new PointFeature(130, 400)
                                                },
                                                new ColorFeature(0, 255, 0, 128));
            }
        }

        public Color BackgroundColor
        {
            get { return Color.White; }
        }

        public Size Size { get; set; }
    }
}