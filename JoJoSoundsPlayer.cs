using JoJoStands;
using JoJoStandsSounds.Networking;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static JoJoStandsSounds.JoJoStandsSounds;

namespace JoJoStandsSounds
{
    public class JoJoSoundsPlayer : ModPlayer
    {
        public static float savedVolume = -1f;

        private bool playedPoseSound = false;
        private bool specialMoveEffectPlaying = false;

        public override void ResetEffects()
        {
            specialMoveEffectPlaying = false;
        }

        public override void PreUpdate()
        {
            if (Main.dedServ)
                return;

            MyPlayer mPlayer = Player.GetModPlayer<MyPlayer>();
            if (ContinuousBarrageSounds)
                mPlayer.standHitTime = 2;

            if (mPlayer.posing && Player.whoAmI == Main.myPlayer)
            {
                if (mPlayer.poseDuration < 200 && !playedPoseSound && mPlayer.poseSoundName != "")
                {
                    string soundPath = "JoJoStandsSounds/Sounds/PoseQuotes/" + mPlayer.poseSoundName + SoundVersion;
                    if (!ModContent.FileExists(soundPath))
                        soundPath = "JoJoStandsSounds/Sounds/PoseQuotes/" + mPlayer.poseSoundName + "_Sub";

                    SoundStyle sound = new SoundStyle(soundPath);
                    sound.Volume = JoJoStands.JoJoStands.ModSoundsVolume;
                    SoundEngine.PlaySound(sound, Player.Center);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        ModNetHandler.soundsSync.SendQuoteSound(256, Player.whoAmI, soundPath, Player.Center);
                    playedPoseSound = true;
                }
            }
            else
            {
                playedPoseSound = false;
            }

            if (mPlayer.bitesTheDustActive)
            {
                biteTheDustAmbienceSFX.Play();
                biteTheDustAmbienceSFX.Volume = JoJoStands.JoJoStands.ModSoundsVolume;
                SoundInstanceGarbageCollector.Track(biteTheDustAmbienceSFX);
                specialMoveEffectPlaying = true;
            }
            else
            {
                if (biteTheDustAmbienceSFX.State == SoundState.Playing)
                    biteTheDustAmbienceSFX.Stop();
            }
            if (mPlayer.timeskipActive)
            {
                timeskipAmbienceSFX.Play();
                timeskipAmbienceSFX.Volume = JoJoStands.JoJoStands.ModSoundsVolume;
                SoundInstanceGarbageCollector.Track(timeskipAmbienceSFX);
                specialMoveEffectPlaying = true;
            }
            else
            {
                if (timeskipAmbienceSFX.State == SoundState.Playing)
                    timeskipAmbienceSFX.Stop();
            }

            if (specialMoveEffectPlaying)
            {
                if (savedVolume == -1f)
                {
                    savedVolume = Main.musicVolume;
                    Main.musicVolume = 0f;
                }
            }
            else
            {
                if (savedVolume != -1f)
                {
                    Main.musicVolume = savedVolume;
                    savedVolume = -1f;
                }
            }

            if (Main.netMode != NetmodeID.SinglePlayer && SyncSounds)
            {
                string[] soundKeys = activeSounds.Keys.ToArray();
                for (int i = 0; i < activeSounds.Count; i++)
                {
                    SoundsHelper.PlaySound(activeSounds[soundKeys[i]].instance, activeSounds[soundKeys[i]].state, activeSounds[soundKeys[i]].position, activeSounds[soundKeys[i]].travelDistance);
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)       //that 1 last frame before you completely die
        {
            /*if (Player.whoAmI == Main.myPlayer)
            {
                if (MyPlayer.DeathSoundID == 6)
                {
                    Main.PlaySound(Mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Deathsounds/Killer"));
                }
            }*/
            return true;
        }
    }
}