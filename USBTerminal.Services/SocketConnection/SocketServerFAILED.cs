using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Serilog;
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
    //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
    public class SocketServerFAILED: ISocketServer
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

        public SocketServerFAILED(IEventAggregator eventAggregator,
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
            var listener = GetExistingOrCreateSocket(model);
            //listener.BeginAccept
            var localEndPoint = new IPEndPoint(IPAddress.Any, int.Parse(model.Port));
            listener.Bind(localEndPoint);
            listener.Listen(100);
            listener.BeginConnect(localEndPoint, OnConnected, listener);
        }

        private void OnConnected(IAsyncResult ar)
        {
            
        }

        private void ExecuteCloseConnectionCommand(NetworkConnection model)
        {
            var socket = GetExistingOrCreateSocket(model);
            try
            {
                socket.Close();
                logger.Information($"Network connection {model.IP} : {model.Port} closed.");
                var map = sockets.FirstOrDefault(s => s.LocalSocket.IP == model.IP);
                sockets.Remove(map);
                eventAggregator.GetEvent<ConnectionClosedEvent>().Publish(model);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
            socket.Close();
        }

        //private  long ToLongAddress(string addr)
        //{
        //    return IPAddress.Parse(addr).Address;
        //    //// careful of sign extension: convert to uint first;
        //    //// unsigned NetworkToHostOrder ought to be provided.
        //    //return (long)(uint)IPAddress.NetworkToHostOrder(
        //    //     (int)IPAddress.Parse(addr).Address);
        //}

        private void ExecuteConnectOverNetworkCommand(NetworkConnection model)
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddress = ipHostInfo.AddressList[0];

            //var ipLong = ToLongAddress(model.IP);
            if (!int.TryParse(model.Port, out var portInt))
            {
                eventAggregator.GetEvent<NetworkErrorEvent>().Publish(model);
                logger.Error($"Failed to connect over network. Invalid port {portInt}");
                return;
            }

            if (!IPEndPoint.TryParse(model.IP, out var ipAddress))
            {
                eventAggregator.GetEvent<NetworkErrorEvent>().Publish(model);
                logger.Error($"Failed to connect over network. Invalid IP address {model.IP}");
                return;
            }

            //var localEndPoint = new IPEndPoint(ipAddress.Address, portInt);
            var localEndPoint = new IPEndPoint(IPAddress.Any, portInt); // ToDo. Why need to listen to all adresses and not just one? one does not work 

            // Create a TCP/IP socket.  
            //Socket listener = new Socket(SocketType.Stream, ProtocolType.Tcp);
            var listener = GetExistingOrCreateSocket(model);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100); 
                //IPAddress.Any
                //while (true)
                //{
                //    // Set the event to nonsignaled state.  
                //    allDone.Reset();

                // Start an asynchronous socket to listen for connections.  
                logger.Information($"Waiting for a connection to {model.IP} : {model.Port}...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                //    // Wait until a connection is made before continuing.  
                //    allDone.WaitOne();
                //}

            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
                eventAggregator.GetEvent<NetworkErrorEvent>().Publish(model);
            }
        }

        // ToDo: may be need to install SimpleTCP nuget
        // https://www.youtube.com/watch?v=ve2LX1tOwIM&ab_channel=FoxLearn
        public void AcceptCallback(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                return; // user closed connection
            }
            // Signal the main thread to continue.  
           // allDone.Set();

            // ???? Client connected? 

            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.  
            StateObject state = new StateObject();
            state.WorkSocket = handler;
            handler.BeginReceive(state.Buffer, 0, state.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            var content = string.Empty;

            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            // Read data from the client socket.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.  
                state.Append(Encoding.ASCII.GetString(
                    state.Buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read
                // more data.  
                content = state.Text;
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the
                    // client. Display it on the console.  
                    logger.Information("Read {0} bytes from socket. \n Data : {1}",
                        content.Length, content);
                    // Echo the data back to the client.  
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.  
                    handler.BeginReceive(state.Buffer, 0, state.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                logger.Information("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }
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
