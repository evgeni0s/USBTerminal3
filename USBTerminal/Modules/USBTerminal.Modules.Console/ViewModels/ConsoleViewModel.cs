using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using USBTerminal.Modules.Console.Views;
using Prism.Regions;
using USBTerminal.Core.Mvvm;
using Prism.Commands;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using IOPath = System.IO.Path;
using System.Reflection;
using USBTerminal.Core.Interfaces.Console;
using Serilog;
using Serilog.Events;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Enums.Console;
using Prism.Ioc;
using Prism.Events;
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Models;
using USBTerminal.Services.Interfaces.Models.Network;
using USBTerminal.Services.Interfaces.Events.Network;

namespace USBTerminal.Modules.Console.ViewModels
{
    public class ConsoleViewModel: RegionViewModelBase
    {
        private readonly ILogger logger;
        private readonly IDialogService dialogService;
        private readonly IApplicationCommands applicationCommands;
        private readonly IEventAggregator eventAggregator;
        private DelegateCommand saveCommand;
        private DelegateCommand clearCommand;
        private string title;
        private bool showTimestamp;

        public ConsoleViewModel(IRegionManager regionManager, 
            ILogger logger, 
            IDialogService dialogService,
            IApplicationCommands applicationCommands,
            IContainerProvider container,
            IEventAggregator eventAggregator)
            : base(regionManager, logger)
        {
            this.logger = logger;
            this.dialogService = dialogService;
            this.applicationCommands = applicationCommands;
            this.eventAggregator = eventAggregator;
            CustomRichTextBox = container.Resolve<CustomRichTextBox>();
        }

        #region Commands
        private DelegateCommand<LogEvent> loggingCommand;

        public DelegateCommand<LogEvent> LoggingCommand
        {
            get { return loggingCommand ?? (loggingCommand = new DelegateCommand<LogEvent>(ExecuteLoggingCommand)); }
        }

        private void ExecuteLoggingCommand(LogEvent logEvent)
        {
            CustomRichTextBox.SetText(logEvent.RenderMessage(), LevelToRunType(logEvent.Level));
        }
        #endregion

        public CustomRichTextBox CustomRichTextBox { get;  }

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public bool ShowTimestamp
        {
            get { return showTimestamp; }
            set { 
                SetProperty(ref showTimestamp, value);
                CustomRichTextBox.ShowTimeStamp(value);
            }
        }

        public DelegateCommand SaveCommand =>
            saveCommand ?? (saveCommand = new DelegateCommand(ExecuteSaveCommand));

        public DelegateCommand ClearCommand =>
            clearCommand ?? (clearCommand = new DelegateCommand(ExecuteClearCommand));

        private void ExecuteClearCommand()
        {
            CustomRichTextBox.Clear();
            logger.Information($"Cleared text for '{Title}'");
        }

        private void ExecuteSaveCommand()
        {
            var settings = new SaveFileDialogSettings
            {
                Title = "This Is The Title",
                InitialDirectory = IOPath.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                Filter = "Text Documents (*.txt)|*.txt|All Files (*.*)|*.*",
                CheckFileExists = false
            };

            bool? success = dialogService.ShowSaveFileDialog(this, settings);
            if (success == true)
            {
                try
                {
                    File.WriteAllText(settings.FileName, CustomRichTextBox.GetText());
                    Logger.Information("Content from console saved in file.");
                    Process.Start(new ProcessStartInfo(settings.FileName) { UseShellExecute = true });
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error saveing console's content");
                }
            }
        }

        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            Title = navigationContext.Parameters.GetValue<string>(nameof(Title));
            if (Title == "Application Logs")
            {
                // Readonly. No command sending
                applicationCommands.LoggingCommand.RegisterCommand(LoggingCommand);
                CustomRichTextBox.IsEnabledCustom = false;
            }
            else
            {
                // Terminal window
                // Send recieve messages from usb
                eventAggregator.GetEvent<UsbMessageReceivedEvent>().Subscribe(OnReceivedResponseFromUsbDecive);
                eventAggregator.GetEvent<NetworkMessageRecievedEvent>().Subscribe(OnReceivedResponseFromNetwork);
                eventAggregator.GetEvent<NetworkMessageSentEvent>().Subscribe(OnNetworkMessageSentEvent);
            }
        }

        private void OnNetworkMessageSentEvent(NetworkMessage message)
        {
            CustomRichTextBox.SetText($"pc >> {message.Payload}", RunType.Yellow);
        }

        private void OnReceivedResponseFromUsbDecive(SerialPortMessage response)
        {
            CustomRichTextBox.SetText($"device >> {response.TextData}", RunType.Blue);
        }

        private void OnReceivedResponseFromNetwork(NetworkMessage response)
        {
            CustomRichTextBox.SetText($"device >> {response.Payload}", RunType.Blue);
        }

        public RunType LevelToRunType(LogEventLevel logEvent)
        {
            switch (logEvent)
            {
                case LogEventLevel.Debug:
                    return RunType.White;
                case LogEventLevel.Information:
                    return RunType.Green;
                case LogEventLevel.Verbose:
                    return RunType.Blue;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return RunType.Red;
                case LogEventLevel.Warning:
                    return RunType.Orange;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
