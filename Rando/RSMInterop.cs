using System;
using System.Collections.Generic;
using System.Linq;
using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace DeathSync {
    internal static class RSMInterop {
        public static void Hook() {
            RandoSettingsManagerMod.Instance.RegisterConnection(new DSProxy() {
                getter = () => DeathSync.Settings,
                setter = s => DeathSync.Settings = s
            });
        }
    }

    internal class DSProxy: RandoSettingsProxy<Settings, Signature> {
        internal Func<Settings> getter;
        internal Action<Settings> setter;

        public override string ModKey => DeathSync.instance.GetName();

        public override VersioningPolicy<Signature> VersioningPolicy => new StructuralVersioningPolicy() { settingsGetter = getter };

        public override bool TryProvideSettings(out Settings settings) {
            settings = getter();
            return settings.Enabled;
        }

        public override void ReceiveSettings(Settings settings) {
            setter(settings ?? new());
        }
    }

    internal class StructuralVersioningPolicy: VersioningPolicy<Signature> {
        internal Func<Settings> settingsGetter;

        public override Signature Version => new() { FeatureSet = FeatureSetForSettings(settingsGetter()) };

        private static List<string> FeatureSetForSettings(Settings s) => SupportedFeatures.Where(f => f.feature(s)).Select(f => f.name).ToList();

        public override bool Allow(Signature s) => s.FeatureSet.All(name => SupportedFeatures.Any(sf => sf.name == name));

        private static List<(Predicate<Settings> feature, string name)> SupportedFeatures = new() {
            (s => s.Enabled, "DeathSync")
        };
    }

    internal struct Signature {
        public List<string> FeatureSet;
    }
}
