using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces.Events.SeasameBot
{
    public class BotConnectionFailedEvent : PubSubEvent<NetworkAddress>
    {
    }
}
