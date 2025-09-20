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

        private double _time;
        private double _lastColorChangeTime;
        private readonly Random _random = new Random();
        private int _colorLocation;

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

        uniform vec4 uColor;

        void main()
        {
            FragColor = uColor;
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
            _shader.Use();
            _colorLocation = _shader.GetUniformLocation("uColor");
        }

        private void OnRender(double deltaTime)
        {
            _time += deltaTime;

            if (_time - _lastColorChangeTime >= 1.0)
            {
                _lastColorChangeTime = _time;
                float r = (float)_random.NextDouble();
                float g = (float)_random.NextDouble();
                float b = (float)_random.NextDouble();
                _gl!.Uniform4(_colorLocation, r, g, b, 1.0f);
            }

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