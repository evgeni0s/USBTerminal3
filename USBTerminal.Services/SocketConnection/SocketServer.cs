using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Serilog;
using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using USBTerminal.Core.Interfaces;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Events.Network;
using USBTerminal.Services.Interfaces.Models;
using USBTerminal.Services.Interfaces.SocketConnection;

namespace USBTerminal.Services.SocketConnection
{
    //https://github.com/BrandonPotter/SimpleTCP
    public class SocketServer: ISocketServer
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands commands;
        private readonly ILogger logger;
        private List<SocketMap> sockets;
        private DelegateCommand<NetworkConnection> connectOverNetworkCommand;
        private DelegateCommand<NetworkConnection> closeConnectionCommand;
        private DelegateCommand<NetworkConnection> sendMessageOnNetworkCommand;
        // Thread signal.  
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private const string defaultPort = "34931";

        public SocketServer(IEventAggregator eventAggregator,
            IApplicationCommands commands,
            ILogger logger)
        {
            this.eventAggregator = eventAggregator;
            this.commands = commands;
            this.logger = logger;
            commands.OpenNetworkConnectionCommand.RegisterCommand(ConnectOverNetworkCommand);
            commands.CloseNetworkConnectionCommand.RegisterCommand(CloseConnectionCommand);
            commands.SendMessageOnNetworkCommand.RegisterCommand(SendMessageOnNetworkCommand);

            sockets = new List<SocketMap>();
        }


        public DelegateCommand<NetworkConnection> ConnectOverNetworkCommand
        {
            get { return connectOverNetworkCommand ?? (connectOverNetworkCommand = new DelegateCommand<NetworkConnection>(ExecuteConnectOverNetworkCommand)); }
        }

        public DelegateCommand<NetworkConnection> CloseConnectionCommand
        {
            get { return closeConnectionCommand ?? (closeConnectionCommand = new DelegateCommand<NetworkConnection>(ExecuteCloseConnectionCommand)); }
        }

        public DelegateCommand<NetworkConnection> SendMessageOnNetworkCommand
        {
            get { return sendMessageOnNetworkCommand ?? (sendMessageOnNetworkCommand = new DelegateCommand<NetworkConnection>(ExecuteSendMessageOnNetworkCommand)); }
        }

        // Continue with: Client and server cant talk on the same port. One needs to call BeginConnect and the other beginAccept
        private void ExecuteSendMessageOnNetworkCommand(NetworkConnection model)
        {
            // connection worked from laptop to PC when PC server has IP 192.168.2.208
            // connection worked from PC as client when PC server has IP 192.168.2.208
            var client = new SimpleTcpClient().Connect(model.IP, int.Parse(model.Port));
            var replyMsg = client.WriteLineAndGetReply("Hello world!", TimeSpan.FromSeconds(3));
        }

        private void OnConnected(IAsyncResult ar)
        {
            
        }

        private void ExecuteCloseConnectionCommand(NetworkConnection model)
        {
        }

        private void ExecuteConnectOverNetworkCommand(NetworkConnection model)
        {
            var server = new SimpleTcpServer().Start(IPAddress.Parse(model.IP), int.Parse(model.Port));
            server.Delimiter = 0x13;
            server.DelimiterDataReceived += (sender, msg) => {
                msg.ReplyLine("You said: " + msg.MessageString);
            };
        }

        public void AcceptCallback(IAsyncResult ar)
        {
           
        }

        public void ReadCallback(IAsyncResult ar)
        {
 
        }

        private void Send(Socket handler, String data)
        {

        }

        private void SendCallback(IAsyncResult ar)
        {

        }

        public string GetDefaultPort()
        {
            return defaultPort;
        }

        public Socket GetExistingOrCreateSocket(NetworkConnection model)
        {
            var map = sockets.FirstOrDefault(s => s.LocalSocket.IP == model.IP);
            if (map == null)
            {
                map = new SocketMap();
                map.LocalSocket = model;
                map.SystemSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                sockets.Add(map);
            }
            return map.SystemSocket;
        }
    }
}
