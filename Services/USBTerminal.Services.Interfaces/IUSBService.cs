using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces
{
    public interface IUSBService
    {
        List<SerialPortModel> GetAll();
        IEnumerable<int> GetBauds();
        IEnumerable<string> GetDataModes();
        IEnumerable<string> GetParities();
        IEnumerable<int> GetDataBits();
        IEnumerable<double> GetStopBits();
    }
}
