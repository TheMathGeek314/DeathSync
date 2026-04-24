using System;
using System.Collections;
using System.Reflection;
using ItemChanger;
using ItemChanger.Tags;

namespace DeathSync {
    public class DeathItem: AbstractItem {
        private static Type syncedTagType;
        private static FieldInfo wasObtainedLocally;
        private static MethodInfo heroDie = typeof(HeroController).GetMethod("Die", BindingFlags.NonPublic | BindingFlags.Instance);

        internal static bool isSyncDying = false;

        public DeathItem() {
            name = "DeathSync";
            InteropTag tag = GetOrAddTag<InteropTag>();
            tag.Message = "RandoSupplementalMetadata";
            tag.Properties["ModSource"] = DeathSync.instance.GetName();
            AddTag<PersistentItemTag>().Persistence = Persistence.Persistent;
        }

        public override void GiveImmediate(GiveInfo info) {
            bool temp = false;
            foreach(Tag tag in tags) {
                if(tag.GetType().Name == "SyncedItemTag") {
                    if(!(bool)wasObtainedLocally.GetValue(tag)) {
                        isSyncDying = true;
                        Die();
                    }
                    temp = true;
                    break;
                }
            }
            if(!temp) {
                isSyncDying = true;
                Die();
            }
        }

        private void Die() {
            IEnumerator routine = (IEnumerator)heroDie.Invoke(HeroController.instance, null);
            GameManager.instance.StartCoroutine(routine);
        }

        internal static void SetupReflection() {
            syncedTagType = typeof(ItemSyncMod.GlobalSettings).Assembly.GetType("ItemSyncMod.Items.SyncedItemTag");
            wasObtainedLocally = syncedTagType.GetField("isLocalPickUp", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
