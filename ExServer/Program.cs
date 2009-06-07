using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using StiLib.Core;

namespace ExServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(ExService));
            host.Open();

            Console.WriteLine("StiLib Experiment Server Started !\n\n");
            Console.ReadLine();

            host.Close();
        }
    }
}
