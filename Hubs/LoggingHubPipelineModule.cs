using System;
using Microsoft.AspNet.SignalR.Hubs;
using Orchard.Logging;
using JabbR.Infrastructure;

namespace JabbR.Hubs
{
    public class LoggingHubPipelineModule : HubPipelineModule
    {
        private readonly Orchard.Logging.ILogger _logger;

        public LoggingHubPipelineModule(Orchard.Logging.ILogger logger)
        {
            _logger = logger;
        }


        public override Func<HubDescriptor, Microsoft.AspNet.SignalR.IRequest, bool> BuildAuthorizeConnect(Func<HubDescriptor, Microsoft.AspNet.SignalR.IRequest, bool> authorizeConnect)
        {
            return base.BuildAuthorizeConnect(authorizeConnect);
        }
        public override Func<IHub, System.Threading.Tasks.Task> BuildConnect(Func<IHub, System.Threading.Tasks.Task> connect)
        {
            return base.BuildConnect(connect);
        }
        public override Func<IHub, bool, System.Threading.Tasks.Task> BuildDisconnect(Func<IHub, bool, System.Threading.Tasks.Task> disconnect)
        {
            return base.BuildDisconnect(disconnect);
        }
        public override Func<IHubIncomingInvokerContext, System.Threading.Tasks.Task<object>> BuildIncoming(Func<IHubIncomingInvokerContext, System.Threading.Tasks.Task<object>> invoke)
        {
            return base.BuildIncoming(invoke);
        }
        public override Func<IHubOutgoingInvokerContext, System.Threading.Tasks.Task> BuildOutgoing(Func<IHubOutgoingInvokerContext, System.Threading.Tasks.Task> send)
        {
            return base.BuildOutgoing(send);
        }
        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext context)
        {
            _logger.Error(exceptionContext.Error, "{0}: Failure while invoking '{1}'.",
                context.Hub.Context.GetUserId(), context.MethodDescriptor.Name);
        }
        public override Func<IHub, System.Threading.Tasks.Task> BuildReconnect(Func<IHub, System.Threading.Tasks.Task> reconnect)
        {
            return base.BuildReconnect(reconnect);
        }
        public override Func<HubDescriptor,Microsoft.AspNet.SignalR.IRequest,System.Collections.Generic.IList<string>,System.Collections.Generic.IList<string>> BuildRejoiningGroups(Func<HubDescriptor,Microsoft.AspNet.SignalR.IRequest,System.Collections.Generic.IList<string>,System.Collections.Generic.IList<string>> rejoiningGroups)
        {
 	         return base.BuildRejoiningGroups(rejoiningGroups);
        }
        protected override void OnAfterConnect(IHub hub)
        {
            base.OnAfterConnect(hub);
        }
        protected override void OnAfterDisconnect(IHub hub, bool stopCalled)
        {
            base.OnAfterDisconnect(hub, stopCalled);
        }
        protected override object OnAfterIncoming(object result, IHubIncomingInvokerContext context)
        {
            return base.OnAfterIncoming(result, context);
        }
        protected override void OnAfterOutgoing(IHubOutgoingInvokerContext context)
        {
            base.OnAfterOutgoing(context);
        }
        protected override void OnAfterReconnect(IHub hub)
        {
            base.OnAfterReconnect(hub);
        }
        protected override bool OnBeforeAuthorizeConnect(HubDescriptor hubDescriptor, Microsoft.AspNet.SignalR.IRequest request)
        {
            return base.OnBeforeAuthorizeConnect(hubDescriptor, request);
        }
        protected override bool OnBeforeConnect(IHub hub)
        {
            return base.OnBeforeConnect(hub);
        }
        protected override bool OnBeforeDisconnect(IHub hub, bool stopCalled)
        {
            return base.OnBeforeDisconnect(hub, stopCalled);
        }
        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            return base.OnBeforeIncoming(context);
        }
        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            return base.OnBeforeOutgoing(context);
        }
        protected override bool OnBeforeReconnect(IHub hub)
        {
            return base.OnBeforeReconnect(hub);
        }
    }
}