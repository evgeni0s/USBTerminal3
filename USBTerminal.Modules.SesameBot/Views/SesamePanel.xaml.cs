using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using USBTerminal.Core.Utils;

namespace USBTerminal.Modules.SesameBot.Views
{
    /// <summary>
    /// Interaction logic for ControlPanel
    /// </summary>
    public partial class SesamePanel : UserControl
    {
        public SesamePanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialise UI elements based on current media item
        /// </summary>
        private void mediaPlayerMain_MediaOpened(object sender, RoutedEventArgs e)
        {
            //sliderTime.Maximum = mediaPlayerMain.NaturalDuration.TimeSpan.TotalMilliseconds;
            //sliderTime.IsEnabled = mediaPlayerMain.IsLoaded;
            //sliderVolume.IsEnabled = mediaPlayerMain.IsLoaded;
            var timeSpan = mediaPlayer.NaturalDuration.TimeSpan;
        }

        private void mediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            //mediaPlayer.Play();
            //var videoPath = Directory.GetCurrentDirectory();
            //var path = AbsoluteResourcePathHelper.GetAbsolutePath("CurtainAnimation.mp4");

            // mediaPlayer.Source = new Uri(videoPath + @"../../../../component/Resources/CurtainAnimation.mp4", UriKind.Relative);
            //mediaPlayer.Source = path;
            //var exists = File.Exists(mediaPlayer.Source.ToString());
            //mediaPlayer.Play();
        }

    }
}
