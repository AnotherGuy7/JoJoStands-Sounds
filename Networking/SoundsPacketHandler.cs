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
        { }

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
                Main.PlaySound(JoJoStandsSounds.Instance.GetLegacySoundSlot(SoundType.Custom, soundPath), pos).Volume = MyPlayer.ModSoundsVolume;
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
                {
                    bool soundExists = false;
                    for (int i = 0; i < JoJoStandsSounds.soundPaths.Count; i++)
                    {
                        if (JoJoStandsSounds.soundPaths[i] == soundPath)
                        {
                            JoJoStandsSounds.soundStates[i] = state;
                            JoJoStandsSounds.soundPositions[i] = pos;
                            JoJoStandsSounds.soundTravelDistances[i] = travelDist;
                            soundExists = true;
                            break;
                        }
                    }
                    if (!soundExists)
                    {
                        JoJoStandsSounds.soundPaths.Add(soundPath);
                        JoJoStandsSounds.soundInstances.Add(JoJoStandsSounds.Instance.GetSound(soundPath).CreateInstance());
                        JoJoStandsSounds.soundStates.Add(state);
                        JoJoStandsSounds.soundPositions.Add(pos);
                        JoJoStandsSounds.soundTravelDistances.Add(travelDist);
                    }
                }
            }
            else
            {
                PlaySoundInstance(-1, fromWho, soundPath, state, pos, travelDist);
            }
        }
    }
}