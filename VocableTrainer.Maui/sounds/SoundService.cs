using Plugin.Maui.Audio;

namespace VocableTrainer.Maui.sounds
{
	class SoundService
	{
		private static Lazy<SoundService> _lazy = new Lazy<SoundService>(() => new SoundService());
		public static void PlayStream(Stream audio)
		{
			_lazy.Value.PlayStream_Internal(audio);
		}

		private void PlayStream_Internal(Stream audio)
		{
			try
			{
				var player = AudioManager.Current.CreatePlayer(audio);
				player.Play();

				while (player.IsPlaying)
				{
					Thread.Sleep(100);
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}
	}
}
