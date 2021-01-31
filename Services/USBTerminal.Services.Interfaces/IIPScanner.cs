using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces
{
    public interface IIPScanner
    {
        NetworkConnection GetCurrentMachineInfo(); // What is my IP
        // To start scanning use Command. 
        // To get response listen to event.
    }
}
