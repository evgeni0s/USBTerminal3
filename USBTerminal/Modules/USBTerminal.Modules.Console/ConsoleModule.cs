using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using USBTerminal.Core;
using USBTerminal.Modules.Console.ViewModels;
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
            var params1 = new NavigationParameters { { "Title", "Terminal" } };
            _regionManager.RequestNavigate(RegionNames.LeftPanelRegion, nameof(ConsoleView), params1);

            var params2 = new NavigationParameters { { "Title", "Application Logs" } };
            _regionManager.RequestNavigate(RegionNames.RightPanelRegion, nameof(ConsoleView), params2);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ConsoleView>();

            // Register custom new and ViewModel
            //delete this from view 
            //prism: ViewModelLocator.AutoWireViewModel = "True"
            //ViewModelLocationProvider.Register<ConsoleView, ConsoleViewModel>
            //ViewModelLocationProvider.Register<TextBoxSinkVewModel, ConsoleViewModel>();
            //ViewModelLocationProvider.Register<TerminalViewModel, ConsoleViewModel>();
        }
    }
}