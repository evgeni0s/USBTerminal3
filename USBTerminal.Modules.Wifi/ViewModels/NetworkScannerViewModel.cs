using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using USBTerminal.Core.Enums;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Models;
using USBTerminal.Services.Interfaces.Network.Events;
using USBTerminal.Services.Interfaces.SocketConnection;

namespace USBTerminal.Modules.Wifi.ViewModels
{
    public class NetworkScannerViewModel : RegionViewModelBase
    {
        private DelegateCommand scanNetworkCommand;
        private DelegateCommand saveCommand;
        private DelegateCommand clearCommand;
        private readonly IIPScanner ipScanner;
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands applicationCommands;
        private readonly IMapper mapper;
        private readonly IContainerProvider container;
        private readonly ISocketServer socketServer;
        //private string ip;
        private string dnsScanningState;
        private List<NetworkConnectionViewModel> networkConnections;
        private NetworkConnection machineInfo;
        private string title = "Network Scanner";
        private string _connectionState;

        public NetworkScannerViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator,
            IMapper mapper, 
            IIPScanner ipScanner,
            IContainerProvider container,
            ISocketServer socketServer
            )
            : base(regionManager, logger)
        {
            this.applicationCommands = applicationCommands;
            this.ipScanner = ipScanner;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<NetworkScanCompletedEvent>().Subscribe(OnNetworkScanCompleted);
            this.container = container;
            this.mapper = mapper;
            this.socketServer = socketServer;
            MachineInfo = ipScanner.GetCurrentMachineInfo();
            ExecuteScanNetworkCommand();
        }

        public DelegateCommand ScanNetworkCommand
        {
            get { return scanNetworkCommand ?? (scanNetworkCommand = new DelegateCommand(ExecuteScanNetworkCommand)); }
        }

        private void ExecuteScanNetworkCommand()
        {
            DnsScanningState = ButtonStates.Waiting;
            RaisePropertyChanged(nameof(DnsScanningState));
            var baseIp = GetBaseIp(MachineInfo.IP);
            applicationCommands.ScanNetworkCommand.Execute(baseIp);
        }

        public NetworkConnection MachineInfo
        {
            get { return machineInfo; }
            set { SetProperty(ref machineInfo, value); }
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public string DnsScanningState
        {
            get { return dnsScanningState; }
            set { SetProperty(ref dnsScanningState, value); }
        }

        public List<NetworkConnectionViewModel> NetworkConnections
        {
            get { return networkConnections; }
            set { SetProperty(ref networkConnections, value); }
        }

        public DelegateCommand SaveCommand
        {
            get { return saveCommand ?? (saveCommand = new DelegateCommand(ExecuteSaveCommand)); }
        }

        private void ExecuteSaveCommand()
        {
            throw new NotImplementedException();
        }

        public DelegateCommand ClearCommand
        {
            get { return clearCommand ?? (clearCommand = new DelegateCommand(ExecuteClearCommand)); }
        }

        private void ExecuteClearCommand()
        {
            throw new NotImplementedException();
        }

        private void OnNetworkScanCompleted(List<NetworkConnection> networkConnections)
        {
            NetworkConnections = networkConnections.Select(CreateViewModel).ToList();
           
            DnsScanningState = ButtonStates.Default;
        }

        private string GetBaseIp(string ip)
        {
            var baseIpEnd = ip.LastIndexOf('.');
            return ip.Substring(0, baseIpEnd + 1);
        }

        private NetworkConnectionViewModel CreateViewModel(NetworkConnection connection)
        {
            var viewModel = container.Resolve<NetworkConnectionViewModel>();

            // Copy fields from service
            mapper.Map(connection, viewModel);

            // DataMode is a local field. Service knows nothing about it. 
            // Need to set default value
            viewModel.Port = socketServer.GetDefaultPort();
            return viewModel;
        }
    }
}
