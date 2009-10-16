using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ServiceModel;

namespace StiLibTest_02
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var service = new Server();
            ServiceHost host = new ServiceHost(service);
            host.Open();

            Application.Run(service);

            host.Close();


            //using (Game1 game = new Game1())
            //{
            //    game.Run();
            //}
        }
    }
}

