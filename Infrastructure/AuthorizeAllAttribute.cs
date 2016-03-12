using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;

namespace JabbR.Infrastructure
{
    public class AuthorizeAllAttribute : Attribute, IAuthorizeHubConnection, IAuthorizeHubMethodInvocation
    {

        public bool AuthorizeHubConnection(HubDescriptor hubDescriptor, Microsoft.AspNet.SignalR.IRequest request)
        {
            return true;
        }

        public bool AuthorizeHubMethodInvocation(IHubIncomingInvokerContext hubIncomingInvokerContext, bool appliesToMethod)
        {
            return true;
        }
    }
}