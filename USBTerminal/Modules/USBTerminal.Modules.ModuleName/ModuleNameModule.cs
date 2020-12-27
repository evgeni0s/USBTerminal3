using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using USBTerminal.Core;
using USBTerminal.Modules.ModuleName.Views;

namespace USBTerminal.Modules.ModuleName
{
    public class ModuleNameModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleNameModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
           // _regionManager.RequestNavigate(RegionNames.MainRegion, "ViewA");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}