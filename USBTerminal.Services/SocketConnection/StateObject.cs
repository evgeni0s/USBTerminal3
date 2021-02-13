using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using USBTerminal.Services.Interfaces.SocketConnection;

namespace USBTerminal.Services.SocketConnection
{
    public class StateObject : IStateObject
    {
        // Received data string.
        private StringBuilder sb = new StringBuilder();

        // Size of receive buffer.  
        public int BufferSize => this.Buffer.Length;

        // Receive buffer.  
        public byte[] Buffer { get; } = new byte[1024];

        // Client socket.
        public Socket WorkSocket { get; set; } = null;

        public string Text => this.sb.ToString();

        public void Append(string text) => this.sb.Append(text);

        public void Reset() => this.sb = new StringBuilder();

    }
}
