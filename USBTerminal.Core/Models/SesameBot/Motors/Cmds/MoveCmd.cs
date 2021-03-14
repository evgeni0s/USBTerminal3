using System;
using System.Collections.Generic;
using System.Text;

namespace USBTerminal.Services.Interfaces.Models.SesameBot.Motors
{
    public class MoveCmd
    {
        public string MotorName { get; set; }
        public string Direction { get; set; }
    }
}


//continue with tests and serialization