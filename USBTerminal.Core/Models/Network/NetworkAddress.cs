using System;
using System.Collections.Generic;
using System.Text;

namespace USBTerminal.Services.Interfaces.Models
{
    public record NetworkAddress
    {
        public string IP { get; set; }

        /// <summary>
        /// "10.22.4."
        /// </summary>
        public string BaseIP { get; set; }
        public string HostName { get; set; }
        public string Port { get; set; }

        //public bool AreEqual(NetworkAddress other)
        //{
        //    if (other == null)
        //    {
        //        return false;
        //    }
        //    return Port == other.Port
        //        && IP == other.IP;
        //}
    }
}
