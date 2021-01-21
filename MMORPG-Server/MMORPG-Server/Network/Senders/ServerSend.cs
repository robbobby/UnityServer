using MMORPG_Server.ClientEntity;
using MMORPG_Server.Main.Networking;
using MMORPG_Server.Package;

namespace MMORPG_Server.Network.Senders {
    /// <summary>
    /// Add the package to the ClientPackets Enum,
    /// Write the sender for that package in the ServerSendPacket class
    /// Done - call the send when you need it
    /// </summary>

    #region PacketsIds enum

    public enum PacketId {
        Welcome = 1,
        SpawnPlayer,
        PlayerPosition,
        PlayerRotation
    }
    #endregion
    // ######################################################################################################## //
    #region Packets
    public class ServerSend : ServerSendPacket {
        public static void Welcome(int clientId, string message) {
            LogPosition.Log("Package sent");
            using Packet packet = new Packet((int) PacketId.Welcome);
            packet.Write(message);
            packet.Write(clientId);
            SendTcpData(clientId, packet);
        }
        
        public static void SpawnPlayer(int toClient, Player player) {
            LogPosition.Log("Package sent");
            using (Packet packet = new Packet((int) PacketId.SpawnPlayer)) {
                packet.Write(player.Id);
                packet.Write(player.Username);
                packet.Write(player.Position);
                packet.Write(player.Rotation);
                
                SendTcpData(toClient, packet);
            }
        }

        public static void PlayerRotation(Player player) {
            LogPosition.Log("Package sent");
            using (Packet packet = new Packet((int) PacketId.PlayerRotation)) {
                packet.Write(player.Id);
                packet.Write(player.Rotation);
                SendUdpDataToAllClients(player.Id, packet);
            }
        }

        public static void PlayerPosition(Player player) {
            LogPosition.Log("Package sent");
            using (Packet packet = new Packet((int) PacketId.PlayerPosition)) {
                packet.Write(player.Id);
            LogPosition.Log("Package sent");
                packet.Write(player.Position);
            LogPosition.Log("Package sent");
                SendUdpDataToAllClients(packet);
            LogPosition.Log("Package sent");
            }
        }
    }
    #endregion
}