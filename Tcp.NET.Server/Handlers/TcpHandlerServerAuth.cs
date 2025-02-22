﻿using PHS.Networking.Enums;
using PHS.Networking.Events.Args;
using PHS.Networking.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using Tcp.NET.Core.Models;
using Tcp.NET.Server.Events.Args;
using Tcp.NET.Server.Models;

namespace Tcp.NET.Server.Handlers
{
    public class TcpHandlerServerAuth<T> : 
        TcpHandlerServerAuthBase<
            TcpConnectionServerAuthEventArgs<T>,
            TcpMessageServerAuthEventArgs<T>,
            TcpErrorServerAuthEventArgs<T>,
            ParamsTcpServerAuth,
            TcpAuthorizeEventArgs<T>,
            IdentityTcpServer<T>,
            T>
    {
        public TcpHandlerServerAuth(ParamsTcpServerAuth parameters) : base(parameters)
        {
        }

        public TcpHandlerServerAuth(ParamsTcpServerAuth parameters, byte[] certificate, string certificatePassword) : base(parameters, certificate, certificatePassword)
        {
        }

        public override Task AuthorizeCallbackAsync(TcpAuthorizeBaseEventArgs<IdentityTcpServer<T>, T> args, CancellationToken cancellationToken)
        {
            FireEvent(this, new TcpConnectionServerAuthEventArgs<T>
            {
                ConnectionEventType = ConnectionEventType.Connected,
                Connection = args.Connection
            });

            return Task.CompletedTask;
        }

        protected override IdentityTcpServer<T> CreateConnection(ConnectionTcp connection)
        {
            return new IdentityTcpServer<T>
            {
                TcpClient = connection.TcpClient,
                ConnectionId = Guid.NewGuid().ToString()
            };
        }

        protected override TcpConnectionServerAuthEventArgs<T> CreateConnectionEventArgs(ConnectionEventArgs<IdentityTcpServer<T>> args)
        {
            return new TcpConnectionServerAuthEventArgs<T>
            {
                Connection = args.Connection,
                ConnectionEventType = args.ConnectionEventType
            };
        }

        protected override TcpErrorServerAuthEventArgs<T> CreateErrorEventArgs(ErrorEventArgs<IdentityTcpServer<T>> args)
        {
            return new TcpErrorServerAuthEventArgs<T>
            {
                Connection = args.Connection,
                Exception = args.Exception,
                Message = args.Message
            };
        }

        protected override TcpMessageServerAuthEventArgs<T> CreateMessageEventArgs(TcpMessageServerBaseEventArgs<IdentityTcpServer<T>> args)
        {
            return new TcpMessageServerAuthEventArgs<T>
            {
                Bytes = args.Bytes,
                Connection = args.Connection,
                Message = args.Message,
                MessageEventType = args.MessageEventType
            };
        }
        protected override void FireEvent(object sender, TcpMessageServerAuthEventArgs<T> args)
        {
            if (!args.Connection.IsAuthorized)
            {
                FireEvent(this, new TcpAuthorizeEventArgs<T>
                {
                    Connection = args.Connection,
                    Token = args.Message,
                });
            }
            else
            {
                base.FireEvent(sender, args);
            }
        }
        protected override void FireEvent(object sender, TcpConnectionServerAuthEventArgs<T> args)
        {
            if (args.Connection.IsAuthorized)
            {
                base.FireEvent(sender, args);
            }
        }

        protected override TcpAuthorizeEventArgs<T> CreateAuthorizeEventArgs(TcpAuthorizeBaseEventArgs<IdentityTcpServer<T>, T> args)
        {
            return new TcpAuthorizeEventArgs<T>
            {
                Connection = args.Connection,
                Token = args.Token
            };
        }
    }
}
