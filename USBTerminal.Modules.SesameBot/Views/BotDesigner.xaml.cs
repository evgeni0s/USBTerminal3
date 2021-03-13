using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using USBTerminal.Modules.SesameBot.ViewModels;

namespace USBTerminal.Modules.SesameBot.Views
{
    /// <summary>
    /// Interaction logic for BotDesigner.xaml
    /// </summary>
    public partial class BotDesigner : UserControl
    {
        public BotDesigner()
        {
            InitializeComponent();
        }
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //(DataContext as BotDesignerViewModel).DeviceDesigner = DeviceDesigner;
        }
    }
}
