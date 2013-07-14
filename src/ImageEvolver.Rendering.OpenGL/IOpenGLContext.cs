using System;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace ImageEvolver.Rendering.OpenGL
{
    public interface IOpenGLContext : IDisposable
    {
        IGraphicsContext GraphicsContext { get; }
        TaskFactory TaskFactory { get; }
        INativeWindow Window { get; }
    }
}