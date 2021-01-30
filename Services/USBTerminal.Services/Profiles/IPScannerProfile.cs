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
            CreateMap<PingReply, DNSModel>().ConvertUsing(Convert);
        }

        private DNSModel Convert(PingReply source, DNSModel destination, ResolutionContext context)
        {
            destination = destination ?? new DNSModel();
            var hostEntry = Dns.GetHostEntry(source.Address);
            destination.HostName = hostEntry.HostName;
            destination.IP = source.Address.ToString();
            destination.BaseIP = GetBaseIp(destination.IP);
            return destination;
        }
        private string GetBaseIp(string ip)
        {
            var baseIpEnd = ip.LastIndexOf('.');
            return ip.Substring(0, baseIpEnd + 1);
        }
    }
}
