﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Tcp.NET.Server.Models;

namespace Tcp.NET.Server.Managers
{
    public class TcpConnectionManagerAuth<T> : TcpConnectionManager
    {
        protected ConcurrentDictionary<T, IUserConnections<T>> _userConnections =
            new ConcurrentDictionary<T, IUserConnections<T>>();

        public IUserConnections<T> GetIdentity(T userId)
        {
            return _userConnections.TryGetValue(userId, out var clientAuthorized) ? clientAuthorized : (default);
        }
        public IUserConnections<T> GetIdentity(IConnectionServer connection)
        {
            return _userConnections.Any(p => p.Value.Connections.Any(t => t != null && t.Client.GetHashCode() == connection.Client.GetHashCode()))
                ? _userConnections.Values.FirstOrDefault(s => s.Connections.Any(t => t != null && t.Client.GetHashCode() == connection.Client.GetHashCode()))
                : (default);
        }
        public IUserConnections<T>[] GetAllIdentities()
        {
            return _userConnections.Values.Where(s => s != null).ToArray();
        }

        public IUserConnections<T> AddUserConnection(T userId, IConnectionServer connection)
        {
            if (!_userConnections.TryGetValue(userId, out var instance))
            {
                instance = new UserConnections<T>
                {
                    UserId = userId,
                    Connections = new List<IConnectionServer>()
                };
                _userConnections.TryAdd(userId, instance);
            }

            if (!instance.Connections.Any(s => s != null && s.Client.GetHashCode() == instance.GetHashCode()))
            {
                instance.Connections.Add(connection);
                return instance;
            }

            return null;
        }
        public void RemoveUserConnection(IConnectionServer connection)
        {
            var userConnection = _userConnections.Values.FirstOrDefault(s => s.Connections.Any(t => t != null && t.Client.GetHashCode() == connection.Client.GetHashCode()));

            if (userConnection != null)
            {
                var instance = userConnection.Connections.FirstOrDefault(s => s != null && s.Client.GetHashCode() == connection.Client.GetHashCode());

                if (instance != null)
                {
                    userConnection.Connections.Remove(instance);

                    if (!userConnection.Connections.Where(s => s != null).Any())
                    {
                        _userConnections.TryRemove(userConnection.UserId, out userConnection);
                    }
                }
            }
        }

        public bool IsConnectionAuthorized(IConnectionServer connection)
        {
            return _userConnections.Values.Any(s => s.Connections.Any(t => t != null && t.Client.GetHashCode() == connection.Client.GetHashCode()));
        }
        public bool IsUserConnected(T userId)
        {
            return _userConnections.ContainsKey(userId);
        }
    }
}
