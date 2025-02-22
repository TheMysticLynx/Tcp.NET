﻿using Tcp.NET.Client.Events.Args;
using Tcp.NET.Client.Handlers;
using Tcp.NET.Client.Models;
using Tcp.NET.Core.Models;

namespace Tcp.NET.Client
{
    public class TcpNETClient :
        TcpNETClientBase<
            TcpConnectionClientEventArgs,
            TcpMessageClientEventArgs,
            TcpErrorClientEventArgs,
            ParamsTcpClient,
            TcpClientHandler,
            ConnectionTcp>,
        ITcpNETClient
    {
        public TcpNETClient(ParamsTcpClient parameters) : base(parameters)
        {
        }

        protected override TcpClientHandler CreateTcpClientHandler()
        {
            return new TcpClientHandler(_parameters);
        }
    }
}
