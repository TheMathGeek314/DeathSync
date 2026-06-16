using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

            List<string> incompatibleMods = new();
            if(ModHooks.GetMod("ScatteredAndLost") is Mod) {
                incompatibleMods.Add("Scattered and Lost");
            }
            if(ModHooks.GetMod("KnightOfNights") is Mod) {
                incompatibleMods.Add("Knight of Nights");
            }
            if(incompatibleMods.Count > 0) {
                throw new CompatibilityException(string.Join(", ", incompatibleMods));
            }
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
                On.HeroController.Die += syncDeath;
            
        }

        public override void Unload() {
            if(isItemSync)
                ISSubmodule.Unload();
            if(isMultiWorld)
                MWSubmodule.Unload();
            On.HeroController.Die -= syncDeath;
        }

        private IEnumerator syncDeath(On.HeroController.orig_Die orig, HeroController self) {
            if(isSyncDying) {
                isSyncDying = false;
            }
            else {
                if(isItemSync)
                    ISSubmodule.Send();
                if(isMultiWorld)
                    MWSubmodule.Send();
            }
            yield return orig(self);
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

    public class CompatibilityException: Exception {
        private string mod;
        public CompatibilityException(string otherMod) {
            mod = otherMod;
        }
        public override string Message => ToString();
        public override string ToString() => $"DeathSync does not function properly while you have {mod} installed. I haven't quite figured out why, but I'm sorry.";
    }
}
