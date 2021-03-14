using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Interfaces.Models
{
    public class SerialPortMessage
    {
        public string TextData { get; set; }
        public byte[] HexData { get; set; }
    }
}
