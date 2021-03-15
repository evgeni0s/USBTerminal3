using InWit.Core.Collections;
using InWit.WPF.MultiRangeSlider;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces.Seasame;

namespace USBTerminal.Modules.SesameBot.ViewModels
{
    public class MovementDesignerViewModel : RegionViewModelBase
    {
        private readonly ILogger logger;
        private readonly IApplicationCommands applicationCommands;
        private readonly IEventAggregator eventAggregator;
        private readonly ISeasameService seasameService;
        private readonly IContainerProvider container;
        private DelegateCommand<WitMultiRangeSliderBarClickedEventArgs> insertRangeCmd;
        private ViewModelBase selectedRange;
        private ObservableContentCollection<ViewModelBase> rangeItems;
        private double sliderMinimum;
        private double sliderMaximum;

        public MovementDesignerViewModel(IRegionManager regionManager,
        ILogger logger,
        IApplicationCommands applicationCommands,
        IEventAggregator eventAggregator,
        IContainerProvider container,
        ISeasameService seasameService)
        : base(regionManager, logger)
        {
            this.logger = logger;
            this.applicationCommands = applicationCommands;
            this.eventAggregator = eventAggregator;
            this.seasameService = seasameService;
            this.container = container;
            rangeItems = new ObservableContentCollection<ViewModelBase>
                               {
                                   new StepperMovementViewModel {From = 0, To = 13, Name = "BoundRange0"},
                                   new StepperMovementViewModel {From = 13, To = 17, Name = "BoundRange1"},
                               };
            sliderMinimum = 0;
            sliderMaximum = 22;
        }

        public ObservableContentCollection<ViewModelBase> RangeItems
        {
            get => rangeItems;
            set => SetProperty(ref rangeItems, value);
        }
        public ViewModelBase SelectedRange
        {
            get => selectedRange;
            set => SetProperty(ref selectedRange, value);
        }

        public double SliderMinimum
        {
            get => sliderMinimum;
            set => SetProperty(ref sliderMinimum, value);
        }

        public double SliderMaximum
        {
            get => sliderMaximum;
            set => SetProperty(ref sliderMaximum, value);
        }

        public DelegateCommand<WitMultiRangeSliderBarClickedEventArgs> InsertRangeCmd => insertRangeCmd ?? (insertRangeCmd = new DelegateCommand<WitMultiRangeSliderBarClickedEventArgs>(ExecuteInsertRangeCommand));
        private void ExecuteInsertRangeCommand(WitMultiRangeSliderBarClickedEventArgs args)
        {
            InsertRange((int)args.Position);
        }


        private void InsertRange(int level)
        {
            var rangeItemsVms = rangeItems.OfType<StepperMovementViewModel>().ToList();
            if (level > rangeItemsVms.Last().To)
                InsertRightRange(level);
            else if (level < rangeItemsVms.First().From)
                InsertLeftRange(level);
            else
            {
                var previousRange = rangeItemsVms.First(x => x.To >= level);
                var newRange = container.Resolve<StepperMovementViewModel>();
                newRange.From = level;
                newRange.To = previousRange.To;
                newRange.Name = string.Format("Stepper Movement{0}", rangeItems.Count);
                rangeItems.Insert(rangeItems.IndexOf(previousRange) + 1, newRange);

                previousRange.To = level;
            }

        }

        private void InsertRightRange(int level)
        {
            var rightRange = container.Resolve<StepperMovementViewModel>();
            rightRange.From = rangeItems.OfType<StepperMovementViewModel>().Last().To;
            rightRange.To = level;
            rightRange.Name = string.Format("Stepper Movement{0}", rangeItems.Count);

            rangeItems.Add(rightRange);
        }

        private void InsertLeftRange(int level)
        {
            var leftRange = container.Resolve<StepperMovementViewModel>();
            leftRange.From = rangeItems.OfType<StepperMovementViewModel>().First().From;
            leftRange.To = level;
            leftRange.Name = string.Format("Stepper Movement{0}", rangeItems.Count);

            rangeItems.Insert(0, leftRange);
        }
    }
}
