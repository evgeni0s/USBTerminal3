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
            // Since console is reusable but one instance is readonly, need to initialize it slightly differently
            _regionManager.RequestNavigate(RegionNames.LeftPanelRegion, nameof(ConsoleView), NavigationArguments.TerminalParameters);
            _regionManager.RequestNavigate(RegionNames.RightPanelRegion, nameof(ConsoleView), NavigationArguments.LogsParameters);
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