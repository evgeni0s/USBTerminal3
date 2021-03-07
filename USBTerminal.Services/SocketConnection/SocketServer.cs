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
using USBTerminal.Services.Interfaces.Models.Network;
using USBTerminal.Services.Interfaces.SocketConnection;

namespace USBTerminal.Services.SocketConnection
{
    //https://github.com/BrandonPotter/SimpleTCP
    public class SocketServer: ISocketServer
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands commands;
        private readonly ILogger logger;
        private List<TcpClientMap> clientsWithAddresses;
        private DelegateCommand<NetworkAddress> connectOverNetworkCommand;
        private DelegateCommand<NetworkAddress> closeConnectionCommand;
        private DelegateCommand<NetworkMessage> sendMessageOnNetworkCommand;
        // Thread signal.  
        public ManualResetEvent allDone = new ManualResetEvent(false);
        private const string defaultPort = "34931";

        private SimpleTcpServer activeServer;

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

            clientsWithAddresses = new List<TcpClientMap>();
           // activeServers = new List<SimpleTcpServer>();
        }


        public DelegateCommand<NetworkAddress> ConnectOverNetworkCommand
        {
            get { return connectOverNetworkCommand ?? (connectOverNetworkCommand = new DelegateCommand<NetworkAddress>(ExecuteConnectOverNetworkCommand)); }
        }

        public DelegateCommand<NetworkAddress> CloseConnectionCommand
        {
            get { return closeConnectionCommand ?? (closeConnectionCommand = new DelegateCommand<NetworkAddress>(ExecuteCloseConnectionCommand)); }
        }

        public DelegateCommand<NetworkMessage> SendMessageOnNetworkCommand
        {
            get { return sendMessageOnNetworkCommand ?? (sendMessageOnNetworkCommand = new DelegateCommand<NetworkMessage>(ExecuteSendMessageOnNetworkCommand)); }
        }

        public void ExecuteSendMessageOnNetworkCommand(NetworkMessage message)
        {
            // connection worked from laptop to PC when PC server has IP 192.168.2.208
            // connection worked from PC as client when PC server has IP 192.168.2.208
            //var client = new SimpleTcpClient().Connect(message.Address.IP, int.Parse(message.Address.Port));
            //var replyMsg = client.WriteLineAndGetReply(message.Payload, TimeSpan.FromSeconds(3));

            //commands.LoggingCommand.Execute()

            var kvp = clientsWithAddresses.FirstOrDefault(c => c.Address == message.Address);

            //var replyMsg = kvp.Client.WriteLineAndGetReply(message.Payload, TimeSpan.FromSeconds(3)); // working, but slowly. Will wait for event
            kvp.Client.WriteLine(message.Payload);

            eventAggregator.GetEvent<NetworkMessageSentEvent>().Publish(message);
        }

        public void ExecuteCloseConnectionCommand(NetworkAddress address)
        {
            var map = clientsWithAddresses.FirstOrDefault(c => c.Address == address);
            if (map == null)
            {
                logger.Error("Could not close connection, because it does not exist.");
                return;
            }
            map.Client.DataReceived -= ClientDataReceived;
            map.Client.Disconnect();
            clientsWithAddresses.Remove(map);
            eventAggregator.GetEvent<ConnectionClosedEvent>().Publish(address);
        }

        public void ExecuteConnectOverNetworkCommand(NetworkAddress address)
        {
            // DO NOT DELETE THIS
            // we can start server only on current IP, so it will be only 1
            //activeServer = new SimpleTcpServer().Start(IPAddress.Parse(model.IP), int.Parse(model.Port));
            //activeServer.Delimiter = 0x13;
            //activeServer.DelimiterDataReceived += (sender, msg) => {
            //    msg.ReplyLine("You said: " + msg.MessageString);
            //};


            //var test1 = server.GetIPAddresses();
            //var test2 = server.GetListeningIPs(); // intrestingly this has 1 address out of box

            //activeServers.Add(server);
            if (!clientsWithAddresses.Any(c => c.Address == address))
            {
                try
                {
                    var client = new SimpleTcpClient().Connect(address.IP, int.Parse(address.Port));
                    var map = new TcpClientMap { Client = client, Address = address };
                    client.DataReceived += ClientDataReceived;
                    clientsWithAddresses.Add(map);
                    eventAggregator.GetEvent<ConnectionSuccessEvent>().Publish(address);

                }
                catch (Exception e)
                {
                    var error = new ConnectionError
                    {
                        Address = address,
                        ErrorMesage = $"{DiagnoseError(e)} e.Message + e.ToString()"
                    };
                    eventAggregator.GetEvent<ConnectionFailedEvent>().Publish(error);
                }
            }
            else
            {
                var error = new ConnectionError
                {
                    Address = address,
                    ErrorMesage = $"Already connected to this device"
                };

                eventAggregator.GetEvent<ConnectionFailedEvent>().Publish(error);
            }
        }

        private string DiagnoseError(Exception e)
        {
            if (string.IsNullOrEmpty(e.Message))
            {
                return null;
            }
            if (e.Message.Contains("No connection could be made because the target machine actively refused it"))
            {
                return "It is likely that connection is made on the wrong port. Try changeing it";
            }
            return null;
        }
       // Continue with investigation why on laptop seasame screen does not open. Check path to animation. Try to debug there 
        private void ClientDataReceived(object sender, Message e)
        {
            var clientAddress = FindAddress(sender as SimpleTcpClient);
            var message = new NetworkMessage() { Address = clientAddress, Payload = e.MessageString };
            eventAggregator.GetEvent<NetworkMessageRecievedEvent>().Publish(message);
        }

        private NetworkAddress FindAddress(SimpleTcpClient client)
        {
            return clientsWithAddresses.FirstOrDefault(kvp => kvp.Client == client).Address;
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

        public List<NetworkAddress> GetActiveConnections()
        {
            return clientsWithAddresses.Select(map => map.Address).ToList();
        }
    }
}
