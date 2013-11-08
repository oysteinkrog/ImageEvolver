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
using System.Diagnostics;
using System.Drawing;
using ImageEvolver.Core.Utilities;
using ImageEvolver.Rendering.OpenGL;
using NUnit.Framework;

namespace ImageEvolver.UnitTests.Rendering
{
    [TestFixture]
    public class RendererPerformanceTests
    {
        [Test]
        public void TestOpenGLRenderingGeometryCache([Values(false, true)] bool useGeometryCache, [Values(10000)] int times)
        {
            var size = new Size(400, 400);
            using (var renderBuffer = new Bitmap(size.Width, size.Height))
            {
                using (var renderer = new GenericFeaturesRendererOpenGL(size, null, useGeometryCache))
                {
                    var sw = new Stopwatch();
                    for (int i = 0; i < times; i++)
                    {
                        var candidate = new TestCandidateRandom(size, new Range<int>(0, 10), new Range<int>(0, 10));
                        {
                            sw.Start();
                            renderer.Render(candidate, renderBuffer);
                            sw.Stop();
                        }
                    }
                    Console.WriteLine(sw.Elapsed.TotalMilliseconds/times + "ms/t");
                }
            }
        }
    }
}