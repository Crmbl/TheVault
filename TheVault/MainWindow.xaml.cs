using System.Windows;
using System.Windows.Controls;

namespace TheVault
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UserControl loginControl && loginControl.Visibility == Visibility.Collapsed)
                MainControl.Visibility = Visibility.Visible;
        }
    }
}
