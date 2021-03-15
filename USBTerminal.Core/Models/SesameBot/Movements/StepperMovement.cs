using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Core.Models.SesameBot.Movements
{
    public class StepperMovement
    {
        public string Name { get; set; }
        public int Speed { get; set; }
        public StepperMoveType StepperMoveType { get; set; }
        public int Steps { get; set; }
    }
}
