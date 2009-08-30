using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace ContentBuild
{
    /// <summary>
    /// MSBuild ILogger interface records content build errors
    /// </summary>
    class ErrorLogger : ILogger
    {
        List<string> errors = new List<string>();
        /// <summary>
        /// Gets a list of all the errors that have been logged.
        /// </summary>
        public List<string> Errors
        {
            get { return errors; }
        }


        /// <summary>
        /// Initializes the custom logger, hooking the ErrorRaised notification event.
        /// </summary>
        public void Initialize(IEventSource eventSource)
        {
            if (eventSource != null)
            {
                eventSource.ErrorRaised += ErrorRaised;
            }
        }

        /// <summary>
        /// Shuts down the custom logger.
        /// </summary>
        public void Shutdown()
        {
        }

        /// <summary>
        /// Handles error notification events by storing the error message string.
        /// </summary>
        void ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            errors.Add(e.Message);
        }


        #region ILogger Members

        string parameters;
        /// <summary>
        /// Implement the ILogger.Parameters property.
        /// </summary>
        string ILogger.Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        LoggerVerbosity verbosity = LoggerVerbosity.Normal;
        /// <summary>
        /// Implement the ILogger.Verbosity property.
        /// </summary>
        LoggerVerbosity ILogger.Verbosity
        {
            get { return verbosity; }
            set { verbosity = value; }
        }

        #endregion

    }
}
