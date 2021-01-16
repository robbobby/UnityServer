using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace MMORPG_Server {
    public class Client {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;
        public Client(int id) {
            this.id = id;
            this.tcp = new TCP(id);
        }

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

                stream = socket.GetStream();
                receiverBuffer = new byte[dataBufferSize];
                stream.BeginRead(receiverBuffer, 0, dataBufferSize, ReceiveCallBack, null);
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
    }
}