using System;
using System.Net;
using System.Collections.Generic;
using WebSockets;
using WebSokets;

Console.WriteLine("Select mode:");
Console.WriteLine("1 - Listener (wait for connections)");
Console.WriteLine("2 - Searcher (find and connect)");
Console.WriteLine("3 - Echo server (multi-client)");
Console.WriteLine("4 - HTTP server (respond to browser)");
Console.WriteLine("5 - Searcher -> Echo server");
string choice = Console.ReadLine();

if (choice == "1")
{
    Listener.Listen();
}
else if (choice == "2")
{
    Console.Write("Enter last octet of your IP (172.20.117.?): ");
    string octet = Console.ReadLine();
    Searcher.localIP = $"172.20.117.{octet}";
    Console.WriteLine("Searching...");
    Searcher.Search();
    Searcher.Print();

    if (Searcher.Found.Count == 0)
    {
        Console.WriteLine("No hosts found.");
        return;
    }

    Console.Write("Enter number to connect to: ");
    int index = int.Parse(Console.ReadLine()) - 1;
    var list = new List<KeyValuePair<string, int>>(Searcher.Found);
    string ip = list[index].Key;
    int port = list[index].Value;
    Searcher.Connect(ip, port);
    Console.WriteLine($"Connected to {ip}:{port}");

    while (true)
    {
        Console.Write("Message: ");
        string msg = Console.ReadLine();
        if (msg == "exit") break;
        Searcher.Write(msg);

        // читаем ответ если это EchoServer
        string echo = Searcher.Read();
        if (echo != null)
            Console.WriteLine($"Echo: {echo}");
    }
}
else if (choice == "3")
{
    EchoServer.Start();
}
else if (choice == "4")
{
    HttpServer.Start();
}
else if (choice == "5")
{
    Console.Write("Enter last octet of your IP (172.20.117.?): ");
    string octet = Console.ReadLine();
    Searcher.localIP = $"172.20.117.{octet}";
    Console.WriteLine("Searching...");
    Searcher.Search();
    Searcher.Print();

    if (Searcher.Found.Count == 0)
    {
        Console.WriteLine("No hosts found.");
        return;
    }

    Console.Write("Enter number to connect to: ");
    int index = int.Parse(Console.ReadLine()) - 1;
    var list = new List<KeyValuePair<string, int>>(Searcher.Found);
    string ip = list[index].Key;
    int port = list[index].Value;

    Searcher.Connect(ip, port);
    Console.WriteLine($"Connected to {ip}:{port}");

    while (true)
    {
        Console.Write("Message: ");
        string msg = Console.ReadLine();
        if (msg == "exit") break;
        Searcher.Write(msg);
        string echo = Searcher.Read();
        if (echo != null)
            Console.WriteLine($"Echo: {echo}");
    }
}
else
{
    Console.WriteLine("Unknown choice.");
}