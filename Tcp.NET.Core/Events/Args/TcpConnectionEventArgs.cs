﻿using PHS.Networking.Events.Args;
using Tcp.NET.Core.Models;

namespace Tcp.NET.Core.Events.Args
{
    public class TcpConnectionEventArgs<T> : ConnectionEventArgs where T : IConnection
    {
        public T Connection { get; set; }
    }
}
