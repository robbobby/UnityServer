using System.Collections.Generic;
using MMORPG_Server.Package;

namespace MMORPG_Server.Network.Handlers {
    
    /// <summary>
    /// Add new packages to the dictionary in GetPackages(),
    /// Add the package to the ClientPackets Enum,
    /// In the ServerHandle.cs write the handler for that package
    /// Done - the listener will now be able to handle the package if it arrives 
    /// </summary>
    public static class PackagesToHandle {
        public static Dictionary<int, Server.PacketHandler> GetPackages() {
            return new() {
                {(int) ClientPackets.WelcomeReceived, ServerHandle.WelcomeReceived},
                {(int) ClientPackets.UdpTestReceived, ServerHandle.UdpTestReceived}
                // Add packages here
            };
        }

        private enum ClientPackets {
            WelcomeReceived = 1,
            UdpTestReceived
            // Add package index here
        }
    }
}