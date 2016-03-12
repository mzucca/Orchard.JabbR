using Autofac;
using Microsoft.AspNet.SignalR;
using JabbR.ContentProviders.Core;
using JabbR.Models;
using JabbR.Services;

namespace JabbR
{
    public class ChatModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacDependencyResolver>().As<IDependencyResolver>().InstancePerMatchingLifetimeScope("shell");
            builder.RegisterType<JabbrContext>().As<JabbrContext>().InstancePerDependency();
            builder.RegisterType<ContentProviderProcessor>().As<ContentProviderProcessor>().SingleInstance();
            builder.RegisterType<PersistedRepository>().As<IJabbrRepository>().InstancePerDependency();
            builder.RegisterType<JabbR.Hubs.Chat>().As<JabbR.Hubs.Chat>().InstancePerDependency().ExternallyOwned();
        }
    }
}