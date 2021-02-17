using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;

namespace USBTerminal.Modules.SesameBot.ViewModels
{
    public class SesamePanelViewModel : RegionViewModelBase
    {
        private DelegateCommand moveLeftCommand;
        private DelegateCommand moveRightCommand;
        private DelegateCommand slideCompletedCommand;
        private TimeSpan curtainPosition;
        private TimeSpan curtainMovieDuration;
        private Uri animationPath;

        private readonly ILogger logger;
        private readonly IApplicationCommands applicationCommands;
        private readonly IEventAggregator eventAggregator;
        private double currentProgress;

        public SesamePanelViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator,
            IAbsoluteResourcePathHelper pathHelper)
            : base(regionManager, logger)
        {
            this.logger = logger;
            this.applicationCommands = applicationCommands;
            this.eventAggregator = eventAggregator;
            AnimationPath = pathHelper.GetAbsolutePath("CurtainAnimation.mp4");
        }

        public DelegateCommand MoveLeftCommand =>
            moveLeftCommand ?? (moveLeftCommand = new DelegateCommand(ExecuteMoveLeftCommand));

        private void ExecuteMoveLeftCommand()
        {
            // var test = FromPercentage(20);
            logger.Information($"Move Left execute {CurrentProgress}");

        }

        public DelegateCommand MoveRightCommand =>
            moveRightCommand ?? (moveRightCommand = new DelegateCommand(ExecuteMoveRightCommand));

        private void ExecuteMoveRightCommand()
        {
            logger.Information($"Move Right execute {CurrentProgress}");
            //var test = FromPercentage(41.23);
        }

        public DelegateCommand SlideCompletedCommand =>
            slideCompletedCommand ?? (slideCompletedCommand = new DelegateCommand(ExecuteSlideCompletedCommand));

        private void ExecuteSlideCompletedCommand()
        {
            logger.Information($"Slider moved {CurrentProgress}");
            //var test = FromPercentage(41.23);
        }

        private TimeSpan FromPercentage(double percentage)
        {
            var duration = CurtainMovieDuration.TotalMilliseconds;
            var position = CurtainPosition.TotalMilliseconds;
            var onePercent = duration / position;
            var resultMs = onePercent * percentage;
            return new TimeSpan((long)resultMs);
        }

        //private static TimeSpan GetVideoDuration(string filePath)
        //{
        //    using (var shell = ShellObject.FromParsingName(filePath))
        //    {
        //        IShellProperty prop = shell.Properties.System.Media.Duration;
        //        var t = (ulong)prop.ValueAsObject;
        //        return TimeSpan.FromTicks((long)t);
        //    }
        //}

        public Uri AnimationPath
        {
            get { return animationPath; }
            set { SetProperty(ref animationPath, value); }
        }


        public TimeSpan CurtainPosition
        {
            get { return curtainPosition; }
            set { SetProperty(ref curtainPosition, value); }
        }

        public TimeSpan CurtainMovieDuration
        {
            get { return curtainMovieDuration; }
            set { SetProperty(ref curtainMovieDuration, value); }
        }

        public double CurrentProgress
        {
            get { return currentProgress; }
            set { SetProperty(ref currentProgress, value); }
        }

    }
}
