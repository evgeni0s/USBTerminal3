using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace USBTerminal.Core.Utils
{
    // Does not work 
    public class MediaElementExtensions: DependencyObject
    {
        // Using a DependencyProperty as the backing store for MoviePosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MoviePositionProperty =
            DependencyProperty.RegisterAttached("MoviePosition", typeof(TimeSpan), typeof(MediaElementExtensions), new PropertyMetadata(default(TimeSpan)));

        public static TimeSpan GetMoviePosition(DependencyObject d)
        {
            return (TimeSpan)d.GetValue(MoviePositionProperty);
        }
        public static void SetMoviePosition(DependencyObject d, TimeSpan value)
        {
            d.SetValue(MoviePositionProperty, value);
        }

        public static readonly DependencyProperty DurationProperty =
    DependencyProperty.RegisterAttached("Duration", typeof(TimeSpan), typeof(MediaElementExtensions), new PropertyMetadata(default(TimeSpan)));

        public static TimeSpan GetDurationPosition(DependencyObject d)
        {
            var mediaElement = d as MediaElement;
            return mediaElement.NaturalDuration.TimeSpan;
        }

        public static void SetDuration(DependencyObject d, TimeSpan value)
        {
           // cant set. it is readonly
        }
    }
}
