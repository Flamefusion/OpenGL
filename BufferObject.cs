using Silk.NET.OpenGL;
using System;

namespace OpenGLTest
{
    public class BufferObject<T> : IDisposable where T : unmanaged
    {
        public uint Handle { get; }
        private readonly GL _gl;
        private readonly BufferTargetARB _bufferType;

        public unsafe BufferObject(GL gl, ReadOnlySpan<T> data, BufferTargetARB bufferType)
        {
            _gl = gl;
            _bufferType = bufferType;

            Handle = _gl.GenBuffer();
            Bind();
            fixed (void* d = data)
            {
                _gl.BufferData(bufferType, (nuint)(data.Length * sizeof(T)), d, BufferUsageARB.StaticDraw);
            }
        }

        public void Bind()
        {
            _gl.BindBuffer(_bufferType, Handle);
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(Handle);
        }
    }
}