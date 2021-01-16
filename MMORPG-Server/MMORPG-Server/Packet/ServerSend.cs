using System.Net.Http.Headers;

namespace MMORPG_Server.Packet {
    public class ServerSend {
        public static void Welcome(int clientId, string message) {
            using (Packet packet = new Packet((int) ServerPackets.Welcome)) {
                packet.Write(message);
                packet.Write(clientId);
                SendTcpData(clientId, packet);
            }
        }
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
    }
}