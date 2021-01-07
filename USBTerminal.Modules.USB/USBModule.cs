using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using USBTerminal.Core;
using USBTerminal.Modules.USB.ViewModels;
using USBTerminal.Modules.USB.Views;

namespace USBTerminal.Modules.USB
{
    public class USBModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public USBModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.BottomPanelRegion, nameof(USBPanelView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<USBPanelView>();
        }
    }
}