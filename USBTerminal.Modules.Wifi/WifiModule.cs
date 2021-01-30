using Prism.Commands;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using USBTerminal.Core;
using USBTerminal.Core.Interfaces;
using USBTerminal.Modules.Wifi.Views;

namespace USBTerminal.Modules.Wifi
{
    public class WifiModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IApplicationCommands applicationCommands;
        private DelegateCommand openWiFiCommand;

        public WifiModule(IRegionManager regionManager, IApplicationCommands applicationCommands)
        {
            _regionManager = regionManager;
            this.applicationCommands = applicationCommands;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {

            //_regionManager.RequestNavigate(RegionNames.BottomPanelRegion, nameof(NetworkScanner));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NetworkScanner>();
        }
    }
}