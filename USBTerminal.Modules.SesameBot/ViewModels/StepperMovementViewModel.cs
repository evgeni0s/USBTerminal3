using AutoMapper;
using Prism.Events;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Models.SesameBot.Movements;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces.SocketConnection;

namespace USBTerminal.Modules.SesameBot.ViewModels
{
    public class StepperMovementViewModel : ViewModelBase
    {
        //private readonly IEventAggregator eventAggregator;
        //private readonly IApplicationCommands applicationCommands;
        //private readonly IMapper mapper;
        //private ILogger logger;

        private int from;
        private int to;
        private string name;
        private int speed;
        private StepperMoveType stepperMoveType;
        private int steps;

        //public StepperMovementViewModel(ILogger logger,
        //    IApplicationCommands applicationCommands,
        //    IMapper mapper,
        //    IEventAggregator eventAggregator)
        //{
        //    this.applicationCommands = applicationCommands;
        //    this.eventAggregator = eventAggregator;
        //    this.logger = logger;
        //    this.mapper = mapper;
        //}

        public int From
        {
            get => from;
            set => SetProperty(ref from, value);
        }

        public int To
        {
            get => to;
            set => SetProperty(ref to, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public int Speed
        {
            get => speed;
            set => SetProperty(ref speed, value);
        }

        public int Steps
        {
            get => steps;
            set => SetProperty(ref steps, value);
        }

        public StepperMoveType StepperMoveType
        {
            get => stepperMoveType;
            set => SetProperty(ref stepperMoveType, value);
        }

        //public Binding LeftValueBinding
        //{
        //    get { return m_leftValueBinding; }
        //    set
        //    {
        //        m_leftValueBinding = value;
        //        this.FirePropertyChanged();
        //    }
        //}

        //public Binding RightValueBinding
        //{
        //    get { return m_rightValueBinding; }
        //    set
        //    {
        //        m_rightValueBinding = value;
        //        this.FirePropertyChanged();
        //    }
        //}
    }
}
