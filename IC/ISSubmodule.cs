using KorzUtils.Helper;
using MultiWorldLib;
using ItemSync = ItemSyncMod.ItemSyncMod;

namespace DeathSync {
    internal static class ISSubmodule {
        public static void Initialize() {
            if(ItemSync.ISSettings.IsItemSync) {
                DeathModule.instance.isItemSync = true;
                ItemSync.Connection.OnDataReceived += DataReceived;
            }
            else
                DeathModule.instance.isItemSync = false;
        }

        public static void Unload() {
            ItemSync.Connection.OnDataReceived -= DataReceived;
        }

        public static void Send() {
            ItemSync.Connection.SendDataToAll(DeathModule.DeathSyncEvent, "hiss");
        }

        private static void DataReceived(DataReceivedEvent e) {
            if(e.Label == DeathModule.DeathSyncEvent && !e.Handled) {
                e.Handled = true;
                if(e.From != ItemSync.ISSettings.UserName) {
                    DeathModule.instance.isSyncDying = true;
                    GameHelper.DisplayMessage($"{e.From} died");
                    DeathModule.instance.Die();
                }
            }
        }
    }
}
