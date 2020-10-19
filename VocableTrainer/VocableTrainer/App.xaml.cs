using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Google.Apis.Drive.v3;
using Plugin.FilePicker.Abstractions;
using Xamarin.Forms;
using Uri = Android.Net.Uri;

namespace VocableTrainer
{
	public partial class App : Application, INotifyPropertyChanged
	{
		public static ViewModel Data = new ViewModel();

		// If modifying these scopes, delete your previously saved credentials
		// at ~/.credentials/drive-dotnet-quickstart.json
		static string[] Scopes = { DriveService.Scope.DriveReadonly };
		static string ApplicationName = "VocableTrainer";

		public App()
		{
			InitializeComponent();
			MainPage = new TrainingPage(); // new MainPage();
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}

		public static async void Play(Sound.Lang type)
		{
			if (Data.CurrentTraining != null)
			{
				Sound sound = await Data.Database.GetSoundAsync(Data.CurrentTraining.Id, type);
				if (sound == null)
				{
					sound = SoundManager.CreateSound(Data.CurrentVocable, type);
					if (sound != null)
					{
						Data.SaveSound(sound);
					}
				}

				if (sound != null)
				{
					SoundManager.Play(sound);
				}
			}
		}

		public static void PlayPause()
		{
		}

		public static void Next()
		{
			if (Data.Trainings.Count > 0)
			{
				int i = (Data.Trainings.IndexOf(Data.CurrentTraining) + 1) % Data.Trainings.Count;
				Data.CurrentTraining = Data.Trainings[i];
				Data.CurrentVocable = Data.CurrentTraining;
			}
		}

		public static void Prev()
		{
			if (Data.Trainings.Count > 0)
			{
				int i = Data.Trainings.IndexOf(Data.CurrentTraining) - 1;
				if (i < 0)
				{
					i = Data.Trainings.Count - 1;
				}

				Data.CurrentTraining = Data.Trainings[i];
				Data.CurrentVocable = Data.CurrentTraining;
			}
		}

	}
}
