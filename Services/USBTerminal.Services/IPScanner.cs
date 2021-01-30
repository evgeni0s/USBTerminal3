using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using USBTerminal.Core.Interfaces;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Models;

namespace USBTerminal.Services
{
    public class IPScanner: IIPScanner
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands commands;
        private readonly ILogger logger;
        private readonly IMapper mapper;
        private DelegateCommand<string> scanNetworkCommand;
        private List<DNSModel> allDns = new List<DNSModel>();

        private CountdownEvent countdown = new CountdownEvent(1);
        private int upCount = 0;
        private object lockObj = new object();
        const bool resolveNames = true;

        public IPScanner(IEventAggregator eventAggregator,
            IApplicationCommands commands,
            ILogger logger,
            IMapper mapper)
        {
            this.eventAggregator = eventAggregator;
            this.commands = commands;
            this.logger = logger;
            this.mapper = mapper;
            commands.ScanNetworkCommand.RegisterCommand(ScanNetworkCommand);
        }

        public DelegateCommand<string> ScanNetworkCommand
        {
            get { return scanNetworkCommand ?? (scanNetworkCommand = new DelegateCommand<string>(ExecuteScanNetworkCommand)); }
        }

        private void ExecuteScanNetworkCommand(string baseIp)
        {
            allDns = new List<DNSModel>();
            PingAsync(GetIpRange(baseIp));
        }

        private void PingCompleted(object sender, PingCompletedEventArgs e)
        {
            var ip = (string)e.UserState;
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                if (resolveNames)
                {
                    string name;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;
                    }
                    catch (SocketException ex)
                    {
                        name = "?";
                        logger.Error($"Socket exception at {ip}");
                        logger.Warning(ex.ToString());
                    }
                    logger.Information($"{ip} ({name}) is up: ({e.Reply.RoundtripTime} ms)");
                    allDns.Add(new DNSModel { HostName = name, IP = GetBaseIp(ip) });
                }
                else
                {
                    logger.Information($"{ip} is up: ({e.Reply.RoundtripTime} ms)");
                }
                lock (lockObj)
                {
                    upCount++;
                }
            }
            else if (e.Reply == null)
            {
                logger.Information("Pinging {0} failed. (Null Reply object?)", ip);
            }
            countdown.Signal();
        }

        private IEnumerable<string> GetIpRange(string baseIp)
            => Enumerable.Range(1, 255).Select(ipSegment => baseIp + ipSegment);

        private async void PingAsync(IEnumerable<string> theListOfIPs)
        {
            var tasks = theListOfIPs.Select(ip => new Ping().SendPingAsync(ip, 2000));
            var results = await Task.WhenAll(tasks);
            var resultsList = results.Where(response => response.Status == IPStatus.Success)
                .Select(response => mapper.Map<DNSModel>(response)).ToList();

            this.eventAggregator.GetEvent<NetworkScanCompletedEvent>().Publish(resultsList);
        }

        public DNSModel GetCurrentMachineInfo()
        {
            var dns = new DNSModel();
            var hostName = Dns.GetHostName();
            var host = Dns.GetHostEntry(hostName);
            var ip = host.AddressList.FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork).ToString();
            dns.IP = ip;
            dns.BaseIP = GetBaseIp(ip);
            dns.HostName = hostName;
            return dns;
        }

        private string GetBaseIp(string ip)
        {
            var baseIpEnd = ip.LastIndexOf('.');
            return ip.Substring(0, baseIpEnd + 1);
        }
    }
}
