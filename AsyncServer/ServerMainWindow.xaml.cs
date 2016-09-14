using System.Diagnostics;
using System.Windows;

namespace AsyncServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServerMainWindow : Window
    {
        private readonly Server m_Server;

        public ServerMainWindow()
        {
            InitializeComponent();

            Trace.Listeners.Add(new TextBoxListener(TextBoxMessages));

            m_Server = new Server();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Starting server...");

            await m_Server.Start();

            Trace.WriteLine("Server started");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Sending hi to all clients...");

            await m_Server.SendAllClients("Hi !!!");

            Trace.WriteLine("Done sending");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("Stopping server...");
            m_Server.Stop();
        }
    }
}
