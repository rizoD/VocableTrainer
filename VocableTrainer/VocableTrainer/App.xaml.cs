using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
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

		public static void TogglePlay()
		{
			if (Data.State == PlayState.Playing)
			{
				Pause();
			}
			else
			{
				Play();
			}
		}

		public static void Play()
		{
			Data.SaveTrainingState();
			Data.TrainingSound.Add(Sound.Lang.Native);
			Data.TrainingSound.Add(Sound.Lang.Foreign);
			Data.State = PlayState.Playing;
		}

		public static void Restart()
		{
			Data.ShuffleTraining();
			Data.SaveTrainingState();
			Data.TrainingSound.Add(Sound.Lang.Native);
			Data.TrainingSound.Add(Sound.Lang.Foreign);
			Data.State = PlayState.Playing;

		}

		public static void Pause()
		{
			Data.TrainingSound.Clear();
			Data.State = PlayState.Pause;
		}

		public static void Next()
		{
			if (Data.Trainings.Count > 0)
			{
				int i = (Data.Trainings.IndexOf(Data.CurrentVocable) + 1) % Data.Trainings.Count;
				Data.CurrentVocable = Data.Trainings[i];
				Settings.LastTraining = Data.CurrentVocable.Id;
				if (!Data.CurrentTraining.AutoPlay)
				{
					var current = Data.TrainingSound.OrderBy(item => Guid.NewGuid()).FirstOrDefault();
					PlaySound(current);
				}
			}
		}
		private static bool IsLastTrainingVocable()
		{
			return (Data.Trainings.IndexOf(Data.CurrentVocable) + 1) >= Data.Trainings.Count;
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
				Settings.LastTraining = Data.CurrentVocable.Id;
				if (!Data.CurrentTraining.AutoPlay)
				{
					var current = Data.TrainingSound.OrderBy(item => Guid.NewGuid()).FirstOrDefault();
					PlaySound(current);
				}
			}
		}

		public static T GetResult<T>(Task<T> task)
		{
			task.Wait(TimeSpan.FromSeconds(5));
			return task.Result;
		}

		private static DateTime lastChange = DateTime.Now;
		private static Random rnd = new Random(DateTime.Now.Millisecond);

		public static void DoTraining()
		{
			if (Data == null || Data.CurrentTraining == null)
			{
				return;
			}

			if (Data.State == PlayState.Playing && Data.CurrentTraining.AutoPlay)
			{
				if (lastChange.AddSeconds(App.Data.CurrentTraining.Pause) <= DateTime.Now)
				{
					if (Data.TrainingSound.Count == 0)
					{
						if (IsLastTrainingVocable())
						{
							Data.State = PlayState.Finished;
							return;
						}

						Data.TrainingSound.Add(Sound.Lang.Native);
						Data.TrainingSound.Add(Sound.Lang.Foreign);

						Next();
					}

					var current = Data.TrainingSound.OrderBy(item => Guid.NewGuid()).FirstOrDefault();
					Data.TrainingSound.Remove(current);

					PlaySound(current);
					if (!Data.CurrentTraining.PlayAnswer)
					{
						Data.TrainingSound.Clear();
					}

					lastChange = DateTime.Now;
				}
			}
			else
			{
				lastChange = DateTime.Now;
			}
		}

	}

}
