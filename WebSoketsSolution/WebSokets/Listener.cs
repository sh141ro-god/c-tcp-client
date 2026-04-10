using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebSokets { 
    internal class Listener
    {
        const int DISCOVERY_PORT = 40000;
        const int SERVICE_PORT = 50001;
        static public bool seeking = false;

        static public void Listen()
        {
            seeking = true;

            UdpClient udp = new UdpClient(DISCOVERY_PORT);
            Console.WriteLine("Broadcast server started");
            while (seeking)
            {
                IPEndPoint remote = null;
                byte[] data = udp.Receive(ref remote);
                string msg = Encoding.UTF8.GetString(data);
                if (msg == "DISCOVER")
                {
                    string response = $"HERE:{SERVICE_PORT}";
                    byte[] resp = Encoding.UTF8.GetBytes(response);
                    udp.Send(resp, resp.Length, remote);
                    seeking = false;
                }
            }

            TcpListener tcp = new TcpListener(IPAddress.Any, SERVICE_PORT);
            tcp.Start();
            Console.WriteLine("TCP server started on port " + SERVICE_PORT);
            while (!seeking)
            {
                TcpClient client = tcp.AcceptTcpClient();
                Console.WriteLine("Client connected: " + ((IPEndPoint)client.Client.RemoteEndPoint).Address);
                NetworkStream s = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytes = s.Read(buffer, 0, buffer.Length);
                string message = Encoding.UTF8.GetString(buffer, 0, bytes);
                Console.WriteLine("Received: " + message);
                client.Close();
            }
            tcp.Stop();
        }
    }
}
