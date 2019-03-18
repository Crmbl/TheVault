using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

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

        private void StartServer(object sender, RoutedEventArgs e)
        {
            AsynchronousSocketListener.StartListening();
        }

        private void SendMessage(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
