using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Core.Models.SesameBot.Movements
{
    public class Movement
    {
        public string Name { get; set; }
        public int Speed { get; set; }
        public DateTime Duration { get; set; }
    }
}
