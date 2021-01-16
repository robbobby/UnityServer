using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace MMORPG_Server {
    public class Server {
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public static int MaxPlayers { get; set; }
        public static int Port { get; set; }
        public static TcpListener tcpListener;

        public static void Start(int maxPlayers, int port) {
            MaxPlayers = maxPlayers;
            Port = port;
            Console.WriteLine("Server starting");
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
            Console.WriteLine($"Server started on {Port}");
            InitServerData();
        }

        private static void TcpConnectCallback(IAsyncResult result) {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
            Console.WriteLine($"{client.Client.RemoteEndPoint} attempting to make a connection...");
            for (int i = 1; i != MaxPlayers; i++) {
                Console.WriteLine(i);
                if (clients[i].tcp.socket == null) {
                    clients[i].tcp.Connect(client);
                    Console.WriteLine(clients[i].tcp.socket.Client);
                    Console.WriteLine($"{client.Client.RemoteEndPoint} found socket: {i} -- Connected...");
                    return;
                }
            }
            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect, server is probably full");
        }

        private static void InitServerData() {
            for(int i = 0; i < MaxPlayers; i ++) {
                clients.Add(i, new Client(i));
            }
        }
    }
}