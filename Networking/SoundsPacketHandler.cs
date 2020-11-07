using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace JoJoStandsSounds.Networking
{
    public class SoundsPacketHandler : PacketHandler
    {
        public const byte QuoteSounds = 0;
        public const byte SoundInstances = 1;

        public SoundsPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)     //decides what happens when a packet is received, it looks for the byte sent with the packet and uses the proper method
        {
            byte messageType = reader.ReadByte();
            switch (messageType)
            {
                case QuoteSounds:
                    ReceiveQuoteSound(reader, fromWho);
                    break;
                case SoundInstances:
                    ReceiveSoundInstance(reader, fromWho);
                    break;
            }
        }

        public void SendQuoteSound(int toWho, int fromWho, string soundsPath, Vector2 pos)
        {
            ModPacket packet = GetPacket(QuoteSounds, fromWho);
            packet.Write(soundsPath);
            packet.WriteVector2(pos);
            packet.Send(toWho, fromWho);
        }

        public void ReceiveQuoteSound(BinaryReader reader, int fromWho)
        {
            string soundPath = reader.ReadString();
            Vector2 pos = reader.ReadVector2();
            if (Main.netMode != NetmodeID.Server)
            {
                Main.PlaySound(JoJoStandsSounds.Instance.GetLegacySoundSlot(SoundType.Custom, soundPath), pos);
            }
            else
            {
                SendQuoteSound(-1, fromWho, soundPath, pos);
            }
        }

        public void PlaySoundInstance(int toWho, int fromWho, string soundsPath, SoundState state, Vector2 pos, int travelDistance)
        {
            ModPacket packet = GetPacket(SoundInstances, fromWho);
            packet.Write(soundsPath);
            packet.Write((int)state);
            packet.WriteVector2(pos);
            packet.Write(travelDistance);
            packet.Send(toWho, fromWho);
        }

        public void ReceiveSoundInstance(BinaryReader reader, int fromWho)
        {
            string soundPath = reader.ReadString();
            SoundState state = (SoundState)reader.ReadInt32();
            Vector2 pos = reader.ReadVector2();
            int travelDist = reader.ReadInt32();
            if (Main.netMode != NetmodeID.Server)
            {
                if (JoJoStandsSounds.syncSounds)
                    PlaySound(soundPath, state, pos, travelDist);
            }
            else
            {
                PlaySoundInstance(-1, fromWho, soundPath, state, pos, travelDist);
            }
        }

        private void PlaySound(string soundPath, SoundState state, Vector2 position, int soundTravelDistance = 10)
        {
            Player player = Main.player[Main.myPlayer];
            int travelDist = soundTravelDistance * 16;
            float distanceFromSource = MathHelper.Clamp(Vector2.Distance(player.position, position) - 64, 0, travelDist);       //-64 so that if the player gets close enough the volume doesn't go higher but instead stays at that volume
            SoundEffectInstance sound = JoJoStandsSounds.Instance.GetSound(soundPath).CreateInstance();

            bool soundExists = false;
            for (int i = 0; i < JoJoStandsSounds.soundInstances.Count; i++)
            {
                if (JoJoStandsSounds.soundInstances[i].ToString() == sound.ToString())
                {
                    sound.Dispose();
                    sound = JoJoStandsSounds.soundInstances[i];
                    soundExists = true;
                    break;
                }
            }

            //Main.NewText("Dist: " + (Vector2.Distance(player.position, position) - 64) + "DFS: " + distanceFromSource);
            sound.Volume = (travelDist - distanceFromSource) * MyPlayer.soundVolume;
            if (sound.Volume != 0f)
            {
                if (!soundExists)
                {
                    if (state != SoundState.Stopped)
                    {
                        Main.PlaySoundInstance(sound);
                    }
                    JoJoStandsSounds.soundInstances.Add(sound);
                    //Main.NewText("Sound Added!");
                }
                else
                {
                    if (state == SoundState.Playing)
                    {
                        Main.PlaySoundInstance(sound);
                        //Main.NewText("Sound is playing!");
                    }
                    else if (state == SoundState.Stopped)
                    {
                        sound.Stop();
                        //Main.NewText("Sound has stopped.");
                    }
                }
            }
        }
    }
}