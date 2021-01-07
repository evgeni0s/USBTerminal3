using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Modules.USB.ViewModels
{
    public class USBPanelViewModel : RegionViewModelBase
    {
        private readonly ILogger logger;
        // private DelegateCommand clearCommand;
        private readonly IApplicationCommands applicationCommands;
        private readonly IUSBService usbService;
        private readonly IEventAggregator eventAggregator;
        private ObservableCollection<SerialPortModel> _availablePorts;

        public USBPanelViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IUSBService usbService,
            IEventAggregator eventAggregator)
            : base(regionManager, logger)
        {
            this.logger = logger;
            this.applicationCommands = applicationCommands;
            this.usbService = usbService;
            this.eventAggregator = eventAggregator;
            AvailablePorts = new ObservableCollection<SerialPortModel>(this.usbService.GetAll());
        }

        public ObservableCollection<SerialPortModel> AvailablePorts
        {
            get { return _availablePorts; }
            set
            {
                SetProperty(ref _availablePorts, value);
            }
        }

    }
}
