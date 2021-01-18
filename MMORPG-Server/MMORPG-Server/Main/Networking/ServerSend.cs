using System;
using MMORPG_Server.Package;
using static MMORPG_Server.Main.ServerConfigConstants;

namespace MMORPG_Server {

    public abstract class ServerSend {
        #region TCP Send Package functions

        protected static void SendTcpData(int clientId, Packet packet) {
            packet.WriteLength();
            Server._Clients[clientId].Tcp.SendData(packet);
        }

        protected static void SendTcpDataToAllClients(Packet packet) {
            for (int i = 0; i <= MAX_PLAYERS; i++) {
                Server._Clients[i].Tcp.SendData(packet);
            }
        }

        protected static void SendTcpDataToAllClients(int clientNotToSendTo, Packet packet) {
            for (int i = 0; i <= MAX_PLAYERS; i++) {
                if (i == clientNotToSendTo) continue;
                Server._Clients[i].Tcp.SendData(packet);
            }
        }
        #endregion

        #region UDP Send Packages

        protected static void SendUdpData(int clientId, Packet packet) {
            Console.WriteLine($"clientId: {clientId}, packet: {packet}");
            packet.WriteLength();
            Server._Clients[clientId].Udp.SendData(packet);
        }

        protected static void SendUdpDataToAllClients(Packet packet) {
            for (int i = 0; i <= MAX_PLAYERS; i++) {
                Server._Clients[i].Udp.SendData(packet);
            }
        }

        protected static void SendUdpDataToAllClients(int clientNotToSendTo, Packet packet) {
            for (int i = 0; i <= MAX_PLAYERS; i++) {
                if (i == clientNotToSendTo) continue;
                Server._Clients[i].Udp.SendData(packet);
            }
        }
        #endregion
    }
}
