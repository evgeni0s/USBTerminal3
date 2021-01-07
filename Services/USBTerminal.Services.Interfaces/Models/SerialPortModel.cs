using System;
using System.Collections.Generic;
using System.Text;

namespace USBTerminal.Services.Interfaces.Models
{
    public class SerialPortModel
    {
        public string Name { get; set; }
        public string BaudRate { get; set; }
        public string Parity { get; set; }
        public string DataBits { get; set; }
        public string StopBits { get; set; }
        public string DataMode { get; set; }
        public bool IsOpen { get; set; }

    }
}
