using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models.Network;

namespace USBTerminal.Services.Interfaces.Events.Network
{
    public class NetworkMessageSentEvent : PubSubEvent<NetworkMessage>
    {
    }
}
