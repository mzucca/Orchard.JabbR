using Orchard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.Infrastructure
{
    public interface IRealtimeLogger : IDependency
    {
        void Log(LogType type, string message);
    }
}