using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using USBTerminal.Modules.ModuleName;
using USBTerminal.Services;
using USBTerminal.Services.Interfaces;
using USBTerminal.Views;

namespace USBTerminal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
        }
    }
}
