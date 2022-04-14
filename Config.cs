using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace JoJoStandsSounds
{
    public class SoundsCustomizableOptions : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(false)]
        [Tooltip("Determines whether or not you want to hear the English dub version of some sounds.")]
        public bool dubVersion;

        [DefaultValue(true)]
        [Tooltip("Determines whether or not you want to sync your sounds across multiplayer (May cause some lag)")]
        public bool syncSounds = true;

        [DefaultValue(false)]
        [Tooltip("Whether or not Stands should continuously use barrage sounds. Disabling this option makes Stands only use ")]
        public bool continuousBarrageSounds = false;

        public override void OnChanged()
        {
            if (dubVersion)
                JoJoStandsSounds.soundVersion = "_Dub";
            else
                JoJoStandsSounds.soundVersion = "_Sub";
            JoJoStandsSounds.syncSounds = syncSounds;
            JoJoStandsSounds.continuousBarrageSounds = continuousBarrageSounds;
        }
    }
}