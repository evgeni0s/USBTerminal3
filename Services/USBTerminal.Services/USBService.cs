using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using USBTerminal.Core.Interfaces;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services
{
    public class USBService : IUSBService
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands commands;
        private readonly ILogger logger;
        private List<SerialPort> ports;

        public USBService(IEventAggregator eventAggregator,
            IApplicationCommands commands,
            ILogger logger)
        {
            this.eventAggregator = eventAggregator;
            this.commands = commands;
            this.logger = logger;
        }

        private void RefreshPorts()
        {
            var portsNames = SerialPort.GetPortNames();
            var newPorts = portsNames.Select(NewPort);
            var tracker = new USBTerminal.Core.Utils.ChangesTracker<SerialPortModel, SerialPort>(newPorts, ports, AreEqual);
        }

        private bool AreEqual(SerialPortModel arg1, SerialPort arg2)
        {
            throw new NotImplementedException();
        }

        public List<SerialPortModel> GetAll()
        {
            throw new NotImplementedException();
        }

        public static SerialPortModel NewPort(string portName) 
        { 
            return new SerialPortModel
            {
                Name = portName,
                BaudRate = "19200",
                DataBits = "8",
                StopBits = "1",
                DataMode = "Hex"
            };
        }
    }
}
