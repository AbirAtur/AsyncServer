using System.Diagnostics;
using System.Windows;

namespace AsyncClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Client m_Client;

        public MainWindow()
        {
            InitializeComponent();

            Trace.Listeners.Add(new TextBoxListener(MessagesTextBox));

            m_Client = new Client();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            m_Client.Disconnect();
        }

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            await m_Client.Connect();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await m_Client.Send(SendTextBox.Text);
        }
    }
}
