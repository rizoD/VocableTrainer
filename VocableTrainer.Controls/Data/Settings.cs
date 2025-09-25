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
		
		public static ControlAction PrevAction
		{
			get => (ControlAction)AppSettings.GetValueOrDefault(nameof(PrevAction), (int)ControlAction.Repeat);
			set => AppSettings.AddOrUpdateValue(nameof(PrevAction), (int)value);
		}

		public static ControlAction PlayPauseAction
		{
			get => (ControlAction)AppSettings.GetValueOrDefault(nameof(PlayPauseAction), (int)ControlAction.PlayPause);
			set => AppSettings.AddOrUpdateValue(nameof(PlayPauseAction), (int)value);
		}

		public static ControlAction NextAction
		{
			get => (ControlAction)AppSettings.GetValueOrDefault(nameof(NextAction), (int)ControlAction.Next);
			set => AppSettings.AddOrUpdateValue(nameof(NextAction), (int)value);
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
