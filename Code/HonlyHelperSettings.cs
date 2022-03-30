using Microsoft.Xna.Framework.Input;

namespace Celeste.Mod.HonlyHelper
{
    public class HonlyHelperSettings : EverestModuleSettings
    {
        [SettingName("HONLY_HELPER_TALK_TO_BADELINE")]
        [DefaultButtonBinding(Buttons.Y, Keys.B)]
        public ButtonBinding TalkToBadeline { get; set; } = new ButtonBinding();
    }
}