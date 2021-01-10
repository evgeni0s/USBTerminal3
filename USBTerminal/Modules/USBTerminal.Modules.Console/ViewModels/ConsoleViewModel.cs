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

namespace USBTerminal.Modules.Console.ViewModels
{
    public class ConsoleViewModel: RegionViewModelBase
    {
        private readonly ILogger logger;
        private readonly IDialogService dialogService;
        private DelegateCommand saveCommand;
        private DelegateCommand clearCommand;
        private string title;
        private IApplicationCommands applicationCommands;

        public ConsoleViewModel(IRegionManager regionManager, 
            ILogger logger, 
            IDialogService dialogService,
            IApplicationCommands applicationCommands,
            IRunFactory runFactory)
            : base(regionManager, logger)
        {
            this.logger = logger;
            this.dialogService = dialogService;
            this.applicationCommands = applicationCommands;
            CustomRichTextBox = new CustomRichTextBox(runFactory);
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
                applicationCommands.LoggingCommand.RegisterCommand(LoggingCommand);
                CustomRichTextBox.IsEnabledCustom = false;
            }
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
