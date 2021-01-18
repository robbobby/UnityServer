using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MMORPG_Server.Package;

namespace MMORPG_Server {
    public class Client {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;
        public UDP udp;
        public Client(int id) {
            this.id = id;
            tcp = new TCP(id);
            udp = new UDP(id);
        }
        
        #region TCP

        public class TCP {
            public TcpClient socket;
            
            private readonly int id;
            private NetworkStream stream;

            private Package.Packet receivedData;
            private byte[] receiverBuffer;
            
            public TCP(int id) => this.id = id;

            public void Connect(TcpClient socket) {
                this.socket = socket;
                
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;
                Console.WriteLine("Sending welcome message");

                stream = socket.GetStream();

                receivedData = new Package.Packet();
                receiverBuffer = new byte[dataBufferSize];
                stream.BeginRead(receiverBuffer, 0, dataBufferSize, ReceiveCallBack, null);
                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void SendData(Package.Packet packet) {
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
                    stream.BeginRead(receiverBuffer,0,  dataBufferSize, ReceiveCallBack, null);
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
                        using (Packet packet = new Packet(packetBytes)) {
                            int packetId = packet.ReadInt();
                            Server.packetHandlers[packetId](id, packet);
                        }
                    });
                    packetLength = 0;
                    if (receivedData.UnreadLength() > 4) {
                        packetLength = receivedData.ReadInt();
                        if (packetLength <= 0) return true;
                    }
                }
                if(packetLength <= 1) return true;
                return false;
            }
        }
        #endregion

        public class UDP {
            public IPEndPoint endPoint;
            private int id;
            public UDP(int id) {
                this.id = id;
            }

            public void Connect(IPEndPoint ipEndPoint) {
                endPoint = ipEndPoint;
                ServerSend.UdpTest(id);
            }

            public void SendData(Packet packet) {
                Server.SendUdpData(endPoint, packet);
            }

            public void HandleData(Packet packetData) {
                int packetLength = packetData.ReadPacketLength();
                byte[] packetBytes = packetData.ReadBytes(packetLength);
                
                ThreadManager.ExecuteOnMainThread(() => {
                    using (Packet packet = new Packet(packetBytes)) {
                        int packetId = packet.ReadInt();
                        Server.packetHandlers[packetId](id, packet);
                    }
                });
            }
        }
        
    }
}
