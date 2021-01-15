using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Timers;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Utils;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services
{
    public class USBService : IUSBService, IDisposable
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
        private DelegateCommand<SerialPortModel> closePortCommand;
        private DelegateCommand<SerialPortMessage> sendMessageToPortCommand;

        public USBService(IEventAggregator eventAggregator,
            IApplicationCommands commands,
            ILogger logger,
            IMapper mapper)
        {
            this.eventAggregator = eventAggregator;
            this.commands = commands;
            this.logger = logger;
            this.mapper = mapper;
            systemPorts = SerialPort.GetPortNames().Select(CreateSystemPort).ToList();

            // Check ports that are available during start
            localPorts = systemPorts.Select(port => mapper.Map<SerialPortModel>(port)).ToList();
            tracker = new ChangesTracker<SerialPortModel, SerialPort>(localPorts, systemPorts, AreEqual);

            commands.OpenPortCommand.RegisterCommand(OpenPortCommand);
            commands.ClosePortCommand.RegisterCommand(ClosePortCommand);
            commands.SendMessageToPortCommand.RegisterCommand(SendMessageToPortCommand);

            // Listen to changes
            timer.Elapsed += new ElapsedEventHandler(RefreshPorts);
            timer.Interval = 5000;
            timer.Enabled = true;
            Test();
        }

        static bool IsSubscribed;

        private void Test()
        {
            foreach (var port in systemPorts)
            {
                port.DataReceived += dataReceived;
            }
            IsSubscribed = true;
        }

        private SerialPort CreateSystemPort(string portName)
        {
            var port = new SerialPort(portName);
            return port;
        }

        public void dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == System.IO.Ports.SerialData.Eof)
            {
                return;
            }

            var port = sender as SerialPort;
            var localPort = tracker.Match(port);  
            var @event = new SerialPortMessage();
            if (localPort.DataMode == "Text")  // ToDo: DataMode is null. Either skip this check or make
                                               //syncronization between 2 instances of all
            {
                @event.TextData = port.ReadExisting();
            }
            else if (localPort.DataMode == "Hex")
            {
                logger.Warning("Warning. Reading data in hex format. Functionality was not tested yet.");
                int bytes = port.BytesToRead;
                @event.HexData = new byte[bytes];
                port.Read(@event.HexData, 0, bytes);
            }

            logger.Information($"{port.PortName}: Data recieved.");
            eventAggregator.GetEvent<UsbMessageReceivedEvent>().Publish(@event);
        }

        public DelegateCommand<SerialPortModel> OpenPortCommand
        {
            get { return openPortCommand ?? (openPortCommand = new DelegateCommand<SerialPortModel>(ExecuteOpenPortCommand)); }
        }

        public DelegateCommand<SerialPortModel> ClosePortCommand
        {
            get { return closePortCommand ?? (closePortCommand = new DelegateCommand<SerialPortModel>(ExecuteClosePortCommand)); }
        }

        public DelegateCommand<SerialPortMessage> SendMessageToPortCommand
        {
            get { return sendMessageToPortCommand ?? (sendMessageToPortCommand = new DelegateCommand<SerialPortMessage>(ExecuteSendMessageCommand)); }
        }

        private void ExecuteSendMessageCommand(SerialPortMessage message)
        {
            if ((message.HexData == null || message.HexData.Length == 0) && message.TextData == null)
            {
                logger.Error("Error sending message. Message is empty.");
                return;
            }
            var openedPorts = systemPorts.Where(port => port.IsOpen).ToList();
            if (openedPorts.Count == 0)
            {
                logger.Error("Error sending message. No opened ports.");
                return;
            }

            foreach (var port in openedPorts)
            {
                if (message.TextData != null)
                {
                    port.Write(message.TextData);
                    logger.Information($"{port.PortName} : message sent.");
                }
            }
        }

        private void ExecuteOpenPortCommand(SerialPortModel port)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(port);
                logger.Information($"Trying to open port: {jsonString}");

                // Copy properties to local port since remotely they could change on GUI 
                var localPort = localPorts.FirstOrDefault(p => AreEqual(p,port));
                mapper.Map(port, localPort);

                // Copy properties to system port
                var systemPort = tracker.Match(port);
                mapper.Map(port, systemPort);
                systemPort.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ExecuteClosePortCommand(SerialPortModel port)
        {
            logger.Information($"Trying to close port: {port.PortName}");
            var systemPort = tracker.Match(port);
            mapper.Map(systemPort, port);
            systemPort.Close();
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
                    added.DataReceived += dataReceived;
                    eventAggregator.GetEvent<PortAddedEvent>().Publish(localPort);
                }

                foreach (var removed in tracker.RemovedItems)
                {
                    var localPort = mapper.Map<SerialPortModel>(removed);
                    var systemPort = tracker.Match(localPort);
                    systemPort.DataReceived -= dataReceived;
                    localPorts.Remove(localPort);
                   // eventAggregator.GetEvent<PortRemovedEvent>().Publish(localPort);
                }

                foreach (var updated in tracker.UpdatedItems)
                {
                    var systemPort = tracker.Match(updated);
                    if (!systemPort.IsOpen && updated.IsOpen)
                    {
                        mapper.Map(systemPort, updated); // Save changes locally
                        eventAggregator.GetEvent<PortClosedEvent>().Publish(updated);
                    }
                    else if (systemPort.IsOpen && !updated.IsOpen)
                    {
                        mapper.Map(systemPort, updated); // Save changes locally
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

        private bool AreEqual(SerialPortModel first, SerialPortModel second)
        {
            if (first == null && first == second)
                return true;
            if (first == null || second == null)
                return false;
            return first.PortName == second.PortName;
        }


        public void Dispose()
        {
            foreach (var systemPort in systemPorts)
            {
                systemPort.DataReceived -= dataReceived;
            }
        }
        ~USBService()
        {
            foreach (var systemPort in systemPorts)
            {
                systemPort.DataReceived -= dataReceived;
            }
        }
    }
}
