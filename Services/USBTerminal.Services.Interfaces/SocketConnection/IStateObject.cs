using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace USBTerminal.Services.Interfaces.SocketConnection
{
    public interface IStateObject
    {
        int BufferSize { get; }

        byte[] Buffer { get; }

        Socket WorkSocket { get; set; }

        string Text { get; }

        void Append(string text);

        void Reset();
    }
}
