using JoJoStands;
using JoJoStandsSounds.Networking;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStandsSounds
{
    public class JoJoSoundsPlayer : ModPlayer
    {
        public static float savedVolume = -1f;

        private bool playedPoseSound = false;
        private SoundEffectInstance biteTheDustAmbienceSFX;
        private SoundEffectInstance timeskipAmbienceSFX;
        private bool specialMoveEffectPlaying = false;

        public override void Initialize()
        {
            timeskipAmbienceSFX = ModContent.GetSound("JoJoStandsSounds/Sounds/SoundEffects/KCTSSFX").CreateInstance();
            biteTheDustAmbienceSFX = ModContent.GetSound("JoJoStandsSounds/Sounds/SoundEffects/KQBTDSFX").CreateInstance();    
        }

        public override void ResetEffects()
        {
            specialMoveEffectPlaying = false;
        }

        public override void PreUpdate()
        {
            MyPlayer mPlayer = player.GetModPlayer<MyPlayer>();
            if (JoJoStandsSounds.continuousBarrageSounds)
                mPlayer.standHitTime = 2;

            if (mPlayer.poseMode && player.whoAmI == Main.myPlayer)
            {
                if (mPlayer.poseDuration < 200 && !playedPoseSound && mPlayer.poseSoundName != "")
                {
                    string soundPath = "Sounds/PoseQuotes/" + mPlayer.poseSoundName + JoJoStandsSounds.soundVersion;
                    Terraria.Audio.LegacySoundStyle sound = mod.GetLegacySoundSlot(SoundType.Custom, soundPath);
                    if (sound == null)      //This is for dub sounds that are missing, otherwise there shouldn't be a value for this if both dub and sub are missing
                    {
                        soundPath = "Sounds/PoseQuotes/" + mPlayer.poseSoundName + "_Sub";
                        sound = mod.GetLegacySoundSlot(SoundType.Custom, soundPath);
                    }
                    Main.PlaySound(sound, player.Center).Volume = MyPlayer.ModSoundsVolume;
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ModNetHandler.soundsSync.SendQuoteSound(256, player.whoAmI, soundPath, player.Center);
                    }
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
                biteTheDustAmbienceSFX.Volume = MyPlayer.ModSoundsVolume;
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
                timeskipAmbienceSFX.Volume = MyPlayer.ModSoundsVolume;
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

            if (Main.netMode != NetmodeID.SinglePlayer && JoJoStandsSounds.syncSounds)
            {
                for (int i = 0; i < JoJoStandsSounds.soundInstances.Count; i++)
                {
                    SoundsHelper.PlaySound(JoJoStandsSounds.soundInstances[i], JoJoStandsSounds.soundStates[i], JoJoStandsSounds.soundPositions[i], JoJoStandsSounds.soundTravelDistances[i]);
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)       //that 1 last frame before you completely die
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (MyPlayer.DeathSoundID == 6)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Deathsounds/Killer"));
                }
            }
            return true;
        }
    }
}