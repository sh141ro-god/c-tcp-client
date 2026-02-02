using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    internal class Searcher
    {
        const int DISCOVERY_PORT = 40000;
        static public Dictionary<string, int> found = new Dictionary<string, int>();
        static public string localIP = "";

        static public void Search()
        {
            byte[] discover = Encoding.UTF8.GetBytes("DISCOVER");
            for (int i = 1; i <= 254; i++)
            {
                string ip = $"192.168.10.{i}";
                IPEndPoint target = new IPEndPoint(IPAddress.Parse(ip), DISCOVERY_PORT);
                UdpClient udp = new UdpClient();
                udp.Client.ReceiveTimeout = 5000; 

                udp.Send(discover, discover.Length, target);

                IPEndPoint remote = null;
                byte[] resp = udp.Receive(ref remote);
                string msg = Encoding.UTF8.GetString(resp);

                if (msg.StartsWith("HERE:"))
                {
                    int port = int.Parse(msg.Substring(5));
                    found[remote.Address.ToString()] = port;
                }
            }
        }
        static public void Print()
        {
            int i= 0;
            foreach (var item in found)
            {
                i++;
                Console.WriteLine($"{i}|{item.Key}:{item.Value}");
            }
        }
    }
}
