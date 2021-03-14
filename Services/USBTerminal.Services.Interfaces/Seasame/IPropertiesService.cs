using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Services.Interfaces.Models.SesameBot;
using USBTerminal.Services.Interfaces.Models.SesameBot.Properties;

namespace USBTerminal.Services.Interfaces.Seasame
{
    public interface IPropertiesService
    {
        IComponentProperties Default(MotorType motor, int nextNameIndex);
        IComponentProperties DefaultBoard(int nextNameIndex);
        List<IComponentProperties> GetProperties(string deviceName);
        List<string> AllDevices();

        void Save(string deviceName, List<IComponentProperties> properties);
    }
}
