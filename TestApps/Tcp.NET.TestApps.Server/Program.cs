﻿using PHS.Networking.Server.Events.Args;
using System;
using System.Threading.Tasks;
using Tcp.NET.Server;
using Tcp.NET.Server.Models;
using Tcp.NET.Server.Events.Args;
using PHS.Networking.Enums;

namespace Tcp.NET.TestApps.Server
{
    class Program
    {
        private static ITcpNETServerAuth<Guid> _authServer;

        static void Main(string[] args)
        {
            _authServer = new TcpNETServerAuth<Guid>(new ParamsTcpServerAuth
            {
                ConnectionSuccessString = "Connected Successfully",
                EndOfLineCharacters = "\r\n",
                Port = 8989,
                ConnectionUnauthorizedString = "Not authorized"
            }, new MockUserService());
            _authServer.MessageEvent += OnMessageEvent;
            _authServer.ServerEvent += OnServerEvent;
            _authServer.ConnectionEvent += OnConnectionEvent;
            _authServer.ErrorEvent += OnErrorEvent;
            _authServer.Start();

            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void OnErrorEvent(object sender, TcpErrorServerAuthEventArgs<Guid> args)
        {
            Console.WriteLine(args.Message);
        }

        private static void OnConnectionEvent(object sender, TcpConnectionServerAuthEventArgs<Guid> args)
        {
            Console.WriteLine(args.ConnectionEventType);
        }

        private static void OnServerEvent(object sender, ServerEventArgs args)
        {
            Console.WriteLine(args.ServerEventType);
        }

        private static void OnMessageEvent(object sender, TcpMessageServerAuthEventArgs<Guid> args)
        {
            switch (args.MessageEventType)
            {
                case MessageEventType.Sent:
                    break;
                case MessageEventType.Receive:
                    Console.WriteLine(args.MessageEventType + ": " + args.Packet.Data);
                    break;
                default:
                    break;
            }
        }
    }
}
