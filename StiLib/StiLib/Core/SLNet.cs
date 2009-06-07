#region File Description
//-----------------------------------------------------------------------------
// SLNet.cs
//
// StiLib Network Service.
// Copyright (c) Zhang Li. 2009-04-30.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Diagnostics;
using System.Reflection;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// Experiment Service Contract
    /// </summary>
    [ServiceContract]
    public interface IExService
    {
        /// <summary>
        /// Run an Experiment
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        [OperationContract]
        string Run(string ex);
        /// <summary>
        /// Run an Experiment Script 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        [OperationContract]
        string RunScript(string ex, string script);
        /// <summary>
        /// Get Experiment Names
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string[] GetEx();
    }

    /// <summary>
    /// Experiment Service
    /// </summary>
    public class ExService : IExService
    {
        AssemblySettings config;

        /// <summary>
        /// Init Experiment Service
        /// </summary>
        public ExService()
        {
            config = new AssemblySettings(Assembly.GetAssembly(typeof(AssemblySettings)));
        }


        /// <summary>
        /// Run Experiment
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public string Run(string ex)
        {
            string ext = ex.Substring(ex.LastIndexOf(".") + 1);

            try
            {
                switch (ext)
                {
                    case "exe":
                        Process.Start(config["stilib"] + ex);
                        break;
                    case "fsx":
                        Process.Start(config["fsi"], config["stilib"] + ex);
                        break;
                    case "py":
                        Process.Start(config["ipy"], config["stilib"] + ex);
                        break;
                }
                Console.WriteLine(ex + " has invoked !");
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// Run Experiment Script
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public string RunScript(string ex, string script)
        {
            StreamWriter writer = new StreamWriter(config["stilib"] + ex);
            writer.Write(script);
            writer.Flush();
            writer.Close();
            return Run(ex);
        }

        /// <summary>
        /// Get Experiment List
        /// </summary>
        /// <returns></returns>
        public string[] GetEx()
        {
            var fsx = Directory.GetFiles(config["stilib"], "*.fsx");
            var py = Directory.GetFiles(config["stilib"], "*.py");
            var exe = Directory.GetFiles(config["stilib"], "*.exe");

            var temp = fsx.Concat<string>(py).Concat<string>(exe);
            string[] ex = temp.ToArray<string>();
            for (int i = 0; i < ex.Length; i++)
            {
                ex[i] = Path.GetFileName(ex[i]);
            }
            return ex;
        }

    }

}
