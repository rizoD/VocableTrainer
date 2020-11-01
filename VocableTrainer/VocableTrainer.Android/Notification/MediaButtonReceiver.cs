using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Sql;
using Xamarin.Forms;

namespace VocableTrainer
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] {
		Intent.ActionMediaButton
	})]
	public class MediaButtonReceiver : BroadcastReceiver
	{
		private static DateTime lastBtnPress = DateTime.Now;

		public string ComponentName { get { return Class.Name; } }

		public override void OnReceive(Context context, Intent intent)
		{
			//Btn Presses seem to need a debounce
			if (lastBtnPress.AddSeconds(2) > DateTime.Now)
			{
				return;
			}

			if (intent.Action != Intent.ActionMediaButton)
			{
				//Toast.MakeText(context, "Not a Media Button", ToastLength.Short).Show();
				return;
			}

			var keyEvent = (KeyEvent)intent.GetParcelableExtra(Intent.ExtraKeyEvent);

			//Toast.MakeText(context, "Key: "+ keyEvent, ToastLength.Short).Show();

			switch (keyEvent.KeyCode)
			{
				case Keycode.MediaPause:
				case Keycode.MediaPlay:
				case Keycode.MediaPlayPause:
					App.TogglePlay(); 
					break;
				case Keycode.MediaNext:
					App.PlayNextAudio();
					break;
				case Keycode.MediaPrevious:
					App.TrainingFlag();
					break;
			}
			lastBtnPress = DateTime.Now;

		}
	}
}
