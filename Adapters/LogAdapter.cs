using Microsoft.Extensions.Logging;
using System;

namespace MicroServices.Shared.Adapters
{
    /// <summary>
    /// Custom implememtation of ILogger
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogAdapter<T>
    {
        void LogCritical(string message, params object[] args);
        void LogDebug(string message, params object[] args);
        void LogError(Exception ex, string message);
        void LogError(string message, params object[] args);
        void LogInformation(string message, params object[] args);
        void LogWarning(string message, params object[] args);
    }

    public class LogAdapter<T> : ILogAdapter<T>
    {
        private readonly ILogger _logger;

        public LogAdapter(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void LogInformation(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            _logger?.LogWarning(message, args);
        }

        public void LogError(string message, params object[] args)
        {
            _logger?.LogError(message, args);
        }

        public void LogError(Exception ex, string message)
        {
            _logger?.LogError(ex, message);
        }

        public void LogCritical(string message, params object[] args)
        {
            _logger?.LogCritical(message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            _logger?.LogDebug(message, args);
        }
    }
}