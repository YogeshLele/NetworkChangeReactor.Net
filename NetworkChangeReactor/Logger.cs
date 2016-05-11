using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Config;
using log4net;

namespace NetworkChangeReactor
{
    public static class Logger
    {
        private static ILog _logger;

        static Logger()
        {
            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + @"\log4net.config";

            if (!File.Exists(filePath))
            {
                filePath = "..\\Log4net.config";
            }

            if (!File.Exists(filePath))
            {
                Logger.Debug("Failed to initialize Logger");
            }

            _logger = LogManager.GetLogger(System.Reflection.Assembly.GetCallingAssembly().ToString());
            XmlConfigurator.ConfigureAndWatch(new FileInfo(filePath));
        }

        public static void Debug(string message)
        {
            if (_logger != null)
                _logger.Debug(message);
        }

        public static void Debug(string message, System.Exception exception)
        {
            if (_logger != null)
                _logger.Debug(message, exception);
        }

        public static void Info(string message)
        {
            if (_logger != null)
                _logger.Info(message);
        }

        public static void Info(string message, System.Exception exception)
        {
            if (_logger != null)
                _logger.Info(message, exception);
        }

        public static void Warn(string message)
        {
            if (_logger != null)
                _logger.Warn(message);
        }

        public static void Warn(string message, System.Exception exception)
        {
            if (_logger != null)
                _logger.Warn(message, exception);
        }

        public static void Error(string message)
        {
            if (_logger != null)
                _logger.Error(message);
        }

        public static void Error(string message, System.Exception exception)
        {
            if (_logger != null)
                _logger.Error(message, exception);
        }
    }
}
