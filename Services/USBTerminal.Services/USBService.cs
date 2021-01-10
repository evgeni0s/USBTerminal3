using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Timers;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Utils;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services
{
    public class USBService : IUSBService
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands commands;
        private readonly ILogger logger;
        private List<SerialPort> systemPorts;
        private List<SerialPortModel> localPorts;
        private int[] baudRates = new[] { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200 };
        private int[] dataBits = new[] { 7, 8, 9 };
        private double[] stopBits = new[] { 0, 1, 2, 1.5 };
        private string[] dataMode = new[] { "Text", "Hex" };
        private string[] parities = new[] { "None", "Odd", "Even", "Mark", "Space" };
        private readonly int RefreshRate = 5000;
        private Timer timer = new Timer();
        private readonly ChangesTracker<SerialPortModel, SerialPort> tracker;
        private readonly IMapper mapper;
        private DelegateCommand<SerialPortModel> openPortCommand;

        public USBService(IEventAggregator eventAggregator,
            IApplicationCommands commands,
            ILogger logger,
            IMapper mapper)
        {
            this.eventAggregator = eventAggregator;
            this.commands = commands;
            this.logger = logger;
            this.mapper = mapper;
            systemPorts = SerialPort.GetPortNames().Select(portName => new SerialPort(portName)).ToList();

            // Check ports that are available during start
            localPorts = systemPorts.Select(port => mapper.Map<SerialPortModel>(port)).ToList();
            tracker = new ChangesTracker<SerialPortModel, SerialPort>(localPorts, systemPorts, AreEqual);

            commands.OpenPortCommand.RegisterCommand(OpenPortCommand);

            // Listen to changes
            timer.Elapsed += new ElapsedEventHandler(RefreshPorts);
            timer.Interval = 5000;
            timer.Enabled = true;
        }


        public DelegateCommand<SerialPortModel> OpenPortCommand
        {
            get { return openPortCommand ?? (openPortCommand = new DelegateCommand<SerialPortModel>(ExecuteOpenPortCommand)); }
        }

        private void ExecuteOpenPortCommand(SerialPortModel port)
        {
            //... do something
            // event will fire automatically
           // Continue with why parity is null
        }

        public List<SerialPortModel> GetAll()
        {
            return localPorts;
        }

        public IEnumerable<int> GetBauds()
        {
            return baudRates;
        }

        public IEnumerable<int> GetDataBits()
        {
            return dataBits;
        }

        public IEnumerable<double> GetStopBits()
        {
            return stopBits;
        }

        public IEnumerable<string> GetDataModes() // for internal logic
        {
            return dataMode;
        }

        public IEnumerable<string> GetParities()
        {
            return parities;
        }

        object lockObject = new object();
        private void RefreshPorts(object sender, ElapsedEventArgs e)
        {
            lock (lockObject)
            {
                tracker.Refresh();
                foreach (var added in tracker.AddedItems)
                {
                    var localPort = mapper.Map<SerialPortModel>(added);
                    localPorts.Add(localPort);
                    eventAggregator.GetEvent<PortAddedEvent>().Publish(localPort);
                }

                foreach (var added in tracker.RemovedItems)
                {
                    var localPort = mapper.Map<SerialPortModel>(added);
                    localPorts.Remove(localPort);
                    eventAggregator.GetEvent<PortRemovedEvent>().Publish(localPort);
                }

                foreach (var updated in tracker.UpdatedItems)
                {
                    var systemPort = tracker.Match(updated);
                    if (!systemPort.IsOpen && updated.IsOpen)
                    {
                        eventAggregator.GetEvent<PortClosedEvent>().Publish(updated);
                    }
                    else if (systemPort.IsOpen && !updated.IsOpen)
                    {
                        eventAggregator.GetEvent<PortOpenedEvent>().Publish(updated);
                    }
                }
            }
        }

        private bool AreEqual(SerialPortModel portModel, SerialPort systemPorts)
        {
            if (portModel == null && systemPorts == null)
                return true;
            if (portModel == null || systemPorts == null)
                return false;
            return portModel.PortName == systemPorts.PortName;
        }

    }
}
