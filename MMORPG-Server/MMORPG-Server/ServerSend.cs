using System;
using MMORPG_Server.Package;

namespace MMORPG_Server {
    public class ServerSend {
        public static void Welcome(int clientId, string message) {
            using (Packet packet = new Packet((int) ServerPackets.Welcome)) {
                packet.Write(message);
                packet.Write(clientId);
                SendTcpData(clientId, packet);
            }
        }

        public static void UdpTest(int clientId) {
            using (Packet packet = new Packet((int) ServerPackets.UdpTest)) {
                packet.Write("Testing Send on UDP");
                SendUdpData(clientId, packet);
            }
        }

        #region TCP Send Package functions

        

        private static void SendTcpData(int clientId, Packet packet) {
            packet.WriteLength();
            Server.clients[clientId].tcp.SendData(packet);
        }

        private static void SendTcpDataToAllClients(Packet packet) {
            for (int i = 0; i <= Server.MaxPlayers; i++) {
                Server.clients[i].tcp.SendData(packet);
            }
        }

        private static void SendTcpDataToAllClients(int clientNotToSendTo, Packet packet) {
            for (int i = 0; i <= Server.MaxPlayers; i++) {
                if (i == clientNotToSendTo) continue;
                Server.clients[i].tcp.SendData(packet);
            }
        }
        #endregion

        #region UDP Send Packages
        public static void SendUdpData(int clientId, Packet packet) {
            Console.WriteLine($"clientId: {clientId}, packet: {packet}");
            packet.WriteLength();
            Server.clients[clientId].udp.SendData(packet);
        }
        private static void SendUdpDataToAllClients(Packet packet) {
            for (int i = 0; i <= Server.MaxPlayers; i++) {
                Server.clients[i].udp.SendData(packet);
            }
        }

        private static void SendUdpDataToAllClients(int clientNotToSendTo, Packet packet) {
            for (int i = 0; i <= Server.MaxPlayers; i++) {
                if (i == clientNotToSendTo) continue;
                Server.clients[i].udp.SendData(packet);
            }
        }
        #endregion
    }
}
