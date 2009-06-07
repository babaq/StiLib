#region File Description
//-----------------------------------------------------------------------------
// SLLogger.cs
//
// StiLib Logging Service
// Copyright (c) Zhang Li. 2009-02-10.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
#endregion

namespace StiLib.Core
{
    /// <summary>
    /// StiLib Logging Service
    /// </summary>
    public class SLLogger
    {
        #region Fields

        string filename;
        string filepath;
        FileStream filestream;
        StreamWriter writer;
        StringBuilder message;

        #endregion

        #region Properties

        /// <summary>
        /// Log File Name
        /// </summary>
        public string FileName
        {
            get { return filename; }
            set { filename = value; }
        }

        /// <summary>
        /// Log File Path
        /// </summary>
        public string FilePath
        {
            get { return filepath; }
            set 
            { 
                filepath = value;
                if (filepath.LastIndexOf("\\") != (filepath.Length - 1))
                {
                    filepath += "\\";
                }
            }
        }

        #endregion


        /// <summary>
        /// Default Logger target to StiLib.log file in current directory
        /// </summary>
        public SLLogger() : this(Directory.GetCurrentDirectory(), "StiLib.log")
        {
        }

        /// <summary>
        /// Logger target to custom log file in current directory
        /// </summary>
        /// <param name="fname">Log File Name</param>
        public SLLogger(string fname) : this(Directory.GetCurrentDirectory(), fname)
        {
        }

        /// <summary>
        /// Logger target to custom log file in custom directory
        /// </summary>
        /// <param name="fpath"></param>
        /// <param name="fname"></param>
        public SLLogger(string fpath, string fname)
        {
            FilePath = fpath;
            filename = fname;

            filestream = new FileStream(filepath+filename, FileMode.Append, FileAccess.Write);
            writer = new StreamWriter(filestream, Encoding.Unicode);
            message = new StringBuilder();
        }


        /// <summary>
        /// Log Exception
        /// </summary>
        /// <param name="e"></param>
        public void Log(Exception e)
        {
            Log(e.Message);
        }

        /// <summary>
        /// Log message string
        /// </summary>
        /// <param name="info"></param>
        public void Log(string info)
        {
            try
            {
                message.Append(DateTime.Now.ToString()).Append("  ---  ").Append(info);
                writer.WriteLine(message.ToString());
                writer.Flush();
            }
            finally
            {
                message.Remove(0, message.Length);
            }
        }

    }
}
