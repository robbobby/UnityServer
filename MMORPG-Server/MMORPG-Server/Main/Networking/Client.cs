using System;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using MMORPG_Server.Network.Senders;
using MMORPG_Server.ClientEntity;
using Packet = MMORPG_Server.Package.Packet;

namespace MMORPG_Server.Main.Networking {
    public class Client {
        private int id;
        private const int DATA_BUFFER_SIZE = 4096;
        public TCP Tcp { get; }
        public UDP Udp { get; }
        public Player Player { get; private set; }

        public Client(int id) {
            this.id = id;
            Tcp = new TCP(id);
            Udp = new UDP(id);
        }
        
        #region TCP

        // ReSharper disable once InconsistentNaming
        public class TCP {
            public TcpClient socket;
            
            private readonly int id;
            private NetworkStream stream;

            private Packet receivedData;
            private byte[] receiverBuffer;
            
            public TCP(int id) => this.id = id;

            public void Connect(TcpClient clientSocket) {
                socket = clientSocket;
                
                clientSocket.ReceiveBufferSize = DATA_BUFFER_SIZE;
                clientSocket.SendBufferSize = DATA_BUFFER_SIZE;
                Console.WriteLine("Sending welcome message");

                stream = clientSocket.GetStream();

                receivedData = new Packet();
                receiverBuffer = new byte[DATA_BUFFER_SIZE];
                stream.BeginRead(receiverBuffer, 0, DATA_BUFFER_SIZE, ReceiveCallBack, null);
                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void SendData(Packet packet) {
                try {
                    if (socket != null) {
                        stream.BeginWrite(packet.ToArray(), 0,
                            packet.Length(), null, null);
                    }
                } catch (Exception exception) {
                    Console.WriteLine($"Error sending TCP data to client {id} -- Error:\n{exception}");                    
                }
            }

            private void ReceiveCallBack(IAsyncResult result) {
                Console.WriteLine(result);
                try {
                    int byteLength = stream.EndRead(result);
                    Console.WriteLine(byteLength);
                    if (!(byteLength > 0)) {
                        // Todo : Disconnect the client
                        return;
                    }
                    Console.WriteLine(byteLength);
                    
                    byte[] data = new byte[byteLength];
                    Array.Copy(receiverBuffer, data, byteLength);
                    
                    receivedData.Reset(HandleData(data));
                    stream.BeginRead(receiverBuffer,0,  DATA_BUFFER_SIZE, ReceiveCallBack, null);
                } catch (Exception exception) {
                    Console.WriteLine($"An error occured : {exception}");
                    // Todo : Disconnect the client
                }
            }
            private bool HandleData(byte[] data) {
                Console.WriteLine("Are we handling data");
                int packetLength = 0;
                receivedData.SetBytes(data);
                if (receivedData.UnreadLength() > 4) {
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0) return true;
                }

                while (packetLength > 0 && packetLength <= receivedData.UnreadLength()) {
                    byte[] packetBytes = receivedData.ReadBytes(packetLength);
                    ThreadManager.ExecuteOnMainThread(() => {
                        using Packet packet = new Packet(packetBytes);
                        int packetId = packet.ReadInt();
                        Server.packetHandlers[packetId](id, packet);
                    });
                    packetLength = 0;
                    if (receivedData.UnreadLength() <= 4) continue;
                    packetLength = receivedData.ReadInt();
                    if (packetLength <= 0) return true;
                }
                return packetLength <= 1;
            }
        }
        #endregion
        // ######################################################################################################## //

        #region UDP
        // ReSharper disable once InconsistentNaming
        public class UDP {
            public IPEndPoint endPoint;
            private readonly int id;
            public UDP(int id) {
                this.id = id;
            }

            public void Connect(IPEndPoint ipEndPoint) {
                endPoint = ipEndPoint;
            }

            public void SendData(Packet packet) {
                Server.SendUdpData(endPoint, packet);
            }

            public void HandleData(Packet packetData) {
                int packetLength = packetData.ReadPacketLength();
                byte[] packetBytes = packetData.ReadBytes(packetLength);
                
                ThreadManager.ExecuteOnMainThread(() => {
                    using Packet packet = new Packet(packetBytes);
                    int packetId = packet.ReadInt();
                    Server.packetHandlers[packetId](id, packet);
                });
            }
        }
        #endregion

        
        // Todo: Can we combine these 2 loops into 1? 
        public void SendIntoGame(string playerName) {
            Player = new Player(id, playerName, new Vector3(0, 0, 0));
            // for(int i = 0; i < Server._Clients.Values.Count; i++)
            // Send to all other clients
            foreach (Client client in Server._Clients.Values)
            {
                if (client.Player != null) {
                    if (client.id != id) {
                        ServerSend.SpawnPlayer(id, client.Player);
                        Console.WriteLine("Added player");
                    }
                }
            }

            // Send to Client player
            // for(int i = 0; i < Server._Clients.Values.Count; i++) {
            foreach (Client client in Server._Clients.Values) {
                if (client.Player != null) {
                    ServerSend.SpawnPlayer(client.id, Player);
                        Console.WriteLine("Added player TO ALL CLIENTS");
                }
            }
        }
    }
}
