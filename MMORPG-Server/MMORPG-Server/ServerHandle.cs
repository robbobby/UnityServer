using System;

namespace MMORPG_Server {
    public class ServerHandle {
        public static void WelcomeReceived(int clientId, Package.Packet packet) {
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
            
            
            // Todo: ### Send player into the game ### //
        }

        public static void UdpTestReceived(int clientId, Package.Packet packet) {
            string message = packet.ReadString();
            Console.WriteLine($"### UDP TEST RECEIVED REPLY ###: \n" +
                              $"Received packet via UDP Contains message: {message}");
        }
    }
}