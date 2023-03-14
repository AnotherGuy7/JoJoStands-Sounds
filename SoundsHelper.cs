using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;

namespace JoJoStandsSounds
{
    public class SoundsHelper
    {
        /// <summary>
        /// A method for playing sound instances with volume drop-off.
        /// </summary>
        public static void PlaySound(SoundEffectInstance sound, SoundState state, Vector2 position, int soundTravelDistance = 40)
        {
            Player Player = Main.player[Main.myPlayer];
            int volumeApexDistance = 16 * 16;       //So that if the Player gets close enough the volume doesn't go higher but instead stays at that volume
            int travelDist = soundTravelDistance * 16;
            float distanceFromSource = MathHelper.Clamp(Vector2.Distance(Player.position, position) - volumeApexDistance, 0, travelDist);

            sound.Volume = ((travelDist - distanceFromSource) / travelDist) * JoJoStands.JoJoStands.ModSoundsVolume;
            if (sound.Volume != 0f)
            {
                if (state == SoundState.Playing && sound.State != SoundState.Playing)
                {
                    sound.Play();
                    SoundInstanceGarbageCollector.Track(sound);
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