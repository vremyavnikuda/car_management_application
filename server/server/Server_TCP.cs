using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace TcpServer
{
    class ServerTCP
    {
        private static List<TcpClient> clients = new List<TcpClient>();
        private static TcpListener server;
        private static readonly string dataFilePath = "server_cars.json";

        static void Main(string[] args)
        {
            // Запуск сервера на порту 5000
            server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            Console.WriteLine("Server started. Port: 5000....");

            Thread listenerThread = new Thread(ListenForClients);
            listenerThread.Start();
        }

        private static void ListenForClients()
        {
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                clients.Add(client);

                Console.WriteLine("New client connected....");
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        private static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[65536];
            string welcomeMessage = "Welcome to the server!\n";
            byte[] message = Encoding.UTF8.GetBytes(welcomeMessage);
            stream.Write(message, 0, message.Length);

            // Рассылка списка клиентов всем подключённым клиентам
            BroadcastClientList();

            while (true)
            {
                try
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string messageReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Get message from client: {client.Client.RemoteEndPoint}: {messageReceived}");
                    if (messageReceived.StartsWith("SEND_DATA:"))
                    {
                        string jsonData = messageReceived.Substring("SEND_DATA:".Length);
                        File.WriteAllText(dataFilePath, jsonData);
                        Console.WriteLine("Data saved to " + dataFilePath);
                    }
                    else if (messageReceived == "REQUEST_DATA")
                    {
                        if (File.Exists(dataFilePath))
                        {
                            string jsonData = File.ReadAllText(dataFilePath);
                            byte[] response = Encoding.UTF8.GetBytes($"DATA: {jsonData}");
                            stream.Write(response, 0, response.Length);
                            Console.WriteLine("Data sent to " + client.Client.RemoteEndPoint);
                        }
                        else
                        {
                            byte[] response = Encoding.UTF8.GetBytes("DATA:[]");
                            stream.Write(response, 0, response.Length);
                            Console.WriteLine("No data found,send empty list");
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error occurred: " + exception.Message);
                    break;
                }
            }

            clients.Remove(client);
            client.Close();
            Console.WriteLine("Client disconnected....");
            BroadcastClientList();
        }

        // Отправить всем клиентам информацию о текущем списке клиентов
        private static void BroadcastClientList()
        {
            string clientList = "Connected clients:\n";
            foreach (var client in clients)
            {
                clientList += client.Client.RemoteEndPoint + "\n";
            }

            byte[] message = Encoding.UTF8.GetBytes(clientList);
            foreach (var client in clients)
            {
                try
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(message, 0, message.Length);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("Error occurred: " + exception.Message);
                }
            }
        }
    }
}
