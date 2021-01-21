using System;
using System.Numerics;
using MMORPG_Server.Package;

namespace MMORPG_Server.Network.Handlers {
    public class ServerHandle {
        
        // Add new listener logic here
        public static void WelcomeReceived(int clientId, Package.Packet packet) {
            LogPosition.Log("Package Received");
            int clientIdCheck = packet.ReadInt();
            string username = packet.ReadString();
            Console.WriteLine($"\n### TCP WELCOME PACKAGE REPLY ### \n" +
                              $"REPLY: {Server._Clients[clientId].Tcp.socket.Client.RemoteEndPoint} " +
                              $"name: {username} " +
                              $"connected successfully and received the welcome package. " +
                              $"Client is now player {clientId}.");
            if (clientId != clientIdCheck) { // Should never be printed, something is very wrong if this is printed
                Console.WriteLine($"Player '{username}' (ID: {clientId} has assumed the wrong client ID: {clientIdCheck}");
            }
            Server._Clients[clientId].SendIntoGame(username);
            
            // Todo: ### Send player into the game ### //
        }

        public static void PlayerMovement(int clientId, Packet packet) {
            LogPosition.Log("Package Received");
            bool[] inputs = new bool[packet.ReadInt()];
            for (int i = 0; i < inputs.Length; i++) {
                inputs[i] = packet.ReadBool();
            }

            Quaternion rotation = packet.ReadRotation();

            Server._Clients[clientId].Player.SetInput(inputs, rotation);
        }
    }
}
