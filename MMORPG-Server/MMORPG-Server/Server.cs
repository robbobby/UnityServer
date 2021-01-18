using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using MMORPG_Server.Main.Networking;
using MMORPG_Server.Package;
using static MMORPG_Server.Main.ServerConfigConstants;

namespace MMORPG_Server {
    public class Server {
        public static readonly Dictionary<int, Client> _Clients = new();
        private static int Port { get; set; }

        public delegate void PacketHandler(int clientId, Packet packet);

        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener _tcpListener;
        private static UdpClient _udpListener;

        public static void Start( int port) {
            Console.WriteLine("Server starting");
            Port = port;
            InitTcpListener();
            _udpListener = new UdpClient(Port);
            _udpListener.BeginReceive(UdpReceiveCallBack, null);
            Console.WriteLine($"Server started on {Port}");
            InitServerData();
        }

        private static void UdpReceiveCallBack(IAsyncResult result) {
            try {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = _udpListener.EndReceive(result, ref clientEndPoint);
                _udpListener.BeginReceive(UdpReceiveCallBack, null);
                if (data.Length < 4) return;
                
                using Packet packet = new Packet(data);
                int clientId = packet.ReadInt();
                if (clientId == 0) {
                    Console.WriteLine($"{clientId} is equal to zero??");
                    return;
                }

                if (_Clients[clientId].Udp.endPoint == null) {
                    Console.WriteLine("New UDP Connection");
                    _Clients[clientId].Udp.Connect(clientEndPoint);
                    return;
                }

                if(clientEndPoint != null && clientEndPoint.Equals(_Clients[clientId].Udp.endPoint))
                    _Clients[clientId].Udp.HandleData(packet);
            } catch(Exception exception) {
                Console.WriteLine($"ERROR receiving UDP data: -- Error: {exception} ");
            }
        }

        public static void SendUdpData(IPEndPoint clientEndPoint, Packet packet) {
            try {
                if (clientEndPoint != null) {
                    _udpListener.BeginSend(packet.ToArray(), packet.Length(),
                        clientEndPoint, null, null);
                }
            } catch (Exception exception) {
                Console.WriteLine($"ERROR: Failed to send UDP Data to {clientEndPoint} -- Error: \n{exception}");
            }
        }

        private static void InitTcpListener() {
            _tcpListener = new TcpListener(IPAddress.Any, Port);
            _tcpListener.Start();
            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
        }

        private static void TcpConnectCallback(IAsyncResult result) {
            TcpClient client = _tcpListener.EndAcceptTcpClient(result);
            _tcpListener.BeginAcceptTcpClient(TcpConnectCallback, null);
            Console.WriteLine($"{client.Client.RemoteEndPoint} attempting to make a connection...");
            for (int i = 1; i != MAX_PLAYERS; i++) {
                Console.WriteLine(i);
                if (_Clients[i].Tcp.socket != null) continue;
                _Clients[i].Tcp.Connect(client);
                Console.WriteLine(_Clients[i].Tcp.socket.Client);
                Console.WriteLine($"{client.Client.RemoteEndPoint} found socket: {i} -- Connected...");
                return;
            }
            Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect, server is probably full");
        }

        private static void InitServerData() {
            for(int i = 0; i < MAX_PLAYERS; i ++) {
                _Clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler> {
                {(int) ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived},
                {(int) ClientPackets.UdpTestReceived, ServerHandle.UdpTestReceived}
            };
            Console.WriteLine("Initialised packets");
        }
    }
}