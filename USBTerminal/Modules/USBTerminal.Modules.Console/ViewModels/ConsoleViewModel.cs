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

namespace USBTerminal.Modules.Console.ViewModels
{
    public class ConsoleViewModel: RegionViewModelBase
    {
        private readonly IDialogService dialogService;
        private DelegateCommand saveCommand;
        public ConsoleViewModel(IRegionManager regionManager, 
            ILogger logger, 
            IDialogService dialogService,
            ITextBoxLogger textBoxLogger)
            : base(regionManager, logger)
        {
            this.dialogService = dialogService;
            TextBoxLogger = textBoxLogger;
        }

        public ITextBoxLogger TextBoxLogger { get; }

        public DelegateCommand SaveCommand =>
            saveCommand ?? (saveCommand = new DelegateCommand(ExecuteSaveCommand));

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
                    File.WriteAllText(settings.FileName, TextBoxLogger.GetText());
                    Logger.Information("Content from console saved in file.");
                    Process.Start(new ProcessStartInfo(settings.FileName) { UseShellExecute = true });
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error saveing console's content");
                }
            }
        }
    }
}
