using JoJoStandsSounds.Networking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStandsSounds
{
    public class JoJoStandsSounds : Mod
    {
        public static string soundVersion = "_Sub";
        public static bool syncSounds = false;
        internal static SoundsCustomizableOptions customizableConfig;

        public static List<SoundEffectInstance> soundInstances = new List<SoundEffectInstance>();

        public static JoJoStandsSounds Instance => ModContent.GetInstance<JoJoStandsSounds>();

        public override void Unload()
        {
            soundVersion = null;
            syncSounds = false;
            customizableConfig = null;
            soundInstances.Clear();
			ModNetHandler.soundsSync = null;
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

        /*public static void PlaySound(SoundEffectInstance sound, Vector2 position, float soundTravelDistance = 10f)
        {
            Player player = Main.player[Main.myPlayer];
            int travelDist = (int)(soundTravelDistance * 16f);
            float distanceFromSource = Vector2.Distance(player.position, position);
            sound.Volume = (distanceFromSource / travelDist) * MyPlayer.soundVolume; //Probably going to want to clamp distFromSource so it doesn't go under 0
            Main.PlaySoundInstance(sound);

            //Multiplayer sync stuff
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (syncSounds)
                {
                    ModNetHandler.playerSync.SendSoundInstance();
                }
            }
        }*/

        //Helper stuff
        /*public static void PlaySound(string soundPath, Vector2 position, float soundTravelDistance = 10f)     //This'll probably not be used
        {
            Player player = Main.player[Main.myPlayer];
            int travelDist = (int)(soundTravelDistance * 16f);
            float distanceFromSource = Vector2.Distance(player.position, position);
            SoundEffectInstance sound = Instance.GetSound(soundPath).CreateInstance();
            sound.Volume = (distanceFromSource / travelDist) * MyPlayer.soundVolume; //Probably going to want to clamp distFromSource so it doesn't go under 0
            Main.PlaySoundInstance(sound);

            //Multiplayer sync stuff
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                if (syncSounds && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModNetHandler.playerSync.SendSoundInstance(256, player.whoAmI, soundPath, position);
                }
            }
        }*/
    }
}