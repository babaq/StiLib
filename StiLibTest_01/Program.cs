using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace StiLibTest_01
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //using (Game1 game = new Game1())
            //{
            //    game.Run();
            //}
        }
    }
}

