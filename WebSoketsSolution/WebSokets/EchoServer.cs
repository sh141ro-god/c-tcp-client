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
        const int DISCOVERY_PORT = 40000;
        static private bool running = false;

        static public void Start()
        {
            running = true;

            // UDP обнаружение в отдельном потоке
            Thread udpThread = new Thread(ListenDiscovery);
            udpThread.IsBackground = true;
            udpThread.Start();

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

        static private void ListenDiscovery()
        {
            UdpClient udp = new UdpClient(DISCOVERY_PORT);
            Console.WriteLine($"[UDP] Discovery listener started on port {DISCOVERY_PORT}");

            while (running)
            {
                try
                {
                    udp.Client.ReceiveTimeout = 1000;
                    IPEndPoint remote = null;

                    byte[] data;
                    try { data = udp.Receive(ref remote); }
                    catch (SocketException) { continue; } // таймаут Ч продолжаем

                    string msg = Encoding.UTF8.GetString(data);
                    if (msg == "DISCOVER")
                    {
                        Console.WriteLine($"[UDP] DISCOVER от {remote.Address}");
                        string response = $"HERE:{PORT}"; // отвечаем портом EchoServer
                        byte[] resp = Encoding.UTF8.GetBytes(response);
                        udp.Send(resp, resp.Length, remote);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UDP] ќшибка: {ex.Message}");
                }
            }

            udp.Close();
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