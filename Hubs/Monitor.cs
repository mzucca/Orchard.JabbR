using JabbR.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace JabbR.Hubs
{
    [AuthorizeAll]
    public class Monitor : Hub
    {
    }
}