using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public class Client : MonoBehaviour {
    public static Client instance;
    public static int dataBufferSize = 4096;
    public string ip = "127.0.0.1";
    public int port = 26950;
    public int clientId = 0;
    public TCP tcp;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if(instance != this) {
            Debug.Log("Instance already exists, destroying object");
            Destroy(this);
        }
    }

    private void Start() {
        tcp = new TCP();
    }

    public void ConnectToServer() {
        tcp.Connect();
    }

    public class TCP {
        public TcpClient socket;
        private NetworkStream stream;
        private byte[] receiveBuffer;

        public void Connect() {
            receiveBuffer = new byte[dataBufferSize];
            socket = new TcpClient {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            socket.BeginConnect(instance.ip, instance.port, ConnectCallBack, socket);
        }

        private void ConnectCallBack(IAsyncResult result) {
            socket.EndConnect(result);
            if (!socket.Connected) {
                return;
            }

            stream = socket.GetStream();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
        }

        private void ReceiveCallBack(IAsyncResult result) {
            try {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0) {
                    // Todo : Disconnect the client
                    return;
                }
                byte[] data = new byte[byteLength];
                Array.Copy(receiveBuffer, data, byteLength);
                    
                // Handle the data
                stream.BeginRead(receiveBuffer,0,  dataBufferSize, ReceiveCallBack, null);
            } catch (Exception exception) {
                Console.WriteLine($"An error occured : {exception}");
                // Todo : Disconnect the client
            }
        }
    }
}
