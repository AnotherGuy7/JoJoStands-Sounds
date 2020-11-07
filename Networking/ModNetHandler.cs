using System.IO;

namespace JoJoStandsSounds.Networking
{
	public class ModNetHandler
	{
		public const byte Sounds = 0;

		public static SoundsPacketHandler soundsSync = new SoundsPacketHandler(Sounds);

		public static void HandlePacket(BinaryReader reader, int fromWho)
		{
			byte messageType = reader.ReadByte();
			switch (messageType)
			{
				case Sounds:
					soundsSync.HandlePacket(reader, fromWho);
					break;
			}
		}
	}
}