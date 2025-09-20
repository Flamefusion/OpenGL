namespace OpenGLTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // The using statement ensures that the Game instance is disposed of correctly when the window closes.
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
}