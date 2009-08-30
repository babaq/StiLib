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
            set
            {
                string newfilename = value;
                if (newfilename != filename)
                {
                    filename = newfilename;
                    ChangeLog();
                }
            }
        }

        /// <summary>
        /// Log File Path
        /// </summary>
        public string FilePath
        {
            get { return filepath; }
            set
            {
                string newfilepath = value;
                if (newfilepath.LastIndexOf("\\") != (newfilepath.Length - 1))
                {
                    newfilepath += "\\";
                }
                if (newfilepath != filepath)
                {
                    filepath = newfilepath;
                    ChangeLog();
                }
            }
        }

        #endregion


        /// <summary>
        /// Init default Logger target to StiLib.log file in current directory
        /// </summary>
        public SLLogger()
            : this(Directory.GetCurrentDirectory(), "StiLib.log")
        {
        }

        /// <summary>
        /// Init Logger target to custom log file in current directory
        /// </summary>
        /// <param name="fname">Log File Name</param>
        public SLLogger(string fname)
            : this(Directory.GetCurrentDirectory(), fname)
        {
        }

        /// <summary>
        /// Init Logger target to custom log file in custom directory
        /// </summary>
        /// <param name="fpath"></param>
        /// <param name="fname"></param>
        public SLLogger(string fpath, string fname)
        {
            filename = fname;
            FilePath = fpath;
            message = new StringBuilder();
        }


        /// <summary>
        /// Change Internal Log Filestream and Streamwriter
        /// </summary>
        private void ChangeLog()
        {
            if (filestream != null)
            {
                filestream.Dispose();
            }
            if (writer != null)
            {
                writer.Dispose();
            }
            filestream = new FileStream(filepath + filename, FileMode.Append, FileAccess.Write);
            writer = new StreamWriter(filestream, Encoding.Unicode);
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
            catch (Exception e)
            {
                SLConstant.ShowException(e);
            }
            finally
            {
                message.Remove(0, message.Length);
            }
        }

    }
}
