using SDL2;


class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        bool success = game.Initialize();
        if (success)
        {
            game.RunLoop();
        }
        game.Shutdown();
    }
}
