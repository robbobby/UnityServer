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
                {(int) PackageId.WelcomeReceived, ServerHandle.WelcomeReceived},
                {(int) PackageId.PlayerMovement, ServerHandle.PlayerMovement}
                
                // Add packages here
            };
        }
// ######################################################################################################## //
        private enum PackageId {
            WelcomeReceived = 1,
            PlayerMovement
            // Add package index here
        }
    }
}