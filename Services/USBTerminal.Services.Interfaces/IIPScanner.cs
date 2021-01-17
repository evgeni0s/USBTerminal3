using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces
{
    public interface IIPScanner
    {
        List<DNSModel> GetAllDns();
        DNSModel GetCurrentMachineInfo();
    }
}
