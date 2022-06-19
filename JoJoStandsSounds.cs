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
        public static string soundVersion = "_Sub";
        public static bool syncSounds = false;
        public static bool continuousBarrageSounds = false;

        public static Dictionary<string, SoundData> activeSounds = new Dictionary<string, SoundData>();

        public struct SoundData
        {
            public int travelDistance;
            public Vector2 position;
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
        }

        public override void Close()
        {
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
            soundVersion = null;
            syncSounds = false;
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
                    if (syncSounds)
                    {
                        int sender = Convert.ToInt32(args[1]);
                        string soundPath = args[2] as string;
                        SoundState state = (SoundState)Convert.ToSingle(args[3]);
                        Vector2 pos = new Vector2(Convert.ToInt32(args[4]), Convert.ToInt32(args[5]));
                        int travelDist = Convert.ToInt32(args[6]);

                        ModNetHandler.soundsSync.PlaySoundInstance(-1, sender, soundPath, state, pos, travelDist);
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