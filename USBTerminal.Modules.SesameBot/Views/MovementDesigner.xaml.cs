using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using USBTerminal.Core.Utils;

namespace USBTerminal.Modules.SesameBot.Views
{
    /// <summary>
    /// Interaction logic for MovementDesigner.xaml
    /// </summary>
    public partial class MovementDesigner : UserControl
    {
        public MovementDesigner()
        {
            InitializeComponent();
        }

        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            Canvas.SetLeft(AddNewItemHint, e.GetPosition((IInputElement)sender).X);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                AddNewItemHint.Visibility = Visibility.Hidden;
                return;
            }
            AddNewItemHint.Visibility = Mouse.DirectlyOver.HasVisualParent<Thumb>() ? Visibility.Hidden : Visibility.Visible;
        }

        private void Slider_MouseEnter(object sender, MouseEventArgs e)
        {
            AddNewItemHint.Visibility = Visibility.Visible;
        }

        private void Slider_MouseLeave(object sender, MouseEventArgs e)
        {

            AddNewItemHint.Visibility = Visibility.Hidden;
        }
    }
}
