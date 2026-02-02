using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    internal class Listener
    {
        const int DISCOVERY_PORT = 40000;
        const int SERVICE_PORT = 50001;
        static public bool running = false;

        static public void LIsten()
        {
            running = true;

            UdpClient udp = new UdpClient(DISCOVERY_PORT);
            Console.WriteLine("Broadcast server started");

            while (running)
            {
                IPEndPoint remote = null;
                byte[] data = udp.Receive(ref remote);
                string msg = Encoding.UTF8.GetString(data);

                if (msg == "DISCOVER")
                {
                    string response = $"HERE:{SERVICE_PORT}";
                    byte[] resp = Encoding.UTF8.GetBytes(response);

                    // ответ ТОЛЬКО отправителю
                    udp.Send(resp, resp.Length, remote);
                }
            }
        }
    }
}
