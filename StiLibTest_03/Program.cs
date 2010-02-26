using System;

namespace StiLibTest_03
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MainGame game = new MainGame())
            {
                game.Run();
            }
        }
    }
}

