using System;
using System.Diagnostics;

namespace Axtox.IoT.Common.System.Logging
{
    public class ConsoleLogger : ILogger
    {
        private string _name;
        private LogLevel _logLevel;

        public LogLevel LogLevel => _logLevel;

        public ConsoleLogger(string name, LogLevel logLevel)
        {
            _name = name;
            _logLevel = logLevel;
        }

        public void LogInfo(string message)
        {
            Log(LogLevel.Info, $"[INFO]: {message}");
        }

        public void LogWarning(string message)
        {
            Log(LogLevel.Warning, $"[WARNING]: {message}");
        }

        public void LogError(string message)
        {
            Log(LogLevel.Error, $"[ERROR]: {message}");
        }

        public void LogDebug(string message)
        {
            Log(LogLevel.Debug, $"[DEBUG]: {message}");
        }

        public void LogCritical(string message)
        {
            Log(LogLevel.Critical, $"[CRITICAL]: {message}");
        }

        public void Log(LogLevel level, string message)
        {
            if (!IsEnabled(level))
                return;

            var sinceLaunched = DateTime.UtcNow - DateTime.UnixEpoch;
            Debug.WriteLine($"[{sinceLaunched}][{_name}]{message}");
        }

        public bool IsEnabled(LogLevel level)
        {
            return _logLevel <= level && level != LogLevel.None;
        }
    }
}
