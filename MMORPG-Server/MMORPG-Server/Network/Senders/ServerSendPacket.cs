using MMORPG_Server.Main.Networking;
using MMORPG_Server.Package;

namespace MMORPG_Server.Network.Senders {
    /// <summary>
    /// Add the package to the ClientPackets Enum,
    /// Write the sender for that package in the ServerSendPacket class
    /// Done - call the send when you need it
    /// </summary>

    #region PacketsIds enum
    public enum ServerPackets {
        Welcome = 1,
        UdpTest
    }
    #endregion
    // ######################################################################################################## //
    #region Packets
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
    #endregion
}