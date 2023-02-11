using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace VocableTrainer
{
	public static class Settings
	{
		static ISettings AppSettings
		{
			get { return CrossSettings.Current; }
		}

		public static bool AllwaysPlayChime
		{
			get => AppSettings.GetValueOrDefault(nameof(AllwaysPlayChime), true);
			set => AppSettings.AddOrUpdateValue(nameof(AllwaysPlayChime), value);
		}

		public static bool PlayRcChime
		{
			get => AppSettings.GetValueOrDefault(nameof(PlayRcChime), true);
			set => AppSettings.AddOrUpdateValue(nameof(PlayRcChime), value);
		}

		public static int LastTraining
		{
			get => AppSettings.GetValueOrDefault(nameof(LastTraining), 0);
			set => AppSettings.AddOrUpdateValue(nameof(LastTraining), value);
		}

		public static int CurrentLanguage
		{
			get => AppSettings.GetValueOrDefault(nameof(CurrentLanguage), 0);
			set => AppSettings.AddOrUpdateValue(nameof(CurrentLanguage), value);
		}

		public static string NativeVoice
		{
			get => AppSettings.GetValueOrDefault(nameof(NativeVoice), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(NativeVoice), value);
		}

		public static void ClearAllData()
		{
			AppSettings.Clear();
		}
	}
}
