using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebSockets
{
    public class Searcher
    {
        //const int DISCOVERY_PORT = 40000;
        const int DISCOVERY_PORT = 40000;
        const int TIMEOUT_MS = 500;

        public static Dictionary<string, int> Found { get; } = new();
        public static TcpClient CurrentTcp { get; private set; }

        // Вписываешь свой IP руками
        public static string localIP = "172.20.117.3";

        public static void Search()
        {
            Found.Clear();
            byte[] discover = Encoding.UTF8.GetBytes("DISCOVER");

            // Берём подсеть из своего IP автоматически
            string subnet = localIP.Substring(0, localIP.LastIndexOf('.'));

            Console.WriteLine($"[Searcher] Сканирование {subnet}.1-254, пропускаю {localIP}...");

            for (int i = 1; i <= 254; i++)
            {
                string ip = $"{subnet}.{i}";
                if (ip == localIP) continue;

                try
                {
                    using UdpClient udp = new UdpClient();
                    udp.Client.ReceiveTimeout = TIMEOUT_MS;

                    IPEndPoint target = new IPEndPoint(IPAddress.Parse(ip), DISCOVERY_PORT);
                    udp.Send(discover, discover.Length, target);

                    IPEndPoint remote = null;
                    byte[] resp = udp.Receive(ref remote);
                    string msg = Encoding.UTF8.GetString(resp);

                    if (msg.StartsWith("HERE:") && int.TryParse(msg.Substring(5), out int port))
                    {
                        string foundIp = remote.Address.ToString();
                        Found[foundIp] = port;
                        Console.WriteLine($"[Searcher] Найден: {foundIp}:{port}");
                        break;
                    }
                }
                catch (SocketException) { /* таймаут — идём дальше */ }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Searcher] {ip}: {ex.Message}");
                }
            }

            Console.WriteLine($"[Searcher] Готово. Найдено: {Found.Count}");
        }

        public static void Print()
        {
            int i = 0;
            foreach (var item in Found)
                Console.WriteLine($"{++i}| {item.Key}:{item.Value}");
        }

        public static bool Connect(string ip, int port)
        {
            try
            {
                CurrentTcp?.Close();
                CurrentTcp = new TcpClient();
                CurrentTcp.Connect(IPAddress.Parse(ip), port);
                Console.WriteLine($"[Searcher] Подключён к {ip}:{port}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Searcher] Ошибка подключения: {ex.Message}");
                return false;
            }
        }

        public static void Write(string message)
        {
            if (CurrentTcp?.Connected != true)
            {
                Console.WriteLine("[Searcher] Нет подключения");
                return;
            }

            try
            {
                NetworkStream s = CurrentTcp.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                s.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Searcher] Ошибка отправки: {ex.Message}");
            }
        }
        static public string Read()
        {
            if (CurrentTcp?.Connected != true) return null;

            try
            {
                NetworkStream s = CurrentTcp.GetStream();
                byte[] buffer = new byte[1024];
                int bytes = s.Read(buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer, 0, bytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Searcher] Ошибка чтения: {ex.Message}");
                return null;
            }
        }
    }
}