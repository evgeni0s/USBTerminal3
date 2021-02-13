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
            CreateMap<PingReply, NetworkConnection>().ConvertUsing(Convert);
        }

        private NetworkConnection Convert(PingReply source, NetworkConnection destination, ResolutionContext context)
        {
            destination = destination ?? new NetworkConnection();
            var hostEntry = Dns.GetHostEntry(source.Address);
            destination.HostName = hostEntry.HostName;
            destination.IP = source.Address.ToString();
            return destination;
        }
    }
}
