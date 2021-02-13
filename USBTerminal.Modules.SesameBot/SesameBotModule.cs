using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
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
            //throw new NotImplementedException();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SesamePanel>();
        }
    }
}
