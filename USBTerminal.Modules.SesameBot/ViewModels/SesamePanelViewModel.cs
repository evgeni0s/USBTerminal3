using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Serilog;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using USBTerminal.Core.Enums;
using USBTerminal.Core.Enums.SesameBot;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;
using USBTerminal.Services.Interfaces.Events.Network;
using USBTerminal.Services.Interfaces.Events.SeasameBot;
using USBTerminal.Services.Interfaces.Models;
using USBTerminal.Services.Interfaces.Seasame;

namespace USBTerminal.Modules.SesameBot.ViewModels
{
    // System.Drawing.Image requires NuGet System.Drawing.Common
    // gif can be created here https://ezgif.com/maker/ezgif-6-2072aea0d43b-jpg. During creation select Frames. This will provide control over quality
    public class SesamePanelViewModel : RegionViewModelBase
    {
        private DelegateCommand moveLeftCommand;
        private DelegateCommand moveRightCommand;
        private DelegateCommand slideCompletedCommand;
        private DelegateCommand searchCommand;
        private IApplicationCommands applicationCommands;
        private TimeSpan curtainPosition;
        private TimeSpan curtainMovieDuration;
        private string animationPath;
        private readonly ILogger logger;
        private readonly IEventAggregator eventAggregator;
        private readonly ISeasameService seasameService;
        private double currentProgress;
        private const double step = 1;
        private ImageSource curtainFrame;
        private GifBitmapDecoder decoder;
        private int sliderMaximum;
        private string connectionState;

        public SesamePanelViewModel(IRegionManager regionManager,
            ILogger logger,
            IApplicationCommands applicationCommands,
            IEventAggregator eventAggregator,
            IAbsoluteResourcePathHelper pathHelper,
            ISeasameService seasameService)
            : base(regionManager, logger)
        {
            this.logger = logger;
            this.applicationCommands = applicationCommands;
            this.eventAggregator = eventAggregator;
            this.seasameService = seasameService;
            AnimationPath = pathHelper.GetAbsolutePath("CurtainAnimation.gif");


            decoder = new GifBitmapDecoder(new Uri(AnimationPath), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnLoad);
            sliderMaximum = decoder.Frames.Count;
            CurtainFrame = decoder.Frames[0]; // start with not empty control
            this.eventAggregator.GetEvent<BotNotFoundEvent>().Subscribe(OnNotFound);
            this.eventAggregator.GetEvent<BotConnectionFailedEvent>().Subscribe(OnConnectionFailed);
            this.eventAggregator.GetEvent<BotConnectionSuccessEvent>().Subscribe(OnConnectionSuccess);
            this.eventAggregator.GetEvent<BotDisconnectedEvent>().Subscribe(OnDisconnected);

            ExecuteSearchCommand();
        }

        private void OnDisconnected(NetworkAddress address)
        {
            ConnectionState = ConnectionStatuses.Disconnected;
            RaisePropertyChanged(nameof(ConnectionState));
        }

        private void OnConnectionSuccess(NetworkAddress address)
        {
            ConnectionState = ConnectionStatuses.Connected;
            RaisePropertyChanged(nameof(ConnectionState));
            seasameService.GetWiFiInfo();
        }

        private void OnConnectionFailed(NetworkAddress address)
        {
            ConnectionState = ConnectionStatuses.Disconnected;
            RaisePropertyChanged(nameof(ConnectionState));
        }

        private void OnNotFound()
        {
            ConnectionState = ConnectionStatuses.Disconnected;
            RaisePropertyChanged(nameof(ConnectionState));
        }

        public DelegateCommand SearchCommand => searchCommand ?? (searchCommand = new DelegateCommand(ExecuteSearchCommand));
        private void ExecuteSearchCommand()
        {
            ConnectionState = ConnectionStatuses.Search;
            applicationCommands.SearchSeasameBotsOnNetworkCommand.Execute(null);
        }

        public DelegateCommand MoveLeftCommand =>
            moveLeftCommand ?? (moveLeftCommand = new DelegateCommand(ExecuteMoveLeftCommand));

        private void ExecuteMoveLeftCommand()
        {
            logger.Information($"Move Left execute {CurrentProgress}");
            CurrentProgress -= step;
            seasameService.MoveTo(CurrentProgress);
        }

        public ImageSource CurtainFrame
        {
            get => curtainFrame;
            set => SetProperty(ref curtainFrame, value);
        }

        public DelegateCommand MoveRightCommand =>
            moveRightCommand ?? (moveRightCommand = new DelegateCommand(ExecuteMoveRightCommand));

        private void ExecuteMoveRightCommand()
        {
            logger.Information($"Move Right execute {CurrentProgress}");
            CurrentProgress += step;
            seasameService.MoveTo(CurrentProgress);
        }

        public static void SaveImageToFile(BitmapFrame image, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }

        public DelegateCommand SlideCompletedCommand =>
            slideCompletedCommand ?? (slideCompletedCommand = new DelegateCommand(ExecuteSlideCompletedCommand));

        private void ExecuteSlideCompletedCommand()
        {
            logger.Information($"Slider moved {CurrentProgress} / {GetPercentValue()} %");
            seasameService.MoveTo(GetPercentValue());
        }

        public string AnimationPath
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
            set
            {
                if (value < SliderMaximum && value >= 0)
                { 
                    SetProperty(ref currentProgress, value);
                    CurtainFrame = decoder.Frames[(int)value];
                }
            }
        }

        public int SliderMaximum
        {
            get { return sliderMaximum; }
            set
            {
                SetProperty(ref sliderMaximum, value);
            }
        }

        public string ConnectionState
        {
            get { return connectionState; }
            set
            {
                this.connectionState = value;
               // RaisePropertyChanged(nameof(ConnectionState));
                SetProperty(ref connectionState, value);
            }
        }

        private double GetPercentValue()
        {
           return CurrentProgress / sliderMaximum  * 100;
        }
    }
}
