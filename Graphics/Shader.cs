using Silk.NET.OpenGL;
using System;

namespace OpenGLTest
{
    public class Shader : IDisposable
    {
        public uint Handle { get; }
        private readonly GL _gl;

        public Shader(GL gl, string vertexSource, string fragmentSource)
        {
            _gl = gl;

            uint vertexShader = CompileShader(ShaderType.VertexShader, vertexSource);
            uint fragmentShader = CompileShader(ShaderType.FragmentShader, fragmentSource);

            Handle = _gl.CreateProgram();
            _gl.AttachShader(Handle, vertexShader);
            _gl.AttachShader(Handle, fragmentShader);
            _gl.LinkProgram(Handle);

            _gl.GetProgram(Handle, ProgramPropertyARB.LinkStatus, out int status);
            if (status == 0)
            {
                throw new Exception($"Error linking shader program: {_gl.GetProgramInfoLog(Handle)}");
            }

            // The shaders are now linked into the program, so we can detach and delete them.
            _gl.DetachShader(Handle, vertexShader);
            _gl.DetachShader(Handle, fragmentShader);
            _gl.DeleteShader(vertexShader);
            _gl.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            _gl.UseProgram(Handle);
        }

        private uint CompileShader(ShaderType type, string source)
        {
            uint shader = _gl.CreateShader(type);
            _gl.ShaderSource(shader, source);
            _gl.CompileShader(shader);

            _gl.GetShader(shader, ShaderParameterName.CompileStatus, out int status);
            if (status == 0)
            {
                throw new Exception($"Error compiling shader of type {type}: {_gl.GetShaderInfoLog(shader)}");
            }

            return shader;
        }

        public void Dispose()
        {
            _gl.DeleteProgram(Handle);
        }
    }
}