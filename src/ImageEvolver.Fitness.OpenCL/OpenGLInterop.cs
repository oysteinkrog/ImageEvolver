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
using System.Runtime.InteropServices;
using Cloo;
using OpenTK.Graphics;

namespace ImageEvolver.Fitness.OpenCL
{
    public static class OpenGLInterop
    {
        public static ComputeContextPropertyList GetInteropProperties(GraphicsContext graphicsContext, ComputePlatform computePlatform)
        {
            // TODO: add support for other types of gl/cl interop (linux, os x etc)
            IntPtr curDC = wglGetCurrentDC();
            var ctx = (IGraphicsContextInternal) graphicsContext;
            IntPtr rawContextHandle = ctx.Context.Handle;
            var props = new List<ComputeContextProperty>
                        {
                            new ComputeContextProperty(ComputeContextPropertyName.CL_GL_CONTEXT_KHR, rawContextHandle),
                            new ComputeContextProperty(ComputeContextPropertyName.CL_WGL_HDC_KHR, curDC),
                            new ComputeContextProperty(ComputeContextPropertyName.Platform, computePlatform.Handle.Value)
                        };
            var properties = new ComputeContextPropertyList(props);
            return properties;
        }

        [DllImport("opengl32.dll")]
        private static extern IntPtr wglGetCurrentDC();
    }
}