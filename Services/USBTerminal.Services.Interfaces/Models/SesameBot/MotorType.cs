using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USBTerminal.Services.Interfaces.Models.SesameBot
{
    public enum MotorType
    {
        Servo, // takes 1 port. max 4 motors
        Stepper // takes 2 port. max 2 motors
    }
}
