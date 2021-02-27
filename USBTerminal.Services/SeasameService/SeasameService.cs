using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Core.Interfaces;
using USBTerminal.Services.Interfaces;
using USBTerminal.Services.Interfaces.Models;
using USBTerminal.Services.Interfaces.Models.Network;
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
        private readonly IUSBService usbService;

        public SeasameService(IApplicationCommands commands,
            IEventAggregator eventAggregator,
            ILogger logger,
            ISocketServer socketServer,
            IUSBService usbService)
        {
            this.commands = commands;
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.socketServer = socketServer;
            this.usbService = usbService;
        }

        //public void MoveLeft()
        //{
        //    socketServer.ExecuteSendMessageOnNetworkCommand
        //}

        //public void MoveRight()
        //{

        //}

        public void MoveTo(double percent)
        {
            var activeConnections = socketServer.GetActiveConnections();
            if (!activeConnections.Any())
            {
                logger.Error("Move command failed. No devices are online.");
                return;
            }
            foreach (var connection in activeConnections)
            {
                var message = new NetworkMessage
                {
                    Address = connection,
                    Payload = $"Move {percent}%"
                };

                commands.SendMessageOnNetworkCommand.Execute(message);
            }
        }
    }
}
