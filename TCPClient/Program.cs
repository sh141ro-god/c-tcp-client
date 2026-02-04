using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TCPClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string IP = "172.20.117.2";


            string hostName = Dns.GetHostName();
            Console.WriteLine($"Имя хоста: {hostName}");


            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (IPAddress ip in hostEntry.AddressList)
            {

                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP = ip.ToString();
                }
            }


            //Listener.LIsten();
            Searcher.localIP = IP;
            Searcher.Search();
            Searcher.Print();

            Console.ReadLine();
        }
    }
}
