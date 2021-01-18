using MMORPG_Server.Package;

namespace MMORPG_Server {
    public class ServerSendPacket : ServerSend {
        public static void Welcome(int clientId, string message) {
            using Packet packet = new Packet((int) ServerPackets.Welcome);
            packet.Write(message);
            packet.Write(clientId);
            SendTcpData(clientId, packet);
        }

        public static void UdpTest(int clientId) {
            using Packet packet = new Packet((int) ServerPackets.UdpTest);
            packet.Write("Testing Send on UDP");
            SendUdpData(clientId, packet);
        }
    }
}