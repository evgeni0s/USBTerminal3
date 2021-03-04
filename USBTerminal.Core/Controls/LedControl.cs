using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace USBTerminal.Core.Controls
{
    public class LedControl : CheckBox
    {
        static LedControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LedControl), new FrameworkPropertyMetadata(typeof(LedControl)));
        }

        public static readonly DependencyProperty OnColorProperty =
            DependencyProperty.Register("OnColor", typeof(Brush), typeof(LedControl), new PropertyMetadata(Brushes.Green));

        public Brush OnColor
        {
            get { return (Brush)GetValue(OnColorProperty); }
            set { SetValue(OnColorProperty, value); }
        }

        public static readonly DependencyProperty OffColorProperty =
            DependencyProperty.Register("OffColor", typeof(Brush), typeof(LedControl), new PropertyMetadata(Brushes.Red));

        public Brush OffColor
        {
            get { return (Brush)GetValue(OffColorProperty); }
            set { SetValue(OffColorProperty, value); }
        }
    }
}
