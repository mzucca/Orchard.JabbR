using log4net;

namespace JabbR
{
    public class Log4netTraceListener : System.Diagnostics.TraceListener
    {
        public ILog Logger { get; set; }
        public Log4netTraceListener()
        {
            Logger = log4net.LogManager.GetLogger("JabbR");

        }
        public override void Write(string message)
        {
            Logger.Debug(message);
        }

        public override void WriteLine(string message)
        {
            Logger.Debug(message);
        }
    }
}
