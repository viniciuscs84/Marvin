using System;

namespace Marvin.Commons.Utilities
{
    /// <summary>
    /// Application logger handler based in log4net
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// log4net object
        /// </summary>
        private static log4net.ILog _log;

        /// <summary>
        /// Static constructor
        /// </summary>
        static Logger()
        {
            if (!log4net.LogManager.GetRepository().Configured)
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));
            _log = log4net.LogManager.GetLogger(typeof(Logger));
        }

        /// <summary>
        /// Register log error message
        /// </summary>
        /// <param name="msg">Error message</param>
        public static void Error(object msg)
        {
            _log.Error(msg);
        }

        /// <summary>
        /// Register log error message with exception
        /// </summary>
        /// <param name="msg">Error message</param>
        /// <param name="ex">Exception</param>
        public static void Error(object msg, Exception ex)
        {
            _log.Error(msg, ex);
        }

        /// <summary>
        /// Register log error with exception
        /// </summary>
        /// <param name="ex">Exception</param>
        public static void Error(Exception ex)
        {
            _log.Error(ex.Message, ex);
        }

        /// <summary>
        /// Register log info message.
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(object msg)
        {
            _log.Info(msg);
        }
    }
}