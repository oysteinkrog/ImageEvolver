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
using System.Drawing;
using ImageEvolver.Core;
using Config = OpenTK.Configuration;

namespace ImageEvolver.Rendering.OpenGL
{
    public sealed class GenericFeaturesRendererOpenGL : IDisposable, IImageCandidateRenderer<IImageCandidate, Bitmap>
    {
        private OpenGlRenderer _glRenderer;

        public GenericFeaturesRendererOpenGL(Size size)
        {
            
            _glRenderer = new OpenGlRenderer(size);
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
                if (_glRenderer != null)
                {
                    _glRenderer.Dispose();
                    _glRenderer = null;
                }
            }
            // free native resources if there are any.
        }

        public Bitmap Render(IImageCandidate candidate)
        {
            var renderTask = _glRenderer.RenderCandidateAsync(candidate);
            return renderTask.Result;
        }
    }
}