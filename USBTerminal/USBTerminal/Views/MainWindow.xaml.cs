using MahApps.Metro.Controls;
using System.Windows;

namespace USBTerminal.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
           // this.HamburgerMenuControl.Content = e.ClickedItem;
           // this.HamburgerMenuControl.IsPaneOpen = false;
        }
    }
}
