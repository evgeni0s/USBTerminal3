using MvvmDialogs;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System;
using USBTerminal.Core;
using USBTerminal.Core.Interfaces;
using USBTerminal.Modules.USB.Views;
using USBTerminal.Modules.Wifi.Views;

namespace USBTerminal.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private DelegateCommand<string> navigateCommand;
        private string _title = "USB Terminal";
        private readonly ILogger logger;
        private readonly IDialogService dialogService;
        private readonly IApplicationCommands applicationCommands;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IContainerProvider container;

        public MainWindowViewModel(IRegionManager regionManager,
            ILogger logger,
            IDialogService dialogService,
            IApplicationCommands applicationCommands,
            IContainerProvider container,
            IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.container = container;
            this.logger = logger;
            this.dialogService = dialogService;
            this.applicationCommands = applicationCommands;
            this.eventAggregator = eventAggregator;
        }

        public DelegateCommand<string> NavigateCommand =>
            navigateCommand ?? (navigateCommand = new DelegateCommand<string>(ExecuteNavigateCommand));

        // ToDo: consider to register commands inside each module
        private void ExecuteNavigateCommand(string menuLabel)
        {
            switch (menuLabel)
            {
                case "USB Connection":
                    regionManager.RequestNavigate(RegionNames.BottomPanelRegion, nameof(USBPanelView));
                    break;
                case "Network Connection":
                    regionManager.RequestNavigate(RegionNames.BottomPanelRegion, nameof(NetworkScanner));
                    break;
                //case "Settings":
                //    break;
                default:
                    logger.Error($"Navigate command has failed. Check menu item lable.");
                    break;
            }
        }
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
    }
}
