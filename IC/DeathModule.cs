using Modding;
using System;
using System.Collections;
using System.Reflection;
using MonoMod.Cil;
using ItemChanger;

namespace DeathSync {
    public class DeathModule: ItemChanger.Modules.Module {
        internal static DeathModule instance;
        internal const string DeathSyncEvent = nameof(DeathSync) + "-PlayerDied";
        internal bool isSyncDying = false;
        internal bool isItemSync = false;
        internal bool isMultiWorld = false;

        private MethodInfo heroDie = typeof(HeroController).GetMethod("Die", BindingFlags.NonPublic | BindingFlags.Instance);

        public DeathModule() {
            ModuleHandlingProperties = ModuleHandlingFlags.AllowDeserializationFailure;
        }

        public override void Initialize() {
            instance = this;
            if(ModHooks.GetMod("ItemSyncMod") is Mod) {
                ISSafetyLayer.Init();
            }
            if(ModHooks.GetMod("MultiWorldMod") is Mod) {
                MWSafetyLayer.Init();
            }
            if(isItemSync || isMultiWorld)
                IL.HeroController.TakeDamage += syncDeath;
        }

        public override void Unload() {
            if(isItemSync)
                ISSubmodule.Unload();
            if(isMultiWorld)
                MWSubmodule.Unload();
            IL.HeroController.TakeDamage -= syncDeath;
        }

        private void syncDeath(ILContext il) {
            ILCursor cursor = new ILCursor(il).Goto(0);
            while(cursor.TryGotoNext(i => i.MatchLdarg(0),
                                     i => i.MatchLdarg(0),
                                     i => i.MatchCallvirt<HeroController>("Die"))) {
                cursor.EmitDelegate<Action>(() => {
                    if(isSyncDying)
                        isSyncDying = false;
                    else {
                        if(isItemSync)
                            ISSubmodule.Send();
                        if(isMultiWorld)
                            MWSubmodule.Send();
                    }
                });
                cursor.Index += 2;
            }
        }

        internal void Die() {
            IEnumerator routine = (IEnumerator)heroDie.Invoke(HeroController.instance, null);
            GameManager.instance.StartCoroutine(routine);
        }
    }

    internal class ISSafetyLayer {
        internal static void Init() {
            ISSubmodule.Initialize();
        }
    }

    internal class MWSafetyLayer {
        internal static void Init() {
            MWSubmodule.Initialize();
        }
    }
}
