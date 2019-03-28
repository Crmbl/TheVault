using System.Windows;
using System.Windows.Controls;
using TheVault.ViewModels;

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
                var loadingViewModel = loadingControl.DataContext as LoadingViewModel;
                MainControl.Init(loadingViewModel?.Mapping);
                MainControl.Visibility = Visibility.Visible;
            }
        }
    }
}
