using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces;

namespace USBTerminal.Modules.Wifi.ViewModels
{
    public class NetworkScannerViewModel : RegionViewModelBase
    {
        private readonly ILogger logger;
        private DelegateCommand scanCommand;
        private DelegateCommand saveCommand;
        private DelegateCommand clearCommand;
        private readonly IApplicationCommands applicationCommands;
        private readonly IIPScanner ipScanner;
        private readonly IEventAggregator eventAggregator;
        private readonly IMapper mapper;
        private readonly IContainerProvider container;
        public NetworkScannerViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator,
            IMapper mapper, 
            IIPScanner ipScanner,
            IContainerProvider container)
            : base(regionManager, logger)
        {
            this.logger = logger;
            this.applicationCommands = applicationCommands;
            this.ipScanner = ipScanner;
            this.eventAggregator = eventAggregator;
            this.container = container;
            this.mapper = mapper;
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

        public DelegateCommand ScanCommand
        {
            get { return scanCommand ?? (scanCommand = new DelegateCommand(ExecuteScanCommand)); }
        }
        private void ExecuteScanCommand()
        {
            
        }
    }
}
