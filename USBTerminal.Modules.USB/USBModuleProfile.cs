using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Modules.USB.ViewModels;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Modules.USB
{
    public class USBModuleProfile: Profile
    {
        public USBModuleProfile()
        {
            CreateMap<SerialPortModel, USBPortViewModel>();
            CreateMap<USBPortViewModel, SerialPortModel>();
        }
    }
}
