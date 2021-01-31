using System;
using System.Collections.Generic;
using System.Text;
using Prism.Events;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces.Events
{
    public class PortClosedEvent: PubSubEvent<SerialPortModel>
    {
    }
}
