using AutoMapper;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services.Profiles
{
    public class IPScannerProfile : Profile
    {
        public IPScannerProfile()
        {
            CreateMap<PingReply, NetworkAddress>().ConvertUsing(Convert);
        }

        private NetworkAddress Convert(PingReply source, NetworkAddress destination, ResolutionContext context)
        {
            destination = destination ?? new NetworkAddress();
            var hostEntry = Dns.GetHostEntry(source.Address);
            destination.HostName = hostEntry.HostName;
            destination.IP = source.Address.ToString();
            return destination;
        }
    }
}
