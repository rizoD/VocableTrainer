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

		static readonly object lockObj = new object();

		// If modifying these scopes, delete your previously saved credentials
		// at ~/.credentials/drive-dotnet-quickstart.json
		static string[] Scopes = { DriveService.Scope.DriveReadonly };
		static string ApplicationName = "VocableTrainer";


		// aapt resource value: 0x7F0D0000
		public const int Error_Short = 2131558400;

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

		public class Controls
		{
			public static void PlayPause(bool rc)
			{
				ExecAction(Settings.PlayPauseAction, rc);

			}

			public static void Next(bool rc)
			{
				ExecAction(Settings.NextAction, rc);

			}

			public static void Prev(bool rc)
			{
				ExecAction(Settings.PrevAction, rc);
			}

			private static void ExecAction(ControlAction action, bool rc)
			{
				switch (action)
				{
					case ControlAction.PlayPause:
						Task.Run(() => App.TogglePlay(rc));
						break;
					case ControlAction.Next:
						Task.Run(() => App.PlayNextAudio());
						break;
					case ControlAction.Repeat:
						Task.Run(() => App.Replay(rc));
						break;
					case ControlAction.Flag:
						Task.Run(() => App.Flag(rc));
						break;
					case ControlAction.Wrong:
						Task.Run(() => App.FlagWrong(rc));
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// Plays the chime sound given
		/// 
		/// We can interact via the app or remotely via the media control buttons (Bluetooth)
		/// if we do it remotely we might want to play a sound to indicate that the action was successfull
		/// so we added the parameter `rcPlay` to know where the action originated from
		/// </summary>
		/// <param name="rcPlay">this shows if the action was trigger remotely</param>
		/// <param name="resource">the resource to play</param>
		private static void PlayChime(bool rcPlay, int resource)
		{
			if (rcPlay && Settings.PlayRcChime || Settings.AllwaysPlayChime)
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

		private static void ResetRecallScore()
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


		private static void FlagWrong(bool rc = false)
		{
			try
			{
				PlayChime(rc, Error_Short);
				ResetRecallScore();

				ForwardTimeToPlayNext(); // if we know that we got it wrong we forward time to play the next sound
			}
			catch (Exception ex)
			{

			}
		}

		private static void Flag(bool rc = false)
		{
			try
			{
				PlayChime(rc, Error_Short);
				// we simply reuse the Flag function to Reset the Recall Score
				ResetRecallScore();

				ForwardTimeToPlayNext(); // if we know that we got it wrong we forward time to play the next sound
				return;
				// this flaggs the vocalbe (was used to flag "wrong" vocalbes)
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

		private static void TimerCallback(object state)
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
			Data.SaveTrainingState();
			Data.TrainingSound.Clear();
			Data.State = PlayState.Pause;

		}

		private static void Next()
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

		private static DateTime lastPlayed = DateTime.Now; // used to show when the last audio file was played
		private static bool working = false; // used to flag if we currently play an audio file

		/// <summary>
		/// Handles the training timeout
		/// during training this gets run every second an will the react accordingly
		/// </summary>
		public static void DoTraining()
		{
			// we can ignore this if no data, no traing, finished or currently doing something 
			if (Data == null ||
				Data.CurrentTraining == null ||
				Data.State == PlayState.Finished ||
				working)
			{
				return;
			}

			// if we are playing the training (not paused) and `AutoPlay` was enabled
			if (Data.State == PlayState.Playing && Data.CurrentTraining.AutoPlay)
			{
				DateTime expired = DateTime.MaxValue;
				lock (lockObj)
				{
					expired = lastPlayed.AddSeconds(App.Data.CurrentTraining.Pause);
				}
				// only do the next step after the appropriate pause time
				if (expired <= DateTime.Now)
				{
					// playing the audio takes time... therfore set the `working` flag.
					// this will make sure that we don't talk over each other
					working = true;
					PlayNextAudio();
					working = false;
				}
			}
		}

		/// <summary>
		/// fowards time to the next audio
		/// sets the `lastPlayed` time to be 1 sec before expiration...
		/// </summary>
		public static void ForwardTimeToPlayNext()
		{
			// we simply set the last played time to be one second before autoply time expires
			// this should fire the `PalyNextAudio` in the `DoTraining` function
			lock (lockObj)
			{
				lastPlayed = DateTime.Now.AddSeconds((App.Data.CurrentTraining.Pause - 1) * -1);
			}
		}

		/// <summary>
		/// Resets the auto playback timer to "start again"
		/// sets the `lastPalyed` time to the current time (so the `DoTraining` will wait again)
		/// </summary>
		public static void ResetAutoPlaybackTimer()
		{
			lock (lockObj)
			{
				lastPlayed = DateTime.Now;
			}
		}
		public static void Replay(bool rc = false)
		{
			try
			{
				PlayChime(rc, Play_Short);
				PlaySound(Data.LastTrainingSound);
				ResetAutoPlaybackTimer();
			}
			catch (Exception ex)
			{

			}
		}

		private static void PlayNextAudio()
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
				ResetAutoPlaybackTimer();
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
