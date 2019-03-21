using TheVault.ViewModels;

namespace TheVault.Views
{
    /// <summary>
    /// Interaction logic for MainControl.xaml
    /// </summary>
    public partial class MainUserControl
    {
        public MainUserControl()
        {
            InitializeComponent();
        }

        public void Init()
        {
            DataContext = new MainViewModel();
        }
    }
}
