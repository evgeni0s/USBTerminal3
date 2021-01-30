using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces.Events
{
    public class NetworkScanCompletedEvent: PubSubEvent<List<DNSModel>>
    {
    }
}
