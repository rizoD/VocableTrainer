using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.Content;
using Android.Media.Audiofx;
using Android.Widget;
using Google.Apis.Drive.v3;
using Plugin.FilePicker.Abstractions;
using Plugin.SimpleAudioPlayer;
using VocableTrainer.Data;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
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

		private static string FlagResource = "Flag_Short.mp3";
		private static string PlayResource = "Play_Short.mp3";
		private static string PauseResource = "Pause_Short.mp3";

		public App()
		{
			InitializeComponent();
			MainPage = new TrainingPage(); // new MainPage();

		}

		private static void PlayChime(bool play, string resource)
		{
			if (play)
			{
				try
				{
					using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("VocableTrainer." + resource))
					{
						CrossSimpleAudioPlayer.Current.Load(stream);
						CrossSimpleAudioPlayer.Current.Play();
					}
				}
				catch (Exception ex)
				{
					ex.ToString();
				}
			}
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

		public static void TrainingFlag(bool rc = false)
		{
			try
			{
				PlayChime(rc, FlagResource);

				Data.CurrentVocable.Flag |= Flags.Training;
				Data.SaveVocable(Data.CurrentVocable);
			}
			catch (Exception ex)
			{

			}
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

		public static void TogglePlay(bool rc = false)
		{
			try
			{
				
				if (Data.CurrentTraining != null)
				{
					if (Data.CurrentTraining.AutoPlay)
					{
						if (Data.State == PlayState.Playing)
						{
							PlayChime(rc, PauseResource);
							Pause();
						}
						else
						{
							PlayChime(rc, PlayResource);
							Play();
						}
					}
					else
					{
						PlayChime(rc, PlayResource);
						PlayNextAudio();
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		public static void TimerCallback(object state)
		{
			try
			{
				Task.Run(() =>
				{
					App.DoTraining();
				});
			}
			catch (Exception ex)
			{
				ex.ToString();
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
			}
		}

		public static T GetResult<T>(Task<T> task)
		{
			task.Wait(TimeSpan.FromSeconds(5));
			return task.Result;
		}

		private static DateTime lastChange = DateTime.Now;
		private static Random rnd = new Random(DateTime.Now.Millisecond);
		private static bool working = false;

		public static void DoTraining()
		{
			if (Data == null ||
				Data.CurrentTraining == null ||
				working)
			{
				return;
			}

			if (Data.State == PlayState.Playing && Data.CurrentTraining.AutoPlay)
			{
				if (lastChange.AddSeconds(App.Data.CurrentTraining.Pause) <= DateTime.Now)
				{
					working = true;

					PlayNextAudio();

					working = false;
					lastChange = DateTime.Now;
				}
			}
		}

		public static void Replay(bool rc = false)
		{
			try
			{
				PlayChime(rc, PlayResource);
				PlaySound(Data.LastTrainingSound);
			}
			catch (Exception ex)
			{

			}
		}

		public static void PlayNextAudio()
		{
			try
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
				Data.LastTrainingSound = current;
				Data.TrainingSound.Remove(current);

				PlaySound(current);
				if (!Data.CurrentTraining.PlayAnswer)
				{
					Data.TrainingSound.Clear();
				}
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
		}

		public static void ShowLoading(Action action, Action ending)
		{
			Data.IsBusy = true;

			try
			{
				Task task = Task.Factory.StartNew(action);

				task.ContinueWith((tsk, obj) =>
				{
					if (tsk.Exception != null)
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							Data.IsBusy = false;
							Toast.MakeText(Android.App.Application.Context, $"Application Error: {tsk.Exception.Message}", ToastLength.Long).Show();
						});
					}
					else
					{
						Device.BeginInvokeOnMainThread(() =>
						{
							Data.IsBusy = false;
							ending();
						});
					}
				}, new object());
			}
			catch (Exception ex)
			{
				Toast.MakeText(Android.App.Application.Context, $"Application Error: {ex.Message}", ToastLength.Long).Show();
			}
		}

	}

}
