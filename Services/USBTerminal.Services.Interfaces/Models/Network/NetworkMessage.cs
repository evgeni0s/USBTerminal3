﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Services.Interfaces.Models.Network
{
    public class NetworkMessage
    {
        public NetworkConnection Address { get; set; }
        public string Payload { get; set; }
    }
}
