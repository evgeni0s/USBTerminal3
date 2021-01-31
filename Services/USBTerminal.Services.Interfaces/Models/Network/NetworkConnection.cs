using System;
using System.Collections.Generic;
using System.Text;

namespace USBTerminal.Services.Interfaces.Models
{
    public class NetworkConnection
    {
        public string IP { get; set; }

        /// <summary>
        /// "10.22.4."
        /// </summary>
        public string BaseIP { get; set; }
        public string HostName { get; set; }
        public string Port { get; set; }
    }
}
