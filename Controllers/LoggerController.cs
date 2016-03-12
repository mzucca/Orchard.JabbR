using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JabbR.Controllers
{
    public class LoggerController : Controller
    {
        public LoggerController()
        {
            Logger = NullLogger.Instance;
        }
        ILogger Logger { get; set; }

        [HttpPost]
        public void Log(string message)
        {
            if(string.IsNullOrEmpty(message))
                return;

            var level = Request.Params["level[value]"];
            if (string.IsNullOrEmpty(level))
            {
                Logger.Information(message);
            }
            else
            {
                switch (level)
                {
                    case "1":
                        Logger.Debug(message);
                        break;
                    case "2":
                        Logger.Information(message);
                        break;
                    case "4":
                        Logger.Warning(message);
                        break;
                    case "8":
                        Logger.Error(message);
                        break;
                }
            }
        }
    }
}