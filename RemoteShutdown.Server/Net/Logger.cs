using System;
using System.Diagnostics;

namespace RemoteShutdown.Net
{
    internal class Logger : ILogger
    {
        #region Fields
        
        private string _channel;

        private LoggerLevel _level;

        #endregion

        #region Public Memebers
        public Logger(string channel)
        {
            SetChannel(channel);
        }

        public void SetLoggerLevel(LoggerLevel level)
        {
            SetLevel(level);
        }

        public void Error(string message)
        {
            WriteError(message);
        }

        public void Error(string message, params object[] args)
        {
            WriteError(message, args);
        }

        public void Debug(string message)
        {
            WriteDebug(message);
        }

        public void Debug(string message, params object[] args)
        {
            WriteDebug(message, args);
        }

        public void Trance(string message)
        {
            WriteTrance(message);
        }

        public void Trance(string message, params object[] args)
        {
            WriteTrance(message, args);
        }

        #endregion

        #region Private Memebers

        [Conditional("DEBUG")]
        private void WriteTrance(string message, object[] args)
        {
            if (_level != LoggerLevel.NoLog
                && _level != LoggerLevel.Error
                && _level != LoggerLevel.Debug)
            {
                LogMessage(message, args);
            }
        }

        [Conditional("DEBUG")]
        private void SetChannel(string channel)
        {
            _channel = channel;
        }

        [Conditional("DEBUG")]
        private void SetLevel(LoggerLevel level)
        {
            _level = level;
        }

        private void WriteError(string message)
        {
            if (_level != LoggerLevel.NoLog)
            {
                LogMessage(message);
            }
        }

        [Conditional("DEBUG")]
        private void WriteError(string message, object[] args)
        {
            if (_level != LoggerLevel.NoLog)
            {
                LogMessage(message, args);
            }
        }

        [Conditional("DEBUG")]
        private void WriteDebug(string message)
        {
            if (_level != LoggerLevel.NoLog
                && _level != LoggerLevel.Error)
            {
                LogMessage(message);
            }
        }

        [Conditional("DEBUG")]
        private void WriteDebug(string message, object[] args)
        {
            if (_level != LoggerLevel.NoLog
                && _level != LoggerLevel.Error)
            {
                LogMessage(message, args);
            }
        }

        [Conditional("DEBUG")]
        private void WriteTrance(string message)
        {
            if (_level != LoggerLevel.NoLog
                && _level != LoggerLevel.Error
                && _level != LoggerLevel.Debug)
            {
                LogMessage(message);
            }
        }


        [Conditional("DEBUG")]
        private void LogMessage(string message)
        {
            Console.WriteLine(string.Format("<{0}> <{1}> <{2}> {3}",
                DateTime.Now.ToString("mm:ss"),
                _channel,
                _level,
                message));
        }

        [Conditional("DEBUG")]
        private void LogMessage(string message, params object[] args)
        {
            Console.WriteLine(string.Format(
                string.Format("<{0}> <{1}> <{2}> {3}",
                   DateTime.Now.ToString("mm:ss"),
                   _channel,
                   _level,
                   message), args));
        }

        #endregion
    }
}
