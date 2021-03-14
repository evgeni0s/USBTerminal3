using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models.SesameBot;
using USBTerminal.Services.SeasameService;

namespace USBTerminal.Services.Interfaces.Seasame
{
    public interface IPropertiesService
    {
        ComponentProperties Default(MotorType motor, int nextNameIndex);
        ComponentProperties DefaultBoard(int nextNameIndex);
        List<ComponentProperties> GetProperties(string deviceName);
        List<string> AllDevices();

        void Save(string deviceName, List<ComponentProperties> properties);
    }
}
