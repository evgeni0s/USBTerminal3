using AutoMapper;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;

namespace USBTerminal.Modules.SesameBot.ViewModels
{
    public class BotDesignerViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IApplicationCommands applicationCommands;
        private readonly IMapper mapper;
        private readonly ILogger logger;
        public BotDesignerViewModel(ILogger logger,
           IApplicationCommands applicationCommands,
           IMapper mapper)
        {
            this.applicationCommands = applicationCommands;
            this.logger = logger;
            this.mapper = mapper;
        }
    }
}
