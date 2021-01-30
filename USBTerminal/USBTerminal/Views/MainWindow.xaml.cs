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

        private void HamburgerMenuControl_HamburgerButtonClick(object sender, RoutedEventArgs e)
        {
            HumburgerMenuCol.Width = new GridLength(
                HamburgerMenuControl.IsPaneOpen
                ? HamburgerMenuControl.HamburgerWidth
                : 240
            );
        }
    }
}
