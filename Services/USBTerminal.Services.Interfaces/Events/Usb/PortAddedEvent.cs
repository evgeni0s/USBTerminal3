using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces.Events
{
    // New port phisically has appeared in system.
    public class PortAddedEvent : PubSubEvent<SerialPortModel>
    {
    }
}
