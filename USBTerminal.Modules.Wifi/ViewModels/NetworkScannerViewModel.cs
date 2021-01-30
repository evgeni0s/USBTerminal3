using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Modules.Wifi.ViewModels
{
    public class NetworkScannerViewModel : RegionViewModelBase
    {
        private DelegateCommand scanCommand;
        private DelegateCommand saveCommand;
        private DelegateCommand clearCommand;
        private readonly IIPScanner ipScanner;
        private readonly IEventAggregator eventAggregator;
        //private readonly IMapper mapper;
        //private readonly IContainerProvider container;
        //private string ip;
        //private string hostName;
        private List<DNSModel> dnsModels;
        private DNSModel machineInfo;
        private string title = "Network Scanner";

        public NetworkScannerViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator,
            //IMapper mapper, 
            IIPScanner ipScanner
            //IContainerProvider container
            )
            : base(regionManager, logger)
        {
            ApplicationCommands = applicationCommands;
            this.ipScanner = ipScanner;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<NetworkScanCompletedEvent>().Subscribe(OnNetworkScanCompleted);
            //this.container = container;
            //this.mapper = mapper;
            MachineInfo = ipScanner.GetCurrentMachineInfo();
        }

        public IApplicationCommands ApplicationCommands { get; set; }

        public DNSModel MachineInfo
        {
            get { return machineInfo; }
            set { SetProperty(ref machineInfo, value); }
        }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        //public string Ip
        //{
        //    get { return ip; }
        //    set { SetProperty(ref ip, value); }
        //}

        //public string HostName
        //{
        //    get { return hostName; }
        //    set { SetProperty(ref hostName, value); }
        //}

        public List<DNSModel> DnsModels
        {
            get { return dnsModels; }
            set { SetProperty(ref dnsModels, value); }
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

        private void OnNetworkScanCompleted(List<DNSModel> dnsList)
        {
            DnsModels = dnsList;
        }

        //public DelegateCommand ScanCommand
        //{
        //    get { return scanCommand ?? (scanCommand = new DelegateCommand(ExecuteScanCommand)); }
        //}
        //private void ExecuteScanCommand()
        //{

        //}
    }
}
