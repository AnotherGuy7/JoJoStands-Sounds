using JoJoStands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using ReLogic.Content;
using System.IO;
using Terraria;
using Terraria.Audio;
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
                SoundStyle sound = new SoundStyle(soundPath);
                sound.Volume = JoJoStands.JoJoStands.ModSoundsVolume;
                SoundEngine.PlaySound(sound, pos);
            }
            else
            {
                SendQuoteSound(-1, fromWho, soundPath, pos);
            }
        }

        public void SendSoundInstance(int toWho, int fromWho, string soundsPath, SoundState state, Vector2 pos, byte travelDistance)
        {
            ModPacket packet = GetPacket(SoundInstances, fromWho);
            packet.Write(soundsPath);
            packet.Write((byte)state);
            packet.Write((int)pos.X * 100);
            packet.Write((int)pos.Y * 100);
            packet.Write(travelDistance);
            packet.Send(toWho, fromWho);
        }

        public void ReceiveSoundInstance(BinaryReader reader, int fromWho)
        {
            string soundPath = reader.ReadString();
            SoundState state = (SoundState)reader.ReadByte();
            Vector2 pos = new Vector2(reader.ReadInt32(), reader.ReadInt32()) / 100f;
            byte travelDist = reader.ReadByte();
            if (Main.netMode != NetmodeID.Server)
            {
                if (JoJoStandsSounds.SyncSounds)
                {
                    if (JoJoStandsSounds.activeSounds.ContainsKey(soundPath))
                    {
                        JoJoStandsSounds.SoundData soundData = JoJoStandsSounds.activeSounds[soundPath];
                        soundData.state = state;
                        soundData.position = pos;
                        soundData.travelDistance = travelDist;
                        JoJoStandsSounds.activeSounds[soundPath] = soundData;
                    }
                    else
                    {
                        JoJoStandsSounds.SoundData newSoundData = new JoJoStandsSounds.SoundData();
                        newSoundData.instance = ModContent.Request<SoundEffect>(soundPath, AssetRequestMode.ImmediateLoad).Value.CreateInstance();
                        newSoundData.state = state;
                        newSoundData.position = pos;
                        newSoundData.travelDistance = travelDist;
                        JoJoStandsSounds.activeSounds.Add(soundPath, newSoundData);
                    }
                }
            }
            else
            {
                SendSoundInstance(-1, fromWho, soundPath, state, pos, travelDist);
            }
        }
    }
}