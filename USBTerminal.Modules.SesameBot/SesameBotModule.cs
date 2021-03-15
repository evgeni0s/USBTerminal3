using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using USBTerminal.Core;
using USBTerminal.Modules.SesameBot.Views;

namespace USBTerminal.Modules.SesameBot
{
    public class SesameBotModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public SesameBotModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            //_regionManager.RequestNavigate(RegionNames.LeftPanelRegion, nameof(ConsoleView), NavigationArguments.TerminalParameters);
            //_regionManager.RequestNavigate(RegionNames.RightPanelRegion, nameof(ConsoleView), NavigationArguments.LogsParameters);
            //_regionManager.RequestNavigate(RegionNames.BottomPanelRegion, nameof(SesamePanel));
            //_regionManager.RequestNavigate(RegionNames.MainRegion, nameof(BotDesigner));
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(MovementDesigner));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SesamePanel>();
            containerRegistry.RegisterForNavigation<BotDesigner>();
            containerRegistry.RegisterForNavigation<MovementDesigner>();
        }
    }
}
