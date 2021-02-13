using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
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
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Modules.USB.ViewModels
{
    // ToDo: write tests
    public class USBPanelViewModel : RegionViewModelBase
    {
        // private DelegateCommand clearCommand;
        private readonly IApplicationCommands applicationCommands;
        private readonly IUSBService usbService;
        private readonly IEventAggregator eventAggregator;
        private readonly IMapper mapper;
        private readonly IContainerProvider container;
        private ObservableCollection<USBPortViewModel> _availablePorts;

        public USBPanelViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator,
            IUSBService usbService,
            IMapper mapper,
            IContainerProvider container)
            : base(regionManager, logger)
        {
            this.applicationCommands = applicationCommands;
            this.usbService = usbService;
            this.eventAggregator = eventAggregator;
            this.container = container;
            this.mapper = mapper;
            var portViewModels = this.usbService.GetAll().Select(CreatePortViewModel);
            logger.Information($"Found {portViewModels.Count()} ports.");
            foreach (var port in portViewModels)
            {
                logger.Information($"{port.PortName}");
            }
            AvailablePorts = new ObservableCollection<USBPortViewModel>(portViewModels);
            eventAggregator.GetEvent<PortAddedEvent>().Subscribe(AddPort, ThreadOption.UIThread);
            eventAggregator.GetEvent<PortRemovedEvent>().Subscribe(RemovePort, ThreadOption.UIThread);
            eventAggregator.GetEvent<PortOpenedEvent>().Subscribe(OnPortOpened);
            eventAggregator.GetEvent<PortClosedEvent>().Subscribe(OnPortClosed);
            eventAggregator.GetEvent<PortErrorEvent>().Subscribe(OnPortError);
        }

        public ObservableCollection<USBPortViewModel> AvailablePorts
        {
            get { return _availablePorts; }
            set
            {
                SetProperty(ref _availablePorts, value);
            }
        }
        private void OnPortClosed(SerialPortModel port)
        {
            Logger.Information($"Closed port: {port.PortName}");
            GetViewModel(port).IsOpen = port.IsOpen;
        }

        private void OnPortOpened(SerialPortModel port)
        {
            Logger.Information($"Opened port: {port.PortName}");
            GetViewModel(port).IsOpen = port.IsOpen;
        }

        private void RemovePort(string portName)
        {
            Logger.Information($"Removed port: {portName}");
            var vmToRemove = AvailablePorts.FirstOrDefault(p => p.PortName == portName);
            AvailablePorts.Remove(vmToRemove);
        }

        private void AddPort(SerialPortModel port)
        {
            Logger.Information($"Added port: {port.PortName}");
            AvailablePorts.Add(CreatePortViewModel(port));
        }

        private void OnPortError(SerialPortModel port)
        {
            Logger.Error($"Unexpected error. Closing port: {port.PortName}");
            var vm = AvailablePorts.FirstOrDefault(p => p.PortName == port.PortName);
            vm.IsOpen = false;
        }

        private USBPortViewModel CreatePortViewModel(SerialPortModel port)
        {
            var viewModel = container.Resolve<USBPortViewModel>();

            // Copy fields from service
            mapper.Map(port, viewModel);

            // DataMode is a local field. Service knows nothing about it. 
            // Need to set default value
            viewModel.DataMode = "Text";

            return viewModel;
        }

        private USBPortViewModel GetViewModel(SerialPortModel port)
        {
            return AvailablePorts.FirstOrDefault(p => p.PortName == port.PortName);
        }
    }
}
