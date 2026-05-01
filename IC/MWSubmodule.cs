using System.Reflection;
using KorzUtils.Helper;
using MultiWorldLib;
using MultiWorld = MultiWorldMod.MultiWorldMod;

namespace DeathSync {
    internal static class MWSubmodule {
        static FieldInfo _connection;
        static MultiWorldMod.ClientConnection Connection;

        public static void Initialize() {
            if(MultiWorld.MWS.IsMW) {
                DeathModule.instance.isMultiWorld = true;
                _connection = typeof(MultiWorld).GetField("Connection", BindingFlags.NonPublic | BindingFlags.Static);
                Connection = _connection.GetValue(null) as MultiWorldMod.ClientConnection;
                Connection.OnDataReceived += DataReceived;
            }
            else
                DeathModule.instance.isMultiWorld = false;
        }

        public static void Unload() {
            Connection.OnDataReceived -= DataReceived;
        }

        public static void Send() {
            Connection.SendDataToAll(DeathModule.DeathSyncEvent, "meow");
        }

        private static void DataReceived(DataReceivedEvent e) {
            if(e.Label == DeathModule.DeathSyncEvent && !e.Handled) {
                e.Handled = true;
                if(e.From != MultiWorld.MWS.GetPlayerName(MultiWorld.MWS.PlayerId)) {
                    DeathModule.instance.isSyncDying = true;
                    GameHelper.DisplayMessage($"{e.From} died");
                    DeathModule.instance.Die();
                }
            }
        }
    }
}
