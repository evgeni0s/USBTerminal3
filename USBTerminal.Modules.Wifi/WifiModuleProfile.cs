using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Modules.Wifi.ViewModels;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Modules.Wifi
{
    public class WifiModuleProfile: Profile
    {
        public WifiModuleProfile()
        {
            CreateMap<NetworkConnection, NetworkConnectionViewModel>();
            CreateMap<NetworkConnectionViewModel, NetworkConnection>();
        }
    }
}
