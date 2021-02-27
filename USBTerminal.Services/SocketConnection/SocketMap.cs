using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.SocketConnection
{
    // Associate local socket with remote. 
    // System.Net.Sockets.Socket has LocalEndPoint and RemoteEndPoint, but  
    // from them I cant get IP
    [Obsolete("Now using simple TCP instead of low level sockets")]
    public class SocketMap
    {
        public NetworkAddress LocalSocket { get; set; }
        public Socket SystemSocket { get; set; }
    }
}
