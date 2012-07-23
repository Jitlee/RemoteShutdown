using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemoteShutdown.Net;
using System.IO;

namespace RemoteShutdown.Utilities
{
    public class MyLogger : ILogger
    {
        #region Fields

        private string _channel;

        private LoggerLevel _level;

        #endregion

        #region Public Memebers
        public MyLogger(string channel)
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

        private void WriteTrance(string message, object[] args)
        {
            if (_level != LoggerLevel.NoLog
                && _level != LoggerLevel.Error
                && _level != LoggerLevel.Debug)
            {
                LogMessage(message, args);
            }
        }

        private void SetChannel(string channel)
        {
            _channel = channel;
        }

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

        private void WriteError(string message, object[] args)
        {
            if (_level != LoggerLevel.NoLog)
            {
                LogMessage(message, args);
            }
        }

        private void WriteDebug(string message)
        {
            if (_level != LoggerLevel.NoLog
                && _level != LoggerLevel.Error)
            {
                LogMessage(message);
            }
        }

        private void WriteDebug(string message, object[] args)
        {
            if (_level != LoggerLevel.NoLog
                && _level != LoggerLevel.Error)
            {
                LogMessage(message, args);
            }
        }

        private void WriteTrance(string message)
        {
            if (_level != LoggerLevel.NoLog
                && _level != LoggerLevel.Error
                && _level != LoggerLevel.Debug)
            {
                LogMessage(message);
            }
        }

        private void LogMessage(string message)
        {
            WriteLog(string.Format("<{0}> <{1}> <{2}> {3}",
                DateTime.Now.ToString("HH:mm:ss"),
                _channel,
                _level,
                message));
        }

        private void LogMessage(string message, params object[] args)
        {
            WriteLog(string.Format(
                string.Format("<{0}> <{1}> <{2}> {3}",
                   DateTime.Now.ToString("HH:mm:ss"),
                   _channel,
                   _level,
                   message), args));
        }

        private void WriteLog(string msg)
        {
            try
            {
                var path = Path.Combine(Environment.CurrentDirectory, DateTime.Now.ToString("yyyy-MM-dd.log"));
                File.AppendAllText(msg, path, Encoding.Default);
            }
            catch { }
        }

        #endregion
    }
}
