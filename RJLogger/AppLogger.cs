using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJLogger
{
    public class AppLogger
    {
        private readonly ILogger _logger;
        private static AppLogger _defaultLogger;

        private AppLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public static AppLogger GetLogger()
        {
            if (_defaultLogger == null)
                _defaultLogger = new AppLogger();

            return _defaultLogger;
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(exception, message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(exception, message);
        }
    }
}
