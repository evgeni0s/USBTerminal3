using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models.SesameBot.Motors;

namespace USBTerminal.Services.Interfaces.Models.SesameBot
{
    public class SesameBot
    {
        public string Name { get; set; }
        public NetworkAddress Address { get; set; }
        public List<Motor> MyProperty { get; set; }
    }
}
