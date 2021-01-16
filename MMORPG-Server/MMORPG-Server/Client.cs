using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using MMORPG_Server.Packet;

namespace MMORPG_Server {
    public class Client {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;
        public Client(int id) {
            this.id = id;
            tcp = new TCP(id);
        }

        #region TCP

        public class TCP {
            public TcpClient socket;
            private readonly int id;
            private NetworkStream stream;
            private byte[] receiverBuffer;
            
            public TCP(int id) => this.id = id;

            public void Connect(TcpClient socket) {
                this.socket = socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;
                Console.WriteLine("Sending welcome message");

                stream = socket.GetStream();
                receiverBuffer = new byte[dataBufferSize];
                stream.BeginRead(receiverBuffer, 0, dataBufferSize, ReceiveCallBack, null);
                ServerSend.Welcome(id, "Welcome to the server!");
            }

            public void SendData(Packet.Packet packet) {
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
                try {
                    int byteLength = stream.EndRead(result);
                    if (byteLength ! > 0) {
                        // Todo : Disconnect the client
                        return;
                    }
                    byte[] data = new byte[byteLength];
                    Array.Copy(receiverBuffer, data, byteLength);
                    
                    // Handle the data
                    stream.BeginRead(receiverBuffer,0,  dataBufferSize, ReceiveCallBack, null);
                } catch (Exception exception) {
                    Console.WriteLine($"An error occured : {exception}");
                    // Todo : Disconnect the client
                }
            }
        }
        #endregion
    }
}
