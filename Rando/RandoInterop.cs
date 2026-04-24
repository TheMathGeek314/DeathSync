using Modding;
using System.IO;
using ItemChanger;
using MenuChanger;
using MenuChanger.MenuElements;
using RandomizerMod.Logging;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace DeathSync {
    internal static class RandoInterop {
        public static void Hook() {
            RandomizerMod.Menu.RandomizerMenuAPI.AddMenuPage(_ => { }, BuildConnectionMenuButton);

            Finder.DefineCustomItem(new DeathItem());

            RandoController.OnExportCompleted += AddModule;
            SettingsLog.AfterLogSettings += LogRandoSettings;

            DeathItem.SetupReflection();

            if(ModHooks.GetMod("RandoSettingsManager") is Mod) {
                RSMInterop.Hook();
            }
        }

        private static bool BuildConnectionMenuButton(MenuPage landingPage, out SmallButton settingsButton) {
            SmallButton button = new(landingPage, "DeathSync");

            void UpdateButtonColor() {
                button.Text.color = DeathSync.Settings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }

            UpdateButtonColor();
            button.OnClick += () => {
                DeathSync.Settings.Enabled = !DeathSync.Settings.Enabled;
                UpdateButtonColor();
            };
            settingsButton = button;
            return true;
        }

        private static void AddModule(RandoController controller) {
            if(!DeathSync.Settings.Enabled)
                return;
            ItemChangerMod.Modules.GetOrAdd<DeathModule>();
        }

        private static void LogRandoSettings(LogArguments args, TextWriter w) {
            w.WriteLine("Logging DeathSync settings:");
            w.WriteLine(JsonUtil.Serialize(DeathSync.Settings));
        }
    }
}
