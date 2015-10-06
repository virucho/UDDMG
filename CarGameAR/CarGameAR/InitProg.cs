using System;

namespace CarGameAR
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game_Main game = new Game_Main())
            {
                game.Run();
            }
        }
    }
}

