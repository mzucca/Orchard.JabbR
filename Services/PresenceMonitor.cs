﻿using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.ViewModels;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hosting;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Transports;
using Newtonsoft.Json;
using Orchard.Logging;

namespace JabbR.Services
{
    public class PresenceMonitor
    {
        private volatile bool _running;
        private Timer _timer;
        private readonly TimeSpan _presenceCheckInterval = TimeSpan.FromMinutes(1);

        //private readonly IKernel _kernel;
        private readonly IHubContext _hubContext;
        private readonly ITransportHeartbeat _heartbeat;
        private readonly IDependencyResolver _resolver;
        private Orchard.Logging.ILogger _logger;

        public PresenceMonitor(
            Orchard.Logging.ILogger logger,
            IConnectionManager connectionManager,
            ITransportHeartbeat heartbeat,
            IDependencyResolver resolver)
        {
            _resolver = resolver;
            _hubContext = connectionManager.GetHubContext<JabbR.Hubs.Chat>();
            _heartbeat = heartbeat;
            _logger = logger;

        }
        public void Start()
        {
            // Start the timer
            _timer = new Timer(_ =>
            {
                Check();
            },
            null,
            TimeSpan.Zero,
            _presenceCheckInterval);
        }

        private void Check()
        {
            if (_running)
            {
                return;
            }

            _running = true;

            try
            {
                _logger.Information("Checking user presence");
                using (var repo = _resolver.Resolve<IJabbrRepository>())
                {
                    // Update the connection presence
                    UpdatePresence(repo);

                    // Remove zombie connections
                    RemoveZombies(repo);

                    // Remove users with no connections
                    RemoveOfflineUsers(repo);

                    // Check the user status
                    CheckUserStatus(repo);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex,"PresenceMonitor fatal error checking");
            }
            finally
            {
                _running = false;
            }
        }

        private void UpdatePresence(IJabbrRepository repo)
        {
            // Get all connections on this node and update the activity
            foreach (var connection in _heartbeat.GetConnections())
            {
                if (!connection.IsAlive)
                {
                    continue;
                }

                ChatClient client = repo.GetClientById(connection.ConnectionId);

                if (client != null)
                {
                    client.LastActivity = DateTimeOffset.UtcNow;
                }
                else
                {
                    EnsureClientConnected(repo, connection);
                }
            }

            repo.CommitChanges();
        }

        // This is an uber hack to make sure the db is in sync with SignalR
        private void EnsureClientConnected(IJabbrRepository repo, ITrackingConnection connection)
        {
            var contextField = connection.GetType().GetField("_context",
                                          BindingFlags.NonPublic | BindingFlags.Instance);
            if (contextField == null)
            {
                return;
            }

            var context = contextField.GetValue(connection) as HostContext;

            if (context == null)
            {
                return;
            }

            string connectionData = context.Request.QueryString["connectionData"];

            if (String.IsNullOrEmpty(connectionData))
            {
                return;
            }

            var hubs = JsonConvert.DeserializeObject<HubConnectionData[]>(connectionData);

            if (hubs.Length != 1)
            {
                return;
            }

            // We only care about the chat hub
            if (!hubs[0].Name.Equals("chat", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _logger.Error("Connection {0} exists but isn't tracked.", connection.ConnectionId);

            string userId = context.Request.User.GetUserId();

            ChatUser user = repo.GetUserById(userId);
            if (user == null)
            {
                _logger.Error("Unable to find user with id {0}", userId);
                return;
            }

            var client = new ChatClient
            {
                Id = connection.ConnectionId,
                User = user,
                UserAgent = context.Request.Headers["User-Agent"],
                LastActivity = DateTimeOffset.UtcNow,
                LastClientActivity = user.LastActivity
            };

            repo.Add(client);
            repo.CommitChanges();
        }

        private void RemoveZombies(IJabbrRepository repo)
        {
            // Remove all zombie clients 
            var zombies = repo.Clients.Where(c =>
                SqlFunctions.DateDiff("mi", c.LastActivity, DateTimeOffset.UtcNow) > 3);

            // We're doing to list since there's no MARS support on azure
            foreach (var client in zombies.ToList())
            {
                _logger.Information("Removed zombie connection {0}", client.Id);

                repo.Remove(client);
            }
        }

        private void RemoveOfflineUsers(IJabbrRepository repo)
        {
            var offlineUsers = new List<ChatUser>();
            IQueryable<ChatUser> users = repo.GetOnlineUsers();

            foreach (var user in users.ToList())
            {
                if (user.ConnectedClients.Count == 0)
                {
                    _logger.Information("{0} has no clients. Marking as offline", user.Name);

                    // Fix users that are marked as inactive but have no clients
                    user.Status = (int)UserStatus.Offline;
                    offlineUsers.Add(user);
                }
            }

            if (offlineUsers.Count > 0)
            {
                PerformRoomAction(offlineUsers, async roomGroup =>
                {
                    foreach (var user in roomGroup.Users)
                    {
                        await _hubContext.Clients.Group(roomGroup.Room.Name).leave(user, roomGroup.Room.Name);
                    }
                });

                repo.CommitChanges();
            }
        }

        private void CheckUserStatus(IJabbrRepository repo)
        {
            var inactiveUsers = new List<ChatUser>();

            IQueryable<ChatUser> users = repo.GetOnlineUsers().Where(u =>
                SqlFunctions.DateDiff("mi", u.LastActivity, DateTime.UtcNow) > 5);

            foreach (var user in users.ToList())
            {
                user.Status = (int)UserStatus.Inactive;
                inactiveUsers.Add(user);
            }

            if (inactiveUsers.Count > 0)
            {
                PerformRoomAction(inactiveUsers, async roomGroup =>
                {
                    await _hubContext.Clients.Group(roomGroup.Room.Name).markInactive(roomGroup.Users);
                });

                repo.CommitChanges();
            }
        }

        private static async void PerformRoomAction(List<ChatUser> users, Func<RoomGroup, Task> callback)
        {
            var roomGroups = from u in users
                             from r in u.Rooms
                             select new { User = u, Room = r } into tuple
                             group tuple by tuple.Room into g
                             select new RoomGroup
                             {
                                 Room = g.Key,
                                 Users = g.Select(t => new UserViewModel(t.User))
                             };

            foreach (var roomGroup in roomGroups)
            {
                try
                {
                    await callback(roomGroup);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error occurred: " + ex);
                }
            }
        }

        private class RoomGroup
        {
            public ChatRoom Room { get; set; }
            public IEnumerable<UserViewModel> Users { get; set; }
        }

        private class HubConnectionData
        {
            public string Name { get; set; }
        }
    }
}