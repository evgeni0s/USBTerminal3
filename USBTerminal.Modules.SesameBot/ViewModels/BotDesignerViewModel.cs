using AutoMapper;
using DiagramDesigner;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using USBTerminal.Core.Controls.GridViewCells;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;

namespace USBTerminal.Modules.SesameBot.ViewModels
{
    public class BotDesignerViewModel : RegionViewModelBase//: ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands applicationCommands;
        private readonly IMapper mapper;
        private readonly ILogger logger;
        private string deviceName;
        private DelegateCommand saveCommand;
        private DelegateCommand newDeviceCommand;
        private string saveLocation;
        private DesignerCanvas deviceDesigner;
        private Dictionary<Guid, DeviceDesign> designs; 


        public BotDesignerViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IMapper mapper)
            : base(regionManager, logger)
        {
            this.applicationCommands = applicationCommands;
            this.logger = logger;
            this.mapper = mapper;

            ExecuteNewDeviceCommand();
            // ToDo: move to app settings 
            var rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveLocation = Path.Combine(rootDirectory, "SeasameBot", "Devices");
            deviceName = "SeasameBot_" + DateTime.Now;

            Properties = new ObservableCollection<object>();
            Properties.Add(new ComboboxField() { Name = "MotorType" });
            Properties.Add(new TextField() { Name = "Speed" });
        }
        public DesignerCanvas DeviceDesigner { get => deviceDesigner; set => SetProperty(ref deviceDesigner, value); }

        public ObservableCollection<object> Properties { get; set; }

        public string DeviceName
        {
            get => deviceName;
            set => SetProperty(ref deviceName, value);
        }
        public string SaveLocation
        {
            get => saveLocation;
            set => SetProperty(ref saveLocation, value);
        }

        public DelegateCommand SaveCommand =>
           saveCommand ?? (saveCommand = new DelegateCommand(ExecuteSaveCommand));

        private void ExecuteSaveCommand()
        {
            Directory.CreateDirectory(saveLocation);
        }

        public DelegateCommand NewDeviceCommand =>
           newDeviceCommand ?? (newDeviceCommand = new DelegateCommand(ExecuteNewDeviceCommand));

        private void ExecuteNewDeviceCommand()
        {
            if (DeviceDesigner != null)
            {
                DeviceDesigner.SelectionService.SelectionChanged -= SelectionChanged;
            }
            DeviceDesigner = new DesignerCanvas();
            DeviceDesigner.FocusVisualStyle = null;
            DeviceDesigner.Focusable = true;
            DeviceDesigner.Background = Brushes.AliceBlue;
            DeviceDesigner.SelectionService.SelectionChanged += SelectionChanged;
            RaisePropertyChanged(nameof(DeviceDesigner));
        }

        private void SelectionChanged(List<ISelectable> currentSelection)
        {
            if(currentSelection == null || currentSelection.Count == 0)
            {

            }
        }
    }
}
