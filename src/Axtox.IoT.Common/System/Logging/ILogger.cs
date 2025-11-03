namespace Axtox.IoT.Common.System.Logging
{
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical,
        None
    }

    public interface ILogger
    {
        LogLevel LogLevel { get; }
        bool IsEnabled(LogLevel level);
        void LogDebug(string message);
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogCritical(string message);
        void Log(LogLevel level, string message);
    }
}
