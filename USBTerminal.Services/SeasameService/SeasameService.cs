using Prism.Commands;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using USBTerminal.Core.Interfaces;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Events;
using USBTerminal.Services.Interfaces.Events.Network;
using USBTerminal.Services.Interfaces.Events.SeasameBot;
using USBTerminal.Services.Interfaces.Models;
using USBTerminal.Services.Interfaces.Models.Network;
using USBTerminal.Services.Interfaces.Network.Events;
using USBTerminal.Services.Interfaces.SeasameConnection;
using USBTerminal.Services.Interfaces.SocketConnection;

namespace USBTerminal.Services.SeasameService
{
    public class SeasameService: ISeasameService
    {
        private readonly IApplicationCommands commands;
        private readonly IEventAggregator eventAggregator;
        private readonly ILogger logger;
        private readonly ISocketServer socketServer;
        private readonly IPScanner ipScanner;
        private readonly IUSBService usbService;
        private const string defaultHostName = "SeasameBot";
        private const string defaultPort = "23";
        private string[] moveDirections = new[] { "Forward", "Backward" };
        private string[] stepperMoveTypes = new[] { "Single", "Double", "Interleave", "Microstep" };
        private string defaultSteps = "200";
        private string defaultSpeed = "10";

        private DelegateCommand searchBotsCommand;
        private NetworkAddress botAddress;

        public SeasameService(IApplicationCommands commands,
            IEventAggregator eventAggregator,
            ILogger logger,
            IPScanner ipScanner,
            ISocketServer socketServer,
            IUSBService usbService)
        {
            this.commands = commands;
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.socketServer = socketServer;
            this.usbService = usbService;
            this.ipScanner = ipScanner;

            commands.SearchSeasameBotsOnNetworkCommand.RegisterCommand(SearchBotsCommand);
            //var activeConnections = socketServer.GetActiveConnections();
            //activeConnections.
            this.eventAggregator.GetEvent<NetworkScanCompletedEvent>().Subscribe(OnNetworkScanCompleted);
            this.eventAggregator.GetEvent<ConnectionSuccessEvent>().Subscribe(OnConnectionSuccessEvent);
            this.eventAggregator.GetEvent<ConnectionFailedEvent>().Subscribe(OnConnectionFailedEvent);
            this.eventAggregator.GetEvent<DeviceDisconnectedEvent>().Subscribe(OnDeviceDisconnectedEvent);
            eventAggregator.GetEvent<TerminalInputEvent>().Subscribe(OnTerminalInput);
            Timer aTimer = new Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            aTimer.Interval = 3000;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (botAddress != null)
            {
                ipScanner.IsAlive(botAddress.IP);
            }
        }

        private void OnDeviceDisconnectedEvent(string ip)
        {
            if (botAddress != null && botAddress.IP == ip)
            {
                logger.Information($"{botAddress.HostName} disconnected.");
                this.eventAggregator.GetEvent<BotConnectionFailedEvent>().Publish(botAddress);
                this.socketServer.ExecuteCloseConnectionCommand(botAddress);
                botAddress = null;
            }
        }


        private void OnTerminalInput(string cmd)
        {
            var message = new NetworkMessage
            {
                Address = botAddress,
                Payload = cmd
            };

            commands.SendMessageOnNetworkCommand.Execute(message);
        }

        private void OnConnectionFailedEvent(ConnectionError args)
        {
            this.eventAggregator.GetEvent<BotConnectionFailedEvent>().Publish(args.Address);
        }

        private void OnConnectionSuccessEvent(NetworkAddress dns)
        {
            if (dns.HostName.Contains(defaultHostName))
            {
                botAddress = dns;
                this.eventAggregator.GetEvent<BotConnectionSuccessEvent>().Publish(dns);
            }
        }

        private void OnNetworkScanCompleted(List<NetworkAddress> allDns)
        {
            var bots = allDns.Where(SeasameBotFilter);
            if (bots.Count() > 1)
            {
                logger.Error("There is more then 1 bot online. Could not decide which to connect.");
                return;
            }
            if (bots.Count() == 0)
            {
                logger.Error("No Seasame Bots were found");
                this.eventAggregator.GetEvent<BotNotFoundEvent>().Publish();
                return;
            }

            var bot = bots.First();
            if (!isRegistered(bot))
            {
                bot.Port = defaultPort;
                logger.Information("Found new Seasame Bot.");
                socketServer.ExecuteConnectOverNetworkCommand(bot);
            }
        }

        private bool isRegistered(NetworkAddress address)
        {
            return botAddress != null && address.IP == botAddress.IP;
        }

        private static bool SeasameBotFilter(NetworkAddress dns)
        {
            return dns.HostName.Contains(defaultHostName);
        }

        public DelegateCommand SearchBotsCommand
        {
            get { return searchBotsCommand ?? (searchBotsCommand = new DelegateCommand(ExecuteSearchBotsCommand)); }
        }

        private void ExecuteSearchBotsCommand()
        {
            logger.Information("Searching for Seasame Bots using Wifi");
            //if (ipScanner.GetAllDns().Count == 0)
            //{
                var currentMachine = ipScanner.GetCurrentMachineInfo();
                commands.ScanNetworkCommand.Execute(currentMachine.BaseIP);
            //}
        }

        //private void OnNetworkScanCompleted(List<NetworkAddress> addresses)
        //{

        //}

        //public void MoveLeft()
        //{
        //    socketServer.ExecuteSendMessageOnNetworkCommand
        //}

        //public void MoveRight()
        //{

        //}

        public void MoveTo(double percent)
        {
            var activeConnections = socketServer.GetActiveConnections().Where(SeasameBotFilter);
            if (!activeConnections.Any())
            {
                logger.Error("Move command failed. No devices are online.");
                return;
            }
            var message = new NetworkMessage
            {
                Address = botAddress,
                Payload = $"Move {percent}%"
            };

            commands.SendMessageOnNetworkCommand.Execute(message);
        }

        public void GetWiFiInfo()
        {
            var activeConnections = socketServer.GetActiveConnections().Where(SeasameBotFilter);
            if (!activeConnections.Any())
            {
                logger.Error("Move command failed. No devices are online.");
                return;
            }
            var message = new NetworkMessage
            {
                Address = botAddress,
                Payload = $"status"
            };

            commands.SendMessageOnNetworkCommand.Execute(message);
        }
    }
}
