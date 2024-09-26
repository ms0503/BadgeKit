using UnityEditor;
using UnityEngine;

namespace BadgeKit.Editor {
    internal static class Menu {
        [MenuItem(Constants.MENU_MAKER)]
        private static void MakerMenu() {
            var window = EditorWindow.GetWindow<Maker>("UIElements");
            window.titleContent = new GUIContent(L10n.Tr("Badge Maker"));
            window.Show();
        }

        [MenuItem(Constants.MENU_VERSION)]
        private static void VersionMenu() {
        }

        [MenuItem(Constants.MENU_VERSION, true)]
        private static bool VersionMenuValidation() {
            return false;
        }
    }
}
