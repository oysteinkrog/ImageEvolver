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
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace ImageEvolver.Rendering.OpenGL
{
    internal class RenderingContextBase : IDisposable
    {
        protected readonly TaskFactory GLTaskFactory;
        protected GraphicsContext GraphicsContext;
        protected GraphicsMode GraphicsMode;
        protected NativeWindow Window;
        private bool _disposed;

        protected RenderingContextBase()
        {
            GLTaskFactory = new TaskFactory(new SingleThreadTaskScheduler());

            // intialize the context, important that this is run on the correct thread
            GLTaskFactory.StartNew(() =>
            {
                Window = new NativeWindow();

                GraphicsMode = new GraphicsMode(32, 24, 0, 4);
                GraphicsContext = new GraphicsContext(GraphicsMode, Window.WindowInfo);
                GraphicsContext.MakeCurrent(Window.WindowInfo);
                GraphicsContext.LoadAll();
            });
        }

        ~RenderingContextBase()
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
                if (disposing)
                {
                    // dispose managed resources
                    if (GraphicsContext != null)
                    {
                        GraphicsContext.Dispose();
                        GraphicsContext = null;
                    }

                    if (Window != null)
                    {
                        Window.Dispose();
                        Window = null;
                    }
                }
                // free native resources if there are any
            }
            finally
            {
                _disposed = true;
            }
        }
    }
}