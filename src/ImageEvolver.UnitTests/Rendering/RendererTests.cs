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

using System.Drawing;
using System.Threading.Tasks;
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
            using (var renderBuffer = new Bitmap(size.Width, size.Height))
            {
                var candidate = new TestCandidate(size);
                using (var renderer = new GenericFeaturesRendererBitmap(size))
                {
                    renderer.RenderAsync(candidate, renderBuffer);
                    renderBuffer.Save(@"SinglePolygonTest_Bitmap.bmp");
                }
            }
        }

        [Test]
        public async Task SinglePolygonTest_OpenGL()
        {
            var size = new Size(400, 400);
            using (var renderBuffer = new Bitmap(size.Width, size.Height))
            {
                var candidate = new TestCandidate(size);
                using (var renderer = await GenericFeaturesRendererOpenGL.Create(size))
                {
                    renderer.RenderAsync(candidate, renderBuffer);
                    renderBuffer.Save(@"SinglePolygonTest_OpenGL.bmp");
                }
            }
        }
    }
}