using AutoMapper;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using USBTerminal.Core.Interfaces;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services
{
    public class IPScanner: IIPScanner
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands commands;
        private readonly ILogger logger;
        private readonly IMapper mapper;

        public IPScanner(IEventAggregator eventAggregator,
            IApplicationCommands commands,
            ILogger logger,
            IMapper mapper)
        {
            this.eventAggregator = eventAggregator;
            this.commands = commands;
            this.logger = logger;
            this.mapper = mapper;
        }

        public List<DNSModel> GetAllDns()
        {
            throw new NotImplementedException();
        }

        public DNSModel GetCurrentMachineInfo()
        {
            var StringHost = System.Net.Dns.GetHostName();
            var StrIP = Dns.GetHostByName(StringHost).AddressList[2].ToString();
            return null;
        }
    }
}
