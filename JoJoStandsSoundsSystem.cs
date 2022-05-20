using Terraria;
using Terraria.ModLoader;

namespace JoJoStandsSounds
{
    public class JoJoStandsSoundsSystem : ModSystem
    {
        public override void PreSaveAndQuit()
        {
            if (JoJoSoundsPlayer.savedVolume != -1f)
            {
                Main.musicVolume = JoJoSoundsPlayer.savedVolume;
                JoJoSoundsPlayer.savedVolume = -1f;
            }
        }
    }
}
