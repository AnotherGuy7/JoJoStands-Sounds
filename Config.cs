using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace JoJoStandsSounds
{
    public class SoundsCustomizableOptions : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        [Label("Sound Sync")]
        [Tooltip("Determines whether or not you want to sync your sounds across multiplayer (May cause some lag)")]
        public bool syncSounds = true;

        [DefaultValue(false)]
        [Label("English Dub Sounds")]
        [Tooltip("Determines whether or not you want to hear the English dub version of some sounds.")]
        public bool dubVersion;

        [DefaultValue(false)]
        [Label("Continuous Barrage Sounds")]
        [Tooltip("Whether or not Stands should continuously use barrage sounds.\nDisabling this option makes Stands only use their battle cries while an enemy is being hit.")]
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