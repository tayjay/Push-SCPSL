using System.ComponentModel;

namespace Push
{
    public class Translations
    {
        [Description("Header for Push settings in the server-specific settings menu.")]
        public string SSGroupLabel { get; set; } = "Push Settings";
        [Description("Label for the Push keybind setting.")]
        public string SSPushLabel { get; set; } = "Keybind - Push";
        [Description("Hint for the Push keybind setting.")]
        public string SSPushHint { get; set; } = "Push another player.";
        [Description("Label for the Pull keybind setting.")]
        public string SSPullLabel { get; set; } = "Keybind - Pull";
        [Description("Hint for the Pull keybind setting.")]
        public string SSPullHint { get; set; } = "Pull another player towards you.";
    }
}