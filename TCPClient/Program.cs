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
            string IP = "127.0.0.1";

            // Получаем имя локального компьютера
            string hostName = Dns.GetHostName();
            Console.WriteLine($"Имя хоста: {hostName}");

            // Получаем все IP-адреса, связанные с хостом
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);

            foreach (IPAddress ip in hostEntry.AddressList)
            {
                // Фильтруем, чтобы получить только IPv4
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP = ip.ToString();
                }
            }


            Listener.LIsten();
            Searcher.localIP = IP;
            Searcher.Search();
        }
    }
}
