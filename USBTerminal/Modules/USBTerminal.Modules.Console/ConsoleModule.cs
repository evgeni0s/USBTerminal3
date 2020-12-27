using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using USBTerminal.Core;
using USBTerminal.Modules.Console.Views;

namespace USBTerminal.Modules.Console
{
    public class ConsoleModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ConsoleModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(ConsoleView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ConsoleView>();
        }
    }
}