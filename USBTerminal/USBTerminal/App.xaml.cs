using AutoMapper;
using ControlzEx.Theming;
using MvvmDialogs;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;
using System.Windows;
using System.Windows.Input;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Interfaces.Console;
using USBTerminal.Modules.Console;
using USBTerminal.Modules.Console.Views;
using USBTerminal.Modules.USB;
using USBTerminal.Modules.USB.ViewModels;
using USBTerminal.Modules.Wifi;
using USBTerminal.Services;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Profiles;
using USBTerminal.Views;
using ApplicationCommands = USBTerminal.Core.ApplicationCommands;

namespace USBTerminal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private IApplicationCommands applicationCommands = new ApplicationCommands();

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDialogService, DialogService>();
            containerRegistry.RegisterSingleton<IUSBService, USBService>();

            containerRegistry.RegisterSingleton<IRunFactory, RunFactory>();
            containerRegistry.Register<USBPortViewModel>();// childe vm
            containerRegistry.Register<CustomRichTextBox>();

            var config = new MapperConfiguration(cfg =>
            {
                // Implement common mappings here. Ex. String to integet, string to boolean
                // ...

                // Profiles are for each module so that it could add extra mappings on top of common
                cfg.AddProfile<USBServiceProfile>();
                cfg.AddProfile<USBModuleProfile>();
            });

            containerRegistry.RegisterSingleton<IConfigurationProvider>(() => config);
            containerRegistry.RegisterScoped<IMapper, Mapper>();

            containerRegistry.RegisterSingleton<IApplicationCommands>(() => applicationCommands);
            //ILoggingFacade
            // Register Serilog with Prism
            // containerRegistry.RegisterSerilog();
            // Prism.Logging.ILoggerFacade
            containerRegistry.RegisterSingleton<ILogger>(() =>  Log.Logger);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ConsoleModule>();
            moduleCatalog.AddModule<USBModule>();
            moduleCatalog.AddModule<WifiModule>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            
            // Configure Serilog and the sinks at the startup of the app
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(path: "USBTerminal.log", rollingInterval: RollingInterval.Day) // TODO: save in my documents, create open log folder
                .WriteTo.Sink(new TextBoxSink(applicationCommands))
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
