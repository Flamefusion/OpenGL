using Silk.NET.OpenGL;
using System;

namespace OpenGLTest
{
    public class VertexArrayObject : IDisposable
    {
        public uint Handle { get; }
        private readonly GL _gl;

        public VertexArrayObject(GL gl)
        {
            _gl = gl;
            Handle = _gl.GenVertexArray();
        }

        public unsafe void LinkAttribute<T>(BufferObject<T> vbo, uint location, int count, VertexAttribPointerType type, uint stride, int offset) where T : unmanaged
        {
            Bind();
            vbo.Bind();
            _gl.VertexAttribPointer(location, count, type, false, stride * (uint)sizeof(T), (void*)(offset * sizeof(T)));
            _gl.EnableVertexAttribArray(location);
        }

        public void Bind()
        {
            _gl.BindVertexArray(Handle);
        }

        public void Unbind()
        {
            _gl.BindVertexArray(0);
        }

        public void Dispose()
        {
            _gl.DeleteVertexArray(Handle);
        }
    }
}