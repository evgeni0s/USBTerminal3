using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace USBTerminal.Services.Interfaces.Events
{
    /// <summary>
    /// User hit enter on Terminal. Such command should be sent to usb device
    /// </summary>
    public class TerminalInputEvent : PubSubEvent<string>
    {
    }
}
