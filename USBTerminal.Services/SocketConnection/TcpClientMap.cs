using SimpleTCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.SocketConnection
{
    public class TcpClientMap
    {
        public NetworkAddress Address { get; set; }
        public SimpleTcpClient Client { get; set; }

    }
}
