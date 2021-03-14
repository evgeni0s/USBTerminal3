using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models.Network;

namespace USBTerminal.Services.Interfaces.Events.Network
{
    public class NetworkMessageRecievedEvent : PubSubEvent<NetworkMessage>
    {
    }
}
