using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using System.Web.Routing;
using Orchard;
using Orchard.Data;


namespace JabbR
{
    public class WorkContextScopeMiddleware : OwinMiddleware
    {
        public Log4netTraceListener Logger { get; set; }
        public WorkContextScopeMiddleware(OwinMiddleware next)
            : base(next)
        {
            Logger = new Log4netTraceListener();
        }

        public override Task Invoke(IOwinContext context)
        {
            var requestContext = (RequestContext)context.Request.Environment["System.Web.Routing.RequestContext"];
            var workContextAccessor = (IWorkContextAccessor)requestContext.RouteData.DataTokens["IWorkContextAccessor"];
            var scope = workContextAccessor.GetContext(requestContext.HttpContext);
            try
            {

                if (scope != null)
                {
                    var tm = scope.Resolve<ITransactionManager>();
                    tm.Demand();

                    return Next.Invoke(context).ContinueWith(t =>
                    {
                        if (t.IsFaulted || t.IsCanceled || t.Exception != null)
                        {
                            tm.Cancel();
                        }
                    });
                }

                return Next.Invoke(context);
            }
            catch (Exception exc)
            {
                Logger.WriteLine("Proligence.SignalR.WorkContextScopeMiddleware Error:" + exc.Message);
                throw exc;
            }
        }
    }

}
