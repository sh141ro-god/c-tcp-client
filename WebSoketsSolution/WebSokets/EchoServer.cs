using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace WebSokets
{
    internal class EchoServer
    {
        const int PORT = 55000;
        static private bool running = false;

        static public void Start()
        {
            running = true;

            TcpListener listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();
            Console.WriteLine($"Echo server started on port {PORT}");

            while (running)
            {
                TcpClient client = listener.AcceptTcpClient();
                string clientAddr = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                Console.WriteLine($"[+] Client connected: {clientAddr}");

                Thread thread = new Thread(() => HandleClient(client, clientAddr));
                thread.IsBackground = true;
                thread.Start();
            }

            listener.Stop();
        }

        static private void HandleClient(TcpClient client, string clientAddr)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (client.Connected)
                {
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    if (bytes == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytes);
                    Console.WriteLine($"[{clientAddr}] received: {message}");

                    byte[] response = Encoding.UTF8.GetBytes(message);
                    stream.Write(response, 0, response.Length);
                    Console.WriteLine($"[{clientAddr}] echoed back: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{clientAddr}] error: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine($"[-] Client disconnected: {clientAddr}");
            }
        }
    }
}
