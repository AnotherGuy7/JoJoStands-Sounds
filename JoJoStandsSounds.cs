using JoJoStandsSounds.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace JoJoStandsSounds
{
    public class JoJoStandsSounds : Mod
    {
        public static string SoundVersion = "_Sub";
        public static bool SyncSounds = false;
        public static bool ContinuousBarrageSounds = false;

        public readonly byte Theme_PowerInstall2 = 0;
        public readonly byte Theme_PowerInstall3 = 1;
        public readonly byte Theme_PowerInstall4 = 2;

        public static SoundEffectInstance[] StandThemes;

        public static Dictionary<string, SoundData> activeSounds = new Dictionary<string, SoundData>();

        public struct SoundData
        {
            public Vector2 position;
            public int travelDistance;
            public SoundEffectInstance instance;
            public SoundState state;
        }

        public static JoJoStandsSounds Instance;

        public static SoundEffectInstance biteTheDustAmbienceSFX;
        public static SoundEffectInstance timeskipAmbienceSFX;

        public override void Load()
        {
            Instance = ModContent.GetInstance<JoJoStandsSounds>();
            if (Main.dedServ)
                return;

            timeskipAmbienceSFX = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/SoundEffects/KCTSSFX", AssetRequestMode.ImmediateLoad).Value.CreateInstance();
            biteTheDustAmbienceSFX = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/SoundEffects/KQBTDSFX", AssetRequestMode.ImmediateLoad).Value.CreateInstance();

            StandThemes = new SoundEffectInstance[3];
            StandThemes[0] = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/Themes/VirtualInsanity/PowerInstall_2", AssetRequestMode.ImmediateLoad).Value.CreateInstance();
            StandThemes[1] = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/Themes/VirtualInsanity/PowerInstall_3", AssetRequestMode.ImmediateLoad).Value.CreateInstance();
            StandThemes[2] = ModContent.Request<SoundEffect>("JoJoStandsSounds/Sounds/Themes/VirtualInsanity/PowerInstall_4", AssetRequestMode.ImmediateLoad).Value.CreateInstance();
        }

        public override void Close()
        {
            if (activeSounds != null)
                activeSounds.Clear();
            if (timeskipAmbienceSFX != null)
            {
                timeskipAmbienceSFX.Stop();
                biteTheDustAmbienceSFX.Stop();
            }

            base.Close();
        }

        public override void Unload()
        {
            SoundVersion = null;
            SyncSounds = false;
            activeSounds = null;
            timeskipAmbienceSFX = null;
            biteTheDustAmbienceSFX = null;
            JoJoSoundsPlayer.savedVolume = -1f;
            ModNetHandler.soundsSync = null;
            Instance = null;
        }

        public override object Call(params object[] args)
        {
            string methodName = args[0] as string;
            switch (methodName)
            {
                case "SendSoundInstance":
                    if (SyncSounds)
                    {
                        int sender = Convert.ToInt32(args[1]);
                        string soundPath = args[2] as string;
                        SoundState state = (SoundState)Convert.ToSingle(args[3]);
                        Vector2 pos = new Vector2(Convert.ToInt32(args[4]), Convert.ToInt32(args[5]));
                        byte travelDist = (byte)MathHelper.Clamp(Convert.ToInt32(args[6]), 0, 255);

                        ModNetHandler.soundsSync.SendSoundInstance(-1, sender, soundPath, state, pos, travelDist);
                    }
                    break;
            }
            return null;
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }
    }
}