using AutoMapper;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using USBTerminal.Core.Enums;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces.Models;
using USBTerminal.Services.Interfaces.SocketConnection;

namespace USBTerminal.Modules.Wifi.ViewModels
{
    public class NetworkConnectionViewModel : ViewModelBase
    {
        private readonly ISocketServer socketServer;
        private readonly IApplicationCommands applicationCommands;
        private DelegateCommand openConnectionCommand;
        private DelegateCommand closeConnectionCommand;
        private readonly IMapper mapper;
        ILogger logger;
        private string connectionState;
        private string ip;
        private string hostName;
        private string port;

        public NetworkConnectionViewModel(ILogger logger,
            IApplicationCommands applicationCommands,
            IMapper mapper,
            ISocketServer socketServer)
        {
            this.applicationCommands = applicationCommands;
            this.socketServer = socketServer;
            this.logger = logger;
            this.mapper = mapper;
            
            this.ConnectionState = ButtonStates.Default;
            //RaisePropertyChanged(nameof(ConnectionState));
        }

        public DelegateCommand OpenConnectionCommand
        {
            get { return openConnectionCommand ?? (openConnectionCommand = new DelegateCommand(ExecuteOpenConncetionCommand)); }
        }

        private void ExecuteOpenConncetionCommand()
        {
            if (!int.TryParse(Port, out var p))
            {
                logger.Error($"Cannot open network connection. Invalid port {port}");
                return;
            }
            ConnectionState = ButtonStates.Waiting;
            RaisePropertyChanged(nameof(ConnectionState));
            var dto = mapper.Map<NetworkConnection>(this);
            applicationCommands.OpenNetworkConnectionCommand.Execute(dto);
        }

        public DelegateCommand CloseConnectionCommand
        {
            get { return closeConnectionCommand ?? (closeConnectionCommand = new DelegateCommand(ExecuteCloseConncetionCommand)); }
        }

        private void ExecuteCloseConncetionCommand()
        {
            ConnectionState = ButtonStates.Waiting;
            RaisePropertyChanged(nameof(ConnectionState));
            var dto = mapper.Map<NetworkConnection>(this);
            applicationCommands.CloseNetworkConnectionCommand.Execute(dto);
        }

        public string ConnectionState
        {
            get { return connectionState; }
            set
            {
                this.connectionState = value;
                SetProperty(ref connectionState, value);
            }
        }

        public string IP
        {
            get => ip; 
            set
            {
                ip = value;
                SetProperty(ref ip, value);
            }
        }

        public string HostName
        {
            get => hostName; 
            set
            {
                hostName = value;
                SetProperty(ref hostName, value);
            }
        }

        public string Port
        {
            get => port; 
            set
            {
                port = value;
                SetProperty(ref port, value);
            }
        }
    }
}
