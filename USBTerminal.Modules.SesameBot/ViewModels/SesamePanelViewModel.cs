using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Serilog;
using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using USBTerminal.Core.Interfaces;
using USBTerminal.Core.Mvvm;

namespace USBTerminal.Modules.SesameBot.ViewModels
{
    // System.Drawing.Image requires NuGet System.Drawing.Common
    // Continue with createing good gif https://ezgif.com/maker/ezgif-6-2072aea0d43b-jpg. During creation select Frames. This will provide control over quality
    public class SesamePanelViewModel : RegionViewModelBase
    {
        private DelegateCommand moveLeftCommand;
        private DelegateCommand moveRightCommand;
        private DelegateCommand slideCompletedCommand;
        private TimeSpan curtainPosition;
        private TimeSpan curtainMovieDuration;
        private string animationPath;
        private readonly ILogger logger;
        private readonly IApplicationCommands applicationCommands;
        private readonly IEventAggregator eventAggregator;
        private double currentProgress;
        private const double step = 1;
        private ImageSource curtainFrame;
        private GifBitmapDecoder decoder;
        private int sliderMaximum;

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
            AnimationPath = pathHelper.GetAbsolutePath("CurtainAnimation.gif");


            decoder = new GifBitmapDecoder(new Uri(AnimationPath), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnLoad);
            sliderMaximum = decoder.Frames.Count;
            CurtainFrame = decoder.Frames[0]; // start with not empty control
        }

        public DelegateCommand MoveLeftCommand =>
            moveLeftCommand ?? (moveLeftCommand = new DelegateCommand(ExecuteMoveLeftCommand));

        private void ExecuteMoveLeftCommand()
        {
            logger.Information($"Move Left execute {CurrentProgress}");
            CurrentProgress -= step;
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
            logger.Information($"Slider moved {CurrentProgress}");
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
    }
}
