using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using USBTerminal.Core;
using USBTerminal.Modules.TestModule.Views;

namespace USBTerminal.Modules.TestModule
{
    public class TestModuleModule : IModule
    {
        private readonly IRegionManager regionManager;

        public TestModuleModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }
        public void OnInitialized(IContainerProvider containerProvider)
        {
            regionManager.RequestNavigate(RegionNames.MainRegion, "ViewA");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewA>();
        }
    }
}