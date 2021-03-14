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
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces.Events.Properties;
using USBTerminal.Services.Interfaces.Models.SesameBot;
using USBTerminal.Services.Interfaces.Seasame;
using USBTerminal.Services.SeasameService;
using USBTerminal.Services.SeasameService.GridViewCells;

namespace USBTerminal.Modules.SesameBot.ViewModels
{
    public class BotDesignerViewModel : RegionViewModelBase//: ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands applicationCommands;
        private readonly IMapper mapper;
        private readonly IPropertiesService propertiesService;
        private readonly ILogger logger;
        private string deviceName;
        private DelegateCommand saveCommand;
        private DelegateCommand newDeviceCommand;
        private string saveLocation;
        private DesignerCanvas deviceDesigner;
        private Dictionary<Guid, ComponentProperties> propertiesCash;
        private List<GridViewField> properties;
        private ObservableCollection<string> devices;
        private string selectedDevice;
        private int nextMotorId = 1;
        private int nextBoardId = 1;

        public BotDesignerViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IMapper mapper,
            IPropertiesService propertiesService,
            IEventAggregator eventAggregator)
            : base(regionManager, logger)
        {
            this.applicationCommands = applicationCommands;
            this.logger = logger;
            this.mapper = mapper;
            this.propertiesService = propertiesService; 
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<NewDeviceConfigurationEvent>().Subscribe(OnConfigurationCreated);
            ExecuteNewDeviceCommand();
            // ToDo: move to app settings 
            var rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveLocation = Path.Combine(rootDirectory, "SeasameBot", "Devices");
            propertiesCash = new Dictionary<Guid, ComponentProperties>();

            Devices = new ObservableCollection<string>(propertiesService.AllDevices());
        }


        public DesignerCanvas DeviceDesigner { get => deviceDesigner; set => SetProperty(ref deviceDesigner, value); }

        public List<GridViewField> Properties { get => properties; set => SetProperty(ref properties, value); }

        public ObservableCollection<string> Devices { get => devices; set => SetProperty(ref devices, value); }

        public string DeviceName
        {
            get => deviceName;
            set
            {
                SetProperty(ref deviceName, value);
                ExecuteOpenDeviceCommand();
            }

        }

        public string SaveLocation
        {
            get => saveLocation;
            set => SetProperty(ref saveLocation, value);
        }

        //public string SelectedDevice
        //{
        //    get => selectedDevice;
        //    set
        //    {
        //        SetProperty(ref selectedDevice, value);
        //        ExecuteOpenDeviceCommand();
        //    }
        //}

        public DelegateCommand SaveCommand =>
           saveCommand ?? (saveCommand = new DelegateCommand(ExecuteSaveCommand));

        private void ExecuteSaveCommand()
        {
            //Directory.CreateDirectory(saveLocation);
            propertiesService.Save(DeviceName, propertiesCash.Values.ToList());
            var saveDirectory = Path.Combine(SaveLocation, deviceName, "schema.json");
            deviceDesigner.Save(saveDirectory);
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

            Properties = new List<GridViewField>();
            deviceName = "SeasameBot_" + DateTime.Now.ToString("yyyy_dd_M_HH_mm_ss"); 
            RaisePropertyChanged(nameof(DeviceName));
            RaisePropertyChanged(nameof(DeviceDesigner));
        }

        private void ExecuteOpenDeviceCommand()
        {
            
            propertiesCash.Clear();
            var existingProperties = propertiesService.GetProperties(deviceName);
            foreach (var prop in existingProperties)
            {
                propertiesCash.Add(prop.Id, prop);
            }
            var schema = Path.Combine(saveLocation, deviceName, "schema.json");
            DeviceDesigner.Open(schema);
            SelectionChanged(new List<ISelectable>());
        }

        private void SelectionChanged(List<ISelectable> currentSelection)
        {

            var designerItems = currentSelection.OfType<DesignerItem>();
            if (currentSelection == null || designerItems.Count() != 1)
            {
                Properties = new List<GridViewField>();
                return;
            }
            var component = designerItems.FirstOrDefault();
            if (propertiesCash.ContainsKey(component.ID))
            {
                Properties = propertiesCash[component.ID].Properties;
                return;
            }
            var componentConent = component.Content as UserControl;
            var componentType = componentConent.ToolTip.ToString();
            if (componentType == "Stepper Motor")
            {
                var newPropertyComponent = propertiesService.Default(MotorType.Stepper, nextMotorId++);
                newPropertyComponent.Id = component.ID;
                propertiesCash.Add(newPropertyComponent.Id, newPropertyComponent);
            }
            else if (componentType == "Stepper Feather Wing")
            {
                var newPropertyComponent = propertiesService.DefaultBoard(nextBoardId++);
                newPropertyComponent.Id = component.ID;
                propertiesCash.Add(newPropertyComponent.Id, newPropertyComponent);
            }
            else
            {
                throw new NotImplementedException();
            }

            Properties = propertiesCash[component.ID].Properties;
            RaisePropertyChanged(nameof(Properties));
        }

        private void OnConfigurationCreated(string name)
        {
            if (!Devices.Contains(name))
            {
                Devices.Add(name);
            }
        }
    }
}
