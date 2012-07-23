using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RemoteShutdown.Net
{
    public static class LoggerFactory
    {
        private static readonly Dictionary<string, ILogger> _loggerDictionary;
        private static LoggerLevel _level = LoggerLevel.Debug;
        private static object _thisObject;
        private static Type _type;

        public static LoggerLevel Level { get { return _level; } }

        static LoggerFactory()
        {
            _loggerDictionary = new Dictionary<string, ILogger>(new StringEqualityComparer());
            _level = LoggerLevel.Debug;
            _thisObject = new object();
        }

        public static ILogger GetLogger(string channel)
        {
            if (!_loggerDictionary.ContainsKey(channel))
            {
                var logger = CreateLogger(channel);
                logger.SetLoggerLevel(_level);
                lock (_thisObject)
                {
                    _loggerDictionary.Add(channel, logger);
                }
                return logger;
            }
            return _loggerDictionary[channel];
        }

        private static ILogger CreateLogger(string channel)
        {
            if (null != _type)
            {
                try
                {
                    return (ILogger)Activator.CreateInstance(_type, channel);
                }
                catch
                {
                    return new Logger(channel);
                }
            }
            return new Logger(channel);
        }

        public static void SetLoggerLevel(LoggerLevel level)
        {
            _level = level;
            lock (_thisObject)
            {
                if (_loggerDictionary.Count > 0)
                {
                    foreach (var dict in _loggerDictionary)
                    {
                        dict.Value.SetLoggerLevel(level);
                    }
                }
            }
        }

        public static void SetLoggerInstance(Type type) 
        {
            if (!type.IsClass)
            {
                throw new ArgumentException("Type must be a class.");
            }
            if (!typeof(ILogger).IsAssignableFrom(type))
            {
                throw new ArgumentException("Type must be from ILogger interfaces inherited.");
            }
            if (null != type.GetConstructor(new Type[] { typeof(string) }))
            {
                _type = type;
                _loggerDictionary.Clear();
                return;
            }
            throw new ArgumentException("Type must have a string argument constructor.");
        }
    }
}
