using System;
using JabbR.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Orchard.Logging;

namespace JabbR.Infrastructure
{
    public class RealtimeLogger : IRealtimeLogger
    {
        private readonly IHubContext _logContext;
        public Orchard.Logging.ILogger OrchardLogger{get;set;}

        public RealtimeLogger(IConnectionManager connectionManager)
        {
            _logContext = connectionManager.GetHubContext<Monitor>();
            OrchardLogger = NullLogger.Instance;
        }

        public void Log(LogType type, string message)
        {
            string logMessage = String.Format("[{0}]: {1}", DateTime.UtcNow, message);

            switch (type)
            {
                case LogType.Message:
                    _logContext.Clients.All.logMessage(logMessage);
                    OrchardLogger.Information(message);
                    break;
                case LogType.Error:
                    _logContext.Clients.All.logError(logMessage);
                    OrchardLogger.Error(message);
                    break;
            }
        }
    }
}