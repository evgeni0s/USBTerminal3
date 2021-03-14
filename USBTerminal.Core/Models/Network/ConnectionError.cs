using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Services.Interfaces.Models.Network
{
    public class ConnectionError
    {
        public string ErrorMesage { get;
            set; }
        public NetworkAddress Address { get; set; }
    }
}
