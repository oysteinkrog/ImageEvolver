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
using System.Threading.Tasks;
using ImageEvolver.Core.Utilities;
using JetBrains.Annotations;
using OpenTK;
using OpenTK.Graphics;

namespace ImageEvolver.Rendering.OpenGL
{
    public class OpenGlContext : IOpenGLContext
    {
        private readonly TaskFactory _taskFactory;
        private bool _disposed;
        private IGraphicsContext _graphicsContext;
        private GraphicsMode _graphicsMode;
        private INativeWindow _window;

        public OpenGlContext(Size size)
        {
            _taskFactory = new TaskFactory(new SingleThreadTaskScheduler());

            // intialize the context, important that this is run on the correct thread
            TaskFactory.StartNew(() =>
            {
                _graphicsMode = new GraphicsMode(32, 24, 0, 4);

                _window = new NativeWindow(size.Width, size.Height, "OpenGlContext Native Window", GameWindowFlags.Default, _graphicsMode, DisplayDevice.Default);
                
                _graphicsContext = new GraphicsContext(GraphicsMode, Window.WindowInfo);
                _graphicsContext.MakeCurrent(Window.WindowInfo);
                _graphicsContext.LoadAll();
            })
                       .Wait();
        }

        ~OpenGlContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            try
            {
                if (_disposed)
                {
                    return;
                }

                if (disposing)
                {
                    // dispose managed resources
                    DisposeHelper.Dispose(ref _graphicsContext);
                    DisposeHelper.Dispose(ref _window);
                }
                // free native resources if there are any
            }
            finally
            {
                _disposed = true;
            }
        }

        [PublicAPI]
        public GraphicsMode GraphicsMode
        {
            get { return _graphicsMode; }
        }

        [PublicAPI]
        public TaskFactory TaskFactory
        {
            get { return _taskFactory; }
        }

        [PublicAPI]
        public INativeWindow Window
        {
            get { return _window; }
        }

        [PublicAPI]
        public IGraphicsContext GraphicsContext
        {
            get { return _graphicsContext; }
        }
    }
}