using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models.SesameBot.Motors;

namespace USBTerminal.Services.Interfaces.Models.SesameBot
{
    public class CreateMotorCmd
    {
        public Motor Motor { get; set; }
        public string Steps { get; set; }
    }
}
