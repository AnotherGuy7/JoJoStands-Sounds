using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.ModLoader;

namespace JoJoStandsSounds.Sounds.BattleCries
{
	public abstract class BarrageSounds : ModSound
	{
		//public virtual BarrageSounds beginningSound { get; } = null;

		public static SoundEffectInstance soundInstance2;
		//private SoundEffectInstance beginningSoundInstance = null;

		//private static bool playedBeginning = false;

		public override SoundEffectInstance PlaySound(ref SoundEffectInstance soundInstance, float volume, float pan, SoundType type)
		{
			//soundInstance2 = soundInstance;
			if (soundInstance.State == SoundState.Playing)
				return null;
			/*if (!playedBeginning)		//controls whether or not to play the sound based on if the beginning portion of the sound played (Commented out cause this isn't an Update())
			{
				Main.NewText("Run");
				if (beginningSound != null && beginningSoundInstance == null)
				{
					beginningSoundInstance = beginningSound.sound.CreateInstance();
					beginningSoundInstance.Play();
					Main.NewText("Play");
				}
				if (beginningSound == null)
				{
					playedBeginning = true;
				}
				if (beginningSoundInstance != null)
				{
					if (beginningSoundInstance.State == SoundState.Playing)
					{
						Main.NewText("Null");
						return null;
					}
					if (beginningSoundInstance.State == SoundState.Stopped)
					{
						Main.NewText("Done");
						playedBeginning = true;
					}
				}
			}*/
			/*Main.NewText(playedBeginning);
			if (beginningSound != null && !playedBeginning)		//if there's a beginning sound, that will play instead
			{
				Main.NewText("p");
				beginningSoundInstance = beginningSound.sound.CreateInstance();
				beginningSoundInstance.Play();
				playedBeginning = true;
				return null;
			}*/
			/*if (beginningSoundInstance != null)
			{
				if (beginningSoundInstance.State == SoundState.Stopped)
				{
					Main.NewText("Null");
					playedBeginning = true;
				}
			}*/


			return soundInstance;
		}
		
		/*public static void StopPlayingSound()		//gets the soundInstance and stops it, obviously
		{
			if (soundInstance2 != null)		//to prevernt those Object isntance errors
			{
				soundInstance2.Stop();
			}
			//playedBeginning = false;
			JoJoStands.JoJoStands.killSounds = false;
		}*/
	}
}