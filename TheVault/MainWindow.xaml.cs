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
            {
                LoadingControl.Init();
                LoadingControl.Visibility = Visibility.Visible;
            }
        }

        private void LoadingControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is UserControl loadingControl && loadingControl.Visibility == Visibility.Collapsed)
            {
                MainControl.Init();
                MainControl.Visibility = Visibility.Visible;
            }
        }
    }
}
