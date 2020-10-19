using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
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
		public static string FilePath
		{
			get => AppSettings.GetValueOrDefault(nameof(FilePath), string.Empty);
			set => AppSettings.AddOrUpdateValue(nameof(FilePath), value);
		}
		public static int CurrentLanguage
		{
			get => AppSettings.GetValueOrDefault(nameof(CurrentLanguage), 0);
			set => AppSettings.AddOrUpdateValue(nameof(CurrentLanguage), value);
		}
		public static int Pause
		{
			get => AppSettings.GetValueOrDefault(nameof(Pause), 0);
			set => AppSettings.AddOrUpdateValue(nameof(Pause), value);
		}

		public static bool PlayAnswer
		{
			get => AppSettings.GetValueOrDefault(nameof(PlayAnswer), true);
			set => AppSettings.AddOrUpdateValue(nameof(PlayAnswer), value);
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
