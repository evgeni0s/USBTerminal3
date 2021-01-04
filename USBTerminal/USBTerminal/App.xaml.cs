using ControlzEx.Theming;
using MvvmDialogs;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;
using System.Windows;
using USBTerminal.Core.Interfaces.Console;
using USBTerminal.Modules.Console;
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
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterSingleton<ITextBoxLogger, TextBoxLogger>();

            //ILoggingFacade
            // Register Serilog with Prism
            // containerRegistry.RegisterSerilog();
            // Prism.Logging.ILoggerFacade
            containerRegistry.RegisterSingleton<ILogger>(() =>  Log.Logger);


        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
            moduleCatalog.AddModule<ConsoleModule>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {            
            // Configure Serilog and the sinks at the startup of the app
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(path: "USBTerminal.log", rollingInterval: RollingInterval.Day) // TODO: save in my documents, create open log folder 
                .CreateLogger();

            base.OnStartup(e);

            // Set the application theme to Light.Blue
            ThemeManager.Current.ChangeTheme(this, "Light.Blue");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Flush all Serilog sinks before the app closes
            Log.CloseAndFlush();

            base.OnExit(e);
        }
    }
}
