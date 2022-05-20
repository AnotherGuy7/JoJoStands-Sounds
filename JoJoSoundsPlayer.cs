using JoJoStands;
using JoJoStandsSounds.Networking;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStandsSounds
{
    public class JoJoSoundsPlayer : ModPlayer
    {
        public static float savedVolume = -1f;

        private bool playedPoseSound = false;
        private bool specialMoveEffectPlaying = false;
        private SoundEffectInstance biteTheDustAmbienceSFX;
        private SoundEffectInstance timeskipAmbienceSFX;

        public override void Initialize()
        {
            timeskipAmbienceSFX = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/SoundEffects/KCTSSFX", AssetRequestMode.ImmediateLoad).Value.CreateInstance();
            biteTheDustAmbienceSFX = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/SoundEffects/KQBTDSFX", AssetRequestMode.ImmediateLoad).Value.CreateInstance();    
        }

        public override void ResetEffects()
        {
            specialMoveEffectPlaying = false;
        }

        public override void PreUpdate()
        {
            MyPlayer mPlayer = Player.GetModPlayer<MyPlayer>();
            if (JoJoStandsSounds.continuousBarrageSounds)
                mPlayer.standHitTime = 2;

            if (mPlayer.poseMode && Player.whoAmI == Main.myPlayer)
            {
                if (mPlayer.poseDuration < 200 && !playedPoseSound && mPlayer.poseSoundName != "")
                {
                    string soundPath = "Sounds/PoseQuotes/" + mPlayer.poseSoundName + JoJoStandsSounds.soundVersion;
                    LegacySoundStyle sound = SoundLoader.GetLegacySoundSlot(Mod, soundPath);
                    if (sound == null)      //This is for dub sounds that are missing, otherwise there shouldn't be a value for this if both dub and sub are missing
                    {
                        soundPath = "Sounds/PoseQuotes/" + mPlayer.poseSoundName + "_Sub";
                        sound = SoundLoader.GetLegacySoundSlot(Mod, soundPath);
                    }
                    SoundEngine.PlaySound(sound, Player.Center).Volume = MyPlayer.ModSoundsVolume;
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