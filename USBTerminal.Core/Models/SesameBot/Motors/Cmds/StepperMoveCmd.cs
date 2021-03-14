using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Services.Interfaces.Models.SesameBot.Motors.Cmds
{
    public class StepperMoveCmd: MoveCmd
    {
        public string MoveType { get; set; }
        public string Steps { get; set; }
    }
}
