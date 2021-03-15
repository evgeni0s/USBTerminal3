using InWit.WPF.MultiRangeSlider;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace USBTerminal.Modules.SesameBot.Utils
{
    public class SliderEventArgsToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is WitMultiRangeSliderBarClickedEventArgs))
                return Binding.DoNothing;

            return ((WitMultiRangeSliderBarClickedEventArgs)value).Position;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
