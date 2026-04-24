using System.Collections;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Modules;

namespace DeathSync {
    public class DeathModule: Module {
        public override void Initialize() {
            On.HeroController.Die += syncDeath;
        }
        public override void Unload() {
            On.HeroController.Die -= syncDeath;
        }

        private IEnumerator syncDeath(On.HeroController.orig_Die orig, HeroController self) {
            if(DeathItem.isSyncDying) {
                DeathItem.isSyncDying = false;
            }
            else {
                AbstractPlacement ap = new EmptyLocation().Wrap();
                ap.Items.Add(new DeathItem());
                ap.GiveAll(new GiveInfo() {
                    Container = Container.Unknown,
                    FlingType = FlingType.DirectDeposit,
                    MessageType = MessageType.None,
                    Transform = HeroController.instance.transform
                });
            }
            yield return orig(self);
        }
    }
}
