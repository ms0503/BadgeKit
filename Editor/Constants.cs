using JetBrains.Annotations;

namespace BadgeKit.Editor {
    [PublicAPI]
    public static class Constants {
        public const string NAME = BadgeKit.Constants.NAME;
        public const string VERSION = BadgeKit.Constants.VERSION;
        public const string MENU_ROOT = "Tools/" + NAME;
        public const string MENU_MAKER = MENU_ROOT + "/Badge Maker";
        public const string MENU_VERSION = MENU_ROOT + "/Version: " + VERSION;
        public const string ASSETS_PATH = "Assets/BadgeKit";
        public const string MATERIALS_PATH = ASSETS_PATH + "/Materials";
        public const string TEXTURES_PATH = ASSETS_PATH + "/Textures";
    }
}
