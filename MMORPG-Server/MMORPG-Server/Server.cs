using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MMORPG_Server.Package;

namespace MMORPG_Server {
    public class Server {
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
        public static int MaxPlayers { get; set; }
        public static int Port { get; set; }

        public delegate void PacketHandler(int clientId, Package.Packet packet);

        public static Dictionary<int, PacketHandler> packetHandlers;
        
        public static TcpListener tcpListener;
        public static UdpClient udpListener;

        public static void Start(int maxPlayers, int port) {
            Console.WriteLine("Server starting");
            MaxPlayers = maxPlayers;
            Port = port;
            InitTcpListener();
            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UdpReceiveCallBack, null);
            Console.WriteLine($"Server started on {Port}");
            InitServerData();
        }

        private static void UdpReceiveCallBack(IAsyncResult result) {
            try {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UdpReceiveCallBack, null);
                if (data.Length < 4) return;
                using (Packet packet = new Packet(data)) {
                    int clientId = packet.ReadInt();
                    if (clientId == 0) {
                        Console.WriteLine($"{clientId} is equal to zero??");
                        return;
                    }

                    if (clients[clientId].udp.endPoint == null) {
                        Console.WriteLine("New UDP Connection");
                        clients[clientId].udp.Connect(clientEndPoint);
                        return;
                    }

                    if(clientEndPoint != null && clientEndPoint.Equals(clients[clientId].udp.endPoint))
                        clients[clientId].udp.HandleData(packet);
                }
            } catch(Exception exception) {
                Console.WriteLine($"ERROR receiving UDP data: -- Error: {exception} ");
            }
        }

        public static void SendUdpData(IPEndPoint clientEndPoint, Package.Packet packet) {
            try {
                if (clientEndPoint != null) {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(),
                        clientEndPoint, null, null);
                }
            } catch (Exception exception) {
                Console.WriteLine($"ERROR: Failed to send UDP Data to {clientEndPoint} -- Error: \n{exception}");
            }
        }

        private static void InitTcpListener() {
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
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

            packetHandlers = new Dictionary<int, PacketHandler>() {
                {(int) ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived},
                {(int) ClientPackets.UdpTestReceived, ServerHandle.UdpTestReceived}
            };
            Console.WriteLine("Initialised packets");
        }
    }
}