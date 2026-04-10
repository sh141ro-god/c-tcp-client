using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WebSokets
{
    internal class HttpServer
    {
        const int PORT = 8080;

        static public void Start()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();
            Console.WriteLine($"HTTP server started. Open browser at http://localhost:{PORT}");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                // Читаем запрос от браузера
                byte[] buffer = new byte[4096];
                int bytes = stream.Read(buffer, 0, buffer.Length);
                string request = Encoding.UTF8.GetString(buffer, 0, bytes);
                Console.WriteLine("--- Browser request ---");
                Console.WriteLine(request);

                // HTML-тело
                string html =
                    "<!DOCTYPE html>\n" +
                    "<html>\n" +
                    "<body>\n" +
                    "<h1>My First Heading</h1>\n" +
                    "<p>My first paragraph.</p>\n" +
                    "</body>\n" +
                    "</html>";

                // HTTP-ответ
                string response =
                    "HTTP/1.1 200 OK\r\n" +
                    "Date: Wed, 11 Feb 2009 11:20:59 GMT\r\n" +
                    "Server: Apache\r\n" +
                    "Last-Modified: Wed, 11 Feb 2021 11:20:59 GMT\r\n" +
                    "Content-Type: text/html; charset=utf-8\r\n" +
                    $"Content-Length: {Encoding.UTF8.GetByteCount(html)}\r\n" +
                    "\r\n" +
                    html;

                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);

                client.Close();
                Console.WriteLine("Response sent.");
            }
        }
    }
}
