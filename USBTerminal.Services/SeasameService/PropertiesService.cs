using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Events.Properties;
using USBTerminal.Services.Interfaces.Models.SesameBot;
using USBTerminal.Services.Interfaces.Seasame;
using USBTerminal.Services.SeasameService.GridViewCells;

namespace USBTerminal.Services.SeasameService
{
    public class PropertiesService: IPropertiesService
    {
        private readonly string saveLocation;
       // private readonly string subDir = "Properties";
        private readonly IEventAggregator eventAggregator;

        public PropertiesService(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            // ToDo: move to app settings 
            var rootDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveLocation = Path.Combine(rootDirectory, "SeasameBot", "Devices");
            Directory.CreateDirectory(saveLocation);
        }

        public List<ComponentProperties> GetProperties(string deviceName)
        {
            var propertiesPath = Path.Combine(saveLocation, deviceName, "properties.json");
            var  JSONtxt = File.ReadAllText(propertiesPath);
            var properties = JsonConvert.DeserializeObject<List<ComponentProperties>>(JSONtxt);
            return new List<ComponentProperties>(properties);
        }

        public ComponentProperties Default(MotorType motor, int nextNameIndex)
        {
            var p = new ComponentProperties
            {
                Properties = new List<GridViewField>() {
                    new TextField() { Name = "Type", Value = Enum.GetName(typeof(MotorType), motor), IsReadOnly = true },
                    new TextField() { Name = "Default Speed (RPM)", Value = "10" },
                }
            };
            if (motor == MotorType.Stepper)
            {
                p.Properties.Add(new TextField() { Name = "Name", Value = $"StepMotor {nextNameIndex}" });
                p.Properties.Add(new TextField() { Name = "Steps", Value = "200"});
            }
            p.Properties.Sort();
            return p;
        }

        public ComponentProperties DefaultBoard(int nextNameIndex)
        {
            var p = new ComponentProperties
            {
                Properties = new List<GridViewField>() {
                    new TextField() { Name = "Name", Value = $"MotorShield {nextNameIndex}" },
                    new TextField() { Name = "Board", Value = "Stepper Feather Wing", IsReadOnly = true },
                    new TextField() { Name = "Ports", Value = "6", IsReadOnly = true }
                }
            };
            p.Properties.Sort();
            return p;
        }

        public List<string> AllDevices()
        {
            var devicesFolder = Directory.GetDirectories(saveLocation).Select(Path.GetFileName)
                            .ToList();
            return devicesFolder;
        }

        public void Save(string deviceName, List<ComponentProperties> properties)
        {
            var propertiesPath = Path.Combine(saveLocation, deviceName);
            var propertiesDirectory = Directory.CreateDirectory(propertiesPath);

            //JsonConvert.SerializeObject(properties,
            //                                            Formatting.Indented,
            //                                            new JsonSerializerSettings
            //                                            {
            //                                                NullValueHandling = NullValueHandling.Ignore
            //                                            });
            foreach (FileInfo file in propertiesDirectory.EnumerateFiles())
            {
                file.Delete();
            }
            // serialize JSON directly to a file
            var proeprtiesFile = Path.Combine(propertiesPath, $"properties.json");
            using (StreamWriter file = File.CreateText(proeprtiesFile))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, properties);
            }
            eventAggregator.GetEvent<NewDeviceConfigurationEvent>().Publish(deviceName);
        }
    }
}
