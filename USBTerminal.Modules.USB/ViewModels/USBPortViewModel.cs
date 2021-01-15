using AutoMapper;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Modules.USB.ViewModels
{
    public class USBPortViewModel : ViewModelBase
    {
        private readonly IUSBService usbService;
        public IApplicationCommands applicationCommands;
        private IMapper mapper;
        private DelegateCommand openPortCommand;
        private DelegateCommand closePortCommand;
        private string _portName;
        private string _portState;
        private int _baudRate;
        private string _parity;
        private int _dataBits;
        private double _stopBits;
        private string _dataMode = "Default";
        private bool _isOpen;

        public USBPortViewModel(IUSBService usbService,
            IMapper mapper,
            IApplicationCommands applicationCommands)
        {
            this.usbService = usbService;
            this.applicationCommands = applicationCommands;
            this.mapper = mapper;
            BaudRatesList = usbService.GetBauds();
            DataBitsList = usbService.GetDataBits();
            StopBitsList = usbService.GetStopBits();
            DataModeList = usbService.GetDataModes();
            ParitiesList = usbService.GetParities();
        }

        public DelegateCommand OpenPortCommand
        {
            get { return openPortCommand ?? (openPortCommand = new DelegateCommand(ExecuteOpenPortCommand)); }
        }

        private void ExecuteOpenPortCommand()
        {
            PortState = "Waiting";
            RaisePropertyChanged(nameof(PortState));
            var dto = mapper.Map<SerialPortModel>(this);
            applicationCommands.OpenPortCommand.Execute(dto);
        }

        public DelegateCommand ClosePortCommand
        {
            get { return closePortCommand ?? (closePortCommand = new DelegateCommand(ExecuteClosePortCommand)); }
        }

        private void ExecuteClosePortCommand()
        {
            PortState = "Waiting";
            RaisePropertyChanged(nameof(PortState));
            var dto = mapper.Map<SerialPortModel>(this);
            applicationCommands.ClosePortCommand.Execute(dto);
        }

        public string PortName
        {
            get => _portName;
            set
            {
                SetProperty(ref _portName, value);
            }
        }

        public int BaudRate
        {
            get => _baudRate;
            set
            {
                SetProperty(ref _baudRate, value);
            }
        }

        public string Parity
        {
            get => _parity;
            set
            {
                SetProperty(ref _parity, value);
            }
        }

        public int DataBits
        {
            get => _dataBits;
            set
            {
                SetProperty(ref _dataBits, value);
            }
        }

        public double StopBits
        {
            get => _stopBits;
            set
            {
                SetProperty(ref _stopBits, value);
            }
        }
        public string DataMode
        {
            get => _dataMode;
            set
            {
                SetProperty(ref _dataMode, value);
            }
        }

        public bool IsOpen
        {
            get { return _isOpen; }
            set 
            { 
                _isOpen = value;
                PortState = value ? "Pressed" : "Default";
                RaisePropertyChanged(nameof(PortState)); // For some reaason SetProperty does not update GUI
                SetProperty(ref _isOpen, value);
            }
        }

        public string PortState
        {
            get { return _portState; }
            set 
            { 
                _portState = value;
                SetProperty(ref _portState, value);
            }
        }

        public IEnumerable<int> BaudRatesList { get; }
        public IEnumerable<double> StopBitsList { get; }
        public IEnumerable<int> DataBitsList { get; }
        public IEnumerable<string> DataModeList { get; }
        public IEnumerable<string> ParitiesList { get; }
    }
}
