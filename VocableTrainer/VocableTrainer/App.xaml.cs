using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Android.Content;
using Android.Media.Audiofx;
using Google.Apis.Drive.v3;
using Plugin.FilePicker.Abstractions;
using VocableTrainer.Data;
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

		public static void TrainingFlag()
		{
			Data.CurrentVocable.Flag |= Flags.Training;
			Data.SaveVocable(Data.CurrentVocable);
		}

		public static void PlaySound(Sound.Lang type)
		{
			if (Data.CurrentVocable != null)
			{
				Sound sound = Data.Database.GetSound(Data.CurrentVocable.Id, type);
				if (sound == null)
				{
					Data.UpdateCurrentSound(type);
				}

				if (sound != null)
				{
					SoundManager.Play(sound);
				}
			}
		}

		public static void Play()
		{
			Data.State = PlayState.Playing;
			Data.SaveTrainingState();
		}

		public static void Restart()
		{
			Data.State = PlayState.Playing;
			Data.ShuffleTraining();
			Data.SaveTrainingState();
		}

		public static void Pause()
		{
			Data.State = PlayState.Pause;
		}

		public static void Next()
		{
			if (Data.Trainings.Count > 0)
			{
				int i = (Data.Trainings.IndexOf(Data.CurrentVocable) + 1) % Data.Trainings.Count;
				Data.CurrentVocable = Data.Trainings[i];
				Settings.LastTraining = i;
			}
		}

		public static void Prev()
		{
			if (Data.Trainings.Count > 0)
			{
				int i = Data.Trainings.IndexOf(Data.CurrentVocable) - 1;
				if (i < 0)
				{
					i = Data.Trainings.Count - 1;
				}

				Data.CurrentVocable = Data.Trainings[i];
				Settings.LastTraining = i;
			}
		}

		public static T GetResult<T>(Task<T> task)
		{
			task.Wait(TimeSpan.FromSeconds(5));
			return task.Result;
		}

	}
}
