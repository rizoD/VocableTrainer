using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Widget;
using Google.Apis.Drive.v3;
using Plugin.SimpleAudioPlayer;
using VocableTrainer.Data;
using Xamarin.Forms;

namespace VocableTrainer
{
	public partial class App : Application, INotifyPropertyChanged
	{
		public static ViewModel Data = new ViewModel();

		// If modifying these scopes, delete your previously saved credentials
		// at ~/.credentials/drive-dotnet-quickstart.json
		static string[] Scopes = { DriveService.Scope.DriveReadonly };
		static string ApplicationName = "VocableTrainer";

		// aapt resource value: 0x7F0D0000
		public const int Flag_Short = 2131558400;

		// aapt resource value: 0x7F0D0001
		public const int Pause_Short = 2131558401;

		// aapt resource value: 0x7F0D0002
		public const int Play_Short = 2131558402;

		public App()
		{
			InitializeComponent();
			MainPage = new TrainingPage(); // new MainPage();
			Task.Run(Data.LoadFile);

		}

		private static void PlayChime(bool play, int resource)
		{
			if (play && Settings.PlayRcChime)
			{
				try
				{
					using (Stream stream = Android.App.Application.Context.Resources.OpenRawResource(resource))
					{
						CrossSimpleAudioPlayer.Current.Load(stream);
						CrossSimpleAudioPlayer.Current.Play();
						while (CrossSimpleAudioPlayer.Current.IsPlaying)
						{
							Thread.Sleep(100);
						}
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

		public static void ResetRecallScore()
		{
			try
			{
				Data.CurrentVocable.RecallScore = 0;
				Data.SaveVocable(Data.CurrentVocable, false);
			}
			catch (Exception ex)
			{

			}
		}
		public static void TrainingFlag(bool rc = false)
		{
			ResetRecallScore(); 
			return;

			// we simply reuse the Flag function to Reset the Recall Score
			try
			{
				PlayChime(rc, Flag_Short);

				Data.CurrentVocable.Flag |= Flags.Training;
				Data.SaveVocable(Data.CurrentVocable, false);
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
							PlayChime(rc, Pause_Short);
							Pause();
						}
						else
						{
							PlayChime(rc, Play_Short);
							Play();
						}
					}
					else
					{
						PlayChime(rc, Play_Short);
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

		public static void UpdateLang()
		{
			if (App.Data != null &&
				App.Data.CurrentLanguage != null)
			{
				Settings.CurrentLanguage = App.Data.CurrentLanguage.Id;
				App.Stop();
				App.Data.LoadVocables();
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
			Data.BlockShuffle(true);
			Data.SaveTrainingState();
			Data.TrainingSound.Add(Sound.Lang.Native);
			Data.TrainingSound.Add(Sound.Lang.Foreign);
			Data.State = PlayState.Playing;
		}

		public static void Stop()
		{
			Data.TrainingSound.Clear();
			Data.State = PlayState.Finished;
		}

		public static void Pause()
		{
			Data.TrainingSound.Clear();
			Data.State = PlayState.Pause;
		}

		public static void Next()
		{
			// before we get the next vocable we increase the RecallScore
			if (Data.CurrentVocable != null)
			{
				Data.CurrentVocable.RecallScore++;
				Data.SaveVocable(Data.CurrentVocable, false);
			}

			if (Data.Trainings.Count > 0)
			{
				// now we can get the next one
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
				Data.State == PlayState.Finished ||
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
				PlayChime(rc, Play_Short);
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
