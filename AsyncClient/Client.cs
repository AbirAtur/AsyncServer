using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AsyncClient
{
    class Client
    {
        private TcpClient m_TcpClient = new TcpClient(AddressFamily.InterNetwork);

        public async Task Connect()
        {
            m_TcpClient.Close();
            m_TcpClient = new TcpClient(AddressFamily.InterNetwork);

            await m_TcpClient.ConnectAsync(IPAddress.Loopback, 6666);
            Trace.WriteLine("Connected to server !");

            NetworkStream networkStream = m_TcpClient.GetStream();

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int received = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    if (received <= 0)
                    {
                        Trace.WriteLine("Server disconnected");
                        return;
                    }

                    Trace.WriteLine($"received message from server : {Encoding.ASCII.GetString(buffer, 0, received)}");
                }
                catch (SocketException ex)
                {
                    Trace.WriteLine($"Socket exception {ex.SocketErrorCode}: {ex.Message}");
                    return;
                }
                catch (ObjectDisposedException)
                {
                    Trace.WriteLine("Connection terminated");
                    return;
                }
            }
        }

        public void Disconnect()
        {
            m_TcpClient.Close();
        }

        public async Task Send(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            NetworkStream networkStream = m_TcpClient.GetStream();

            await networkStream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}
