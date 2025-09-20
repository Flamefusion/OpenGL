using Silk.NET.Windowing;
using Silk.NET.OpenGL;

namespace OpenGLTest
{
    class Program
    {
        // Add a '?' to mark these as nullable. This solves the CS8618 error.
        private static IWindow? window;
        private static GL? Gl;

        // Our vertex data
        private static readonly float[] vertices =
        {
            //X    Y      Z
            -0.5f, -0.5f, 0.0f,
             0.5f, -0.5f, 0.0f,
             0.0f,  0.5f, 0.0f
        };

        private static uint VertexBufferObject;
        private static uint VertexArrayObject;
        private static uint ShaderProgram;

        // Simple Vertex Shader
        private const string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 aPosition;

        void main()
        {
            gl_Position = vec4(aPosition, 1.0);
        }";

        // Simple Fragment Shader
        private const string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }";


        static void Main(string[] args)
        {
            var options = WindowOptions.Default;
            options.Size = new Silk.NET.Maths.Vector2D<int>(800, 600);
            options.Title = "TRIANGLE";

            window = Window.Create(options);

            // Assign events
            window.Load += OnLoad;
            window.Render += OnRender;
            window.Closing += OnClose;

            // Run the window
            window.Run();
        }

        // Add the 'unsafe' keyword here. This solves the CS0214 error.
        private static unsafe void OnLoad()
        {
            // Get the OpenGL API instance
            Gl = window.CreateOpenGL();

            // Set the clear color
            Gl.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            // Create and bind Vertex Array Object (VAO)
            VertexArrayObject = Gl.GenVertexArray();
            Gl.BindVertexArray(VertexArrayObject);

            // Create and bind Vertex Buffer Object (VBO)
            VertexBufferObject = Gl.GenBuffer();
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, VertexBufferObject);
            // Upload the vertex data to the VBO
            // This 'fixed' block is why we need the 'unsafe' keyword.
            fixed (float* ptr = vertices)
            {
                Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(float)), ptr, BufferUsageARB.StaticDraw);
            }
            
            // --- Compile Shaders ---
            uint vertexShader = Gl.CreateShader(ShaderType.VertexShader);
            Gl.ShaderSource(vertexShader, VertexShaderSource);
            Gl.CompileShader(vertexShader);
            // Check for compile errors
            Gl.GetShader(vertexShader, ShaderParameterName.CompileStatus, out int status);
            if (status == 0)
            {
                Console.WriteLine($"Error compiling vertex shader: {Gl.GetShaderInfoLog(vertexShader)}");
            }

            uint fragmentShader = Gl.CreateShader(ShaderType.FragmentShader);
            Gl.ShaderSource(fragmentShader, FragmentShaderSource);
            Gl.CompileShader(fragmentShader);
            // Check for compile errors
            Gl.GetShader(fragmentShader, ShaderParameterName.CompileStatus, out status);
            if (status == 0)
            {
                Console.WriteLine($"Error compiling fragment shader: {Gl.GetShaderInfoLog(fragmentShader)}");
            }

            // Link shaders into a shader program
            ShaderProgram = Gl.CreateProgram();
            Gl.AttachShader(ShaderProgram, vertexShader);
            Gl.AttachShader(ShaderProgram, fragmentShader);
            Gl.LinkProgram(ShaderProgram);
            // Check for linking errors
            Gl.GetProgram(ShaderProgram, ProgramPropertyARB.LinkStatus, out status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader program: {Gl.GetProgramInfoLog(ShaderProgram)}");
            }

            // Delete the shaders as they're now linked into our program and no longer necessary
            Gl.DetachShader(ShaderProgram, vertexShader);
            Gl.DetachShader(ShaderProgram, fragmentShader);
            Gl.DeleteShader(vertexShader);
            Gl.DeleteShader(fragmentShader);
            // --- End of Shader Compilation ---

            // Tell OpenGL how to interpret the vertex data
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            Gl.EnableVertexAttribArray(0);

            // Unbind VBO and VAO
            Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        private static void OnRender(double deltaTime)
        {
            // Use the null-forgiving operator '!' because we know Gl is not null here.
            Gl!.Clear(ClearBufferMask.ColorBufferBit);

            // Bind our VAO and use our shader program
            Gl!.BindVertexArray(VertexArrayObject);
            Gl!.UseProgram(ShaderProgram);

            // Draw the triangle
            Gl!.DrawArrays(PrimitiveType.Triangles, 0, 3);
        }

        private static void OnClose()
        {
            // Use the null-forgiving operator '!' because we know Gl is not null here.
            // Cleanup resources
            Gl!.DeleteBuffer(VertexBufferObject);
            Gl!.DeleteVertexArray(VertexArrayObject);
            Gl!.DeleteProgram(ShaderProgram);
        }
    }
}