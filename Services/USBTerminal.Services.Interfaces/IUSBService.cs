using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces
{
    public interface IUSBService
    {
        //public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);
        List<SerialPortModel> GetAll();

        // todo: event PortClosed => update all props
    }
}
