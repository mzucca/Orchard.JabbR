using System;

namespace JabbR.Infrastructure
{
    public static class LoggingExtensions
    {
        public static void Error(this IRealtimeLogger logger, string message, params object[] args)
        {
            logger.Log(LogType.Error, String.Format(message, args));
        }
        public static void Information(this IRealtimeLogger logger, string message, params object[] args)
        {
            logger.Log(LogType.Message, String.Format(message, args));
        }
        public static void Debug(this IRealtimeLogger logger, string message, params object[] args)
        {
            logger.Log(LogType.Debug, String.Format(message, args));
        }

        public static void Log(this IRealtimeLogger logger, Exception exception)
        {
            logger.Log(LogType.Error, "Exception:\r\n" + exception.ToString());
        }

        public static void LogError(this IRealtimeLogger logger, string message, params object[] args)
        {
            logger.Log(LogType.Error, String.Format(message, args));
        }

        public static void Log(this IRealtimeLogger logger, string message, params object[] args)
        {
            logger.Log(LogType.Message, String.Format(message, args));
        }
    }
}