using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperFishRemovalTool.Logging
{
    /// <summary>
    /// The severity of a log item
    /// </summary>
    public enum LogSeverity
    {
        Information = 0,
        Warning = 1,
        Error = 2,
        Critical = 3,
    }

    internal static class Logger
    {
        /// <summary>
        /// Adds a logger instance
        /// </summary>
        /// <param name="loggerInstance"></param>
        public static void AddLogger(ILogger loggerInstance)
        {
            Logger.LoggingInstances.Add(loggerInstance);
        }

        /// <summary>
        /// Enables logging.  Off by default
        /// </summary>
        public static bool IsLoggingEnabled { get; set; }


        /// <summary>
        /// Logs a message
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="message"></param>
        public static void Log(LogSeverity severity, string message)
        {
            if (IsLoggingEnabled && Logger.LoggingInstances.Any())
            {
                foreach (var instance in Logger.LoggingInstances)
                {
                    instance.Log(severity, message);
                }
            }
        }

        /// <summary>
        /// Logs a message with an exception
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        public static void Log(Exception ex, string message)
        {
            if (IsLoggingEnabled && Logger.LoggingInstances.Any())
            {
                foreach (var instance in Logger.LoggingInstances)
                {
                    instance.Log(ex, message + "\r\n" + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// Logs a formatted message
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Log(Exception ex, string format, params object[] args)
        {
            if (IsLoggingEnabled && Logger.LoggingInstances.Any())
            {
                foreach (var instance in Logger.LoggingInstances)
                {
                    instance.Log(ex, String.Format(format, args));
                }
            }
        }

        /// <summary>
        /// Logs a formatted message
        /// </summary>
        /// <param name="severity"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public static void Log(LogSeverity severity, string format, params object[] args)
        {
            if (IsLoggingEnabled && Logger.LoggingInstances.Any())
            {
                foreach (var instance in Logger.LoggingInstances)
                {
                    instance.Log(severity, String.Format(format, args));
                }
            }
        }

        private static IList<ILogger> LoggingInstances
        {
            get { return _loggingInstances ?? (_loggingInstances = new List<ILogger>()); }
        }

        private static IList<ILogger> _loggingInstances;
    }
}
