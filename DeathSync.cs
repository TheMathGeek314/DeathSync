using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace DeathSync {
    public class DeathSync: Mod, IGlobalSettings<Settings> {
        new public string GetName() => "DeathSync";
        public override string GetVersion() => "1.0.0.0";

        public static Settings Settings { get; set; } = new();
        public void OnLoadGlobal(Settings s) => Settings = s;
        public Settings OnSaveGlobal() => Settings;

        internal static DeathSync instance;

        public DeathSync(): base(null) {
            instance = this;
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            RandoInterop.Hook();
        }
    }
}

//accidental mw/is dependency