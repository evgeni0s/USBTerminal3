using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models;
using USBTerminal.Services.Interfaces.Models.Network;

namespace USBTerminal.Services.Interfaces.SocketConnection
{
    public interface ISocketServer
    {
        string GetDefaultPort();
        List<NetworkAddress> GetActiveConnections();
        void ExecuteConnectOverNetworkCommand(NetworkAddress address);
    }
}
