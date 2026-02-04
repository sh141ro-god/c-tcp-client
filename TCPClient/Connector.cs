using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TCPClient
{
    internal class Connector
    {
        static public string IP = "127.0.0.1";
        static public TcpClient mainConnection;
        static public bool running = false;

        static Dictionary<string, TcpClient> conections = new Dictionary<string, TcpClient>();

        static public void Connect()
        {
            Connect(IP);
        }
        static public void Connect(string _ip)
        {
            TcpClient client = new TcpClient();
            client.ConnectAsync(_ip, 50001);
            conections.Add(IP, client);
            mainConnection = client;
        }

        static async public void SendMessage()
        {
            NetworkStream stream = mainConnection.GetStream();

            string requestMessage = Console.ReadLine(); ;

            var requestData = Encoding.UTF8.GetBytes(requestMessage);

            await stream.WriteAsync(requestData,0, requestData.Length);
        }
        static async public void Listen() 
        {
            NetworkStream stream = mainConnection.GetStream();
            running = true;
            
            while (true) 
            {
                if (stream.DataAvailable) 
                {
                    var mess  = stream.ReadByte().ToString();

                    Console.WriteLine(mess);
                }
            }
        }
    }
}
