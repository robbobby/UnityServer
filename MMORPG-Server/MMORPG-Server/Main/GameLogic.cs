using MMORPG_Server.Main.Networking;

namespace MMORPG_Server.Main {
    public class GameLogic {
        public static void Update() {
            foreach (Client client in Server._Clients.Values) {
                if (client.Player != null) {
                    client.Player.Update();
                }
            }
            ThreadManager.UpdateMain();
        }
    }
}