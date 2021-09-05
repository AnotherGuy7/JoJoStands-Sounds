using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;

namespace JoJoStandsSounds
{
    public class SoundsHelper
    {
        /// <summary>
        /// A method for playing sound instances with volume drop-off.
        /// </summary>
        public static void PlaySound(SoundEffectInstance sound, SoundState state, Vector2 position, int soundTravelDistance = 40)
        {
            Player player = Main.player[Main.myPlayer];
            int volumeApexDistance = 16 * 16;       //So that if the player gets close enough the volume doesn't go higher but instead stays at that volume
            int travelDist = soundTravelDistance * 16;
            float distanceFromSource = MathHelper.Clamp(Vector2.Distance(player.position, position) - volumeApexDistance, 0, travelDist);

            sound.Volume = ((travelDist - distanceFromSource) / travelDist) * MyPlayer.ModSoundsVolume;
            if (sound.Volume != 0f)
            {
                if (state == SoundState.Playing && sound.State != SoundState.Playing)
                {
                    Main.PlaySoundInstance(sound);
                }
                else if (state == SoundState.Stopped)
                {
                    sound.Stop();
                }
            }
            else
            {
                sound.Stop();
            }
        }
    }
}