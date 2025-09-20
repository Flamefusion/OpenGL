using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using Silk.NET.Maths;

namespace OpenGLTest
{
    public class Game : IDisposable
    {
        private readonly IWindow _window;
        private GL? _gl;

        // Our vertex data
        private static readonly float[] _vertices =
        {
            //X    Y      Z
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
             0.0f,  0.5f, 0.0f
        };

        // OpenGL resource wrappers
        private BufferObject<float>? _vbo;
        private VertexArrayObject? _vao;
        private Shader? _shader;

        // Simple Vertex Shader
        private const string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }";

        // Simple Fragment Shader for genrating coloers for each pixel on the triangle
        private const string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
            FragColor = vec4(0.2f, 0.5f, 1.0f, 1.0f);
        }";

        public Game()
        {
            var options = WindowOptions.Default;
            options.Size = new Vector2D<int>(800, 600);
            options.Title = "TRIANGLE - Modular";

            _window = Window.Create(options);

            _window.Load += OnLoad;
            _window.Render += OnRender;
        }

        public void Run()
        {
            _window.Run();
            // After Run() completes, the window is closed, and the IDisposable.Dispose method
            // is called by the 'using' block in Program.cs.
        }

        private void OnLoad()
        {
            _gl = _window.CreateOpenGL();

            _gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _vbo = new BufferObject<float>(_gl, _vertices, BufferTargetARB.ArrayBuffer);
            _vao = new VertexArrayObject(_gl);
            _vao.LinkAttribute(_vbo, 0, 3, VertexAttribPointerType.Float, 3, 0);
            _shader = new Shader(_gl, VertexShaderSource, FragmentShaderSource);
        }

        private void OnRender(double deltaTime)
        {
            _gl!.Clear(ClearBufferMask.ColorBufferBit);

            _vao!.Bind();
            _shader!.Use();

            _gl.DrawArrays(PrimitiveType.Triangles, 0, (uint)_vertices.Length / 3);
        }

        public void Dispose()
        {
            _vbo?.Dispose();
            _vao?.Dispose();
            _shader?.Dispose();
            _gl?.Dispose();
            _window?.Dispose();
        }
    }
}