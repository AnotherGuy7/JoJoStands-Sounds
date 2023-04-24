using JoJoStands;
using JoJoStandsSounds.Networking;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Content;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Exceptions;
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
            if (JoJoStandsSounds.ContinuousBarrageSounds)
                mPlayer.standHitTime = 2;

            if (mPlayer.posing && Player.whoAmI == Main.myPlayer)
            {
                if (mPlayer.poseDuration < 200 && !playedPoseSound && mPlayer.poseSoundName != "")
                {
                    string soundPath = "JoJoStandsSounds/Sounds/PoseQuotes/" + mPlayer.poseSoundName + JoJoStandsSounds.SoundVersion;
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
                JoJoStandsSounds.biteTheDustAmbienceSFX.Play();
                JoJoStandsSounds.biteTheDustAmbienceSFX.Volume = JoJoStands.JoJoStands.ModSoundsVolume;
                SoundInstanceGarbageCollector.Track(JoJoStandsSounds.biteTheDustAmbienceSFX);
                specialMoveEffectPlaying = true;
            }
            else
            {
                if (JoJoStandsSounds.biteTheDustAmbienceSFX.State == SoundState.Playing)
                    JoJoStandsSounds.biteTheDustAmbienceSFX.Stop();
            }
            if (mPlayer.timeskipActive)
            {
                JoJoStandsSounds.timeskipAmbienceSFX.Play();
                JoJoStandsSounds.timeskipAmbienceSFX.Volume = JoJoStands.JoJoStands.ModSoundsVolume;
                SoundInstanceGarbageCollector.Track(JoJoStandsSounds.timeskipAmbienceSFX);
                specialMoveEffectPlaying = true;
            }
            else
            {
                if (JoJoStandsSounds.timeskipAmbienceSFX.State == SoundState.Playing)
                    JoJoStandsSounds.timeskipAmbienceSFX.Stop();
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

            if (Main.netMode != NetmodeID.SinglePlayer && JoJoStandsSounds.SyncSounds)
            {
                string[] soundKeys = JoJoStandsSounds.activeSounds.Keys.ToArray();
                for (int i = 0; i < JoJoStandsSounds.activeSounds.Count; i++)
                {
                    SoundsHelper.PlaySound(JoJoStandsSounds.activeSounds[soundKeys[i]].instance, JoJoStandsSounds.activeSounds[soundKeys[i]].state, JoJoStandsSounds.activeSounds[soundKeys[i]].position, JoJoStandsSounds.activeSounds[soundKeys[i]].travelDistance);
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