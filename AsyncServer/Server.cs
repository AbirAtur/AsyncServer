using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncServer
{
    class Server
    {
        private readonly TcpListener m_TcpListener = new TcpListener(IPAddress.Loopback, 6666);

        private readonly ConcurrentBag<TcpClient> m_Clients = new ConcurrentBag<TcpClient>();

        public async Task Start()
        {
            m_TcpListener.Start();

            while (true)
            {
                try
                {
                    Trace.WriteLine("Waiting for client");
                    TcpClient tcpClient = await m_TcpListener.AcceptTcpClientAsync();
                    m_Clients.Add(tcpClient);

                    Trace.WriteLine("Client connected !");

                    // ReSharper disable once UnusedVariable
                    var _ = HandleClient(tcpClient);
                }
                catch (SocketException ex)
                {
                    Trace.WriteLine($"Socket exception {ex.SocketErrorCode}: {ex.Message}");
                    return;
                }
            }
        }

        private async Task HandleClient(TcpClient tcpClient)
        {
            NetworkStream networkStream = tcpClient.GetStream();
            IPEndPoint address = tcpClient.Client.RemoteEndPoint as IPEndPoint;
            string endpoint = address?.Port.ToString() ?? "-1";

            while (true)
            {
                try
                {
                    Trace.WriteLine("Waiting for data from client...");
                    byte[] buffer = new byte[1024];
                    int received = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                    if (received <= 0)
                    {
                        Trace.WriteLine($"Client {endpoint} disconnected");
                        return;
                    }

                    Trace.WriteLine($"received message from {endpoint} : {Encoding.ASCII.GetString(buffer, 0, received)}");
                }
                catch (SocketException ex)
                {
                    Trace.WriteLine($"Socket exception {ex.SocketErrorCode}: {ex.Message}");
                    return;
                }
                catch (ObjectDisposedException)
                {
                    Trace.WriteLine($"Connection terminated from client: {endpoint}");
                    return;
                }
            }
        }

        public void Stop()
        {
            m_TcpListener.Stop();
            
            foreach (TcpClient tcpClient in m_Clients)
            {
                tcpClient.Close();
            }
        }

        public async Task SendAllClients(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);

            foreach (TcpClient tcpClient in m_Clients)
            {
                var stream = tcpClient.GetStream();
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
        }
    }
}
