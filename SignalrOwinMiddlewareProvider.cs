using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Configuration;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Transports;
using Orchard.Logging;
using Orchard.Owin;
using Owin;
using JabbR.Hubs;
using JabbR.Services;


namespace JabbR
{
    public class SignalrOwinMiddlewareProvider : IOwinMiddlewareProvider
    {
        private readonly IDependencyResolver _dependencyResolver;
        private readonly HubConfiguration _hubConfiguration;
        private readonly IJabbrConfiguration _jabbrConfiguration;
        public ILogger Logger { get; set; }

        public SignalrOwinMiddlewareProvider(IDependencyResolver dependencyResolver, IJabbrConfiguration jabbrConfiguration)
        {
            _dependencyResolver = dependencyResolver;
            _jabbrConfiguration = jabbrConfiguration;
            Logger = NullLogger.Instance;
            _hubConfiguration = new HubConfiguration
            {
                EnableJavaScriptProxies = true,
                EnableDetailedErrors = true,
                Resolver = _dependencyResolver
            };
        }
        public IEnumerable<OwinMiddlewareRegistration> GetOwinMiddlewares()
        {
            yield return new OwinMiddlewareRegistration
            {
                Configure = app =>
                {
                    SetupSignalR(app);
                },
                Priority = "-99"
            };
        }
        private void SetupSignalR(/*I_jabbrConfigurationuration _jabbrConfiguration, IKernel kernel,*/ IAppBuilder app)
        {
            Logger.Debug("JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: Entering");
            try
            {
                var resolver = GlobalHost.DependencyResolver;
                var connectionManager = resolver.Resolve<IConnectionManager>();
                var heartbeat = resolver.Resolve<ITransportHeartbeat>();
                var hubPipeline = resolver.Resolve<IHubPipeline>();
                var configuration = resolver.Resolve<IConfigurationManager>();

                //app.Map("/signalr", map =>
                //{
                //    map.Use<WorkContextScopeMiddleware>();
                //    map.MapSignalR(_hubConfiguration);
                //});
                // Enable service bus scale out
                Logger.Debug("JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: Check Enable service bus scale out");

                if (!String.IsNullOrEmpty(_jabbrConfiguration.ServiceBusConnectionString) &&
                    !String.IsNullOrEmpty(_jabbrConfiguration.ServiceBusTopicPrefix))
                {
                    var sbConfig = new ServiceBusScaleoutConfiguration(_jabbrConfiguration.ServiceBusConnectionString,
                                                                       _jabbrConfiguration.ServiceBusTopicPrefix)
                    {
                        TopicCount = 5
                    };

                    Logger.Debug("JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: Enabling service bus scale out");
                    resolver.UseServiceBus(sbConfig);
                }

                Logger.Debug("JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: Check Enable sql-server scale out");
                if (_jabbrConfiguration.ScaleOutSqlServer)
                {
                    Logger.Debug("JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: Enabling sql-server scale out");
                    resolver.UseSqlServer(_jabbrConfiguration.SqlConnectionString.ConnectionString);
                }

                Logger.Debug("JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: Adding LoggingHubPipelineModule");
                hubPipeline.AddModule(new LoggingHubPipelineModule(Logger));

                app.MapSignalR(_hubConfiguration);

                var monitor = new PresenceMonitor(Logger, connectionManager, heartbeat, _dependencyResolver);
                Logger.Debug("JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: starting presence monitor");
                monitor.Start();
                Logger.Debug("JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: Completed");
            }
            catch (Exception exc)
            {
                Logger.Error(exc, "JabbR.SignalrOwinMiddlewareProvider.SetupSignalR: Exception --" + exc.Message);
            }

        }
    }
}
