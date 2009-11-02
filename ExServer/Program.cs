using System.Windows.Forms;
using System.ServiceModel;

namespace ExServer
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
        }
    }
}

