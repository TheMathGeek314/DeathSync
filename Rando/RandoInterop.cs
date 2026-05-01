using ItemChanger;
using MenuChanger;
using MenuChanger.MenuElements;
using RandomizerMod.RC;

namespace DeathSync {
    internal static class RandoInterop {
        public static void Hook() {
            MultiWorldLib.ExportedAPI.ExportedExtensionsMenuAPI.AddExtensionsMenu(BuildConnectionMenuButton);
            RandoController.OnExportCompleted += AddModule;
        }

        private static BaseButton BuildConnectionMenuButton(MenuPage landingPage) {
            SmallButton button = new(landingPage, "DeathSync");

            void UpdateButtonColor() {
                button.Text.color = DeathSync.Settings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }

            UpdateButtonColor();
            button.OnClick += () => {
                DeathSync.Settings.Enabled = !DeathSync.Settings.Enabled;
                UpdateButtonColor();
            };
            return button;
        }

        private static void AddModule(RandoController controller) {
            if(!DeathSync.Settings.Enabled)
                return;
            ItemChangerMod.Modules.GetOrAdd<DeathModule>();
        }
    }
}
