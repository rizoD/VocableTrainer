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
		private static int cnt = 0;

		public string ComponentName { get { return Class.Name; } }

		public override void OnReceive(Context context, Intent intent)
		{
		

			if (intent.Action != Intent.ActionMediaButton)
			{
				//Toast.MakeText(context, "Not a Media Button", ToastLength.Short).Show();
				return;
			}
			cnt = (cnt + 1) % 2;

			//Btn Presses seem to need a debounce
			if (cnt == 0)
			{
				return;
			}
			var keyEvent = (KeyEvent)intent.GetParcelableExtra(Intent.ExtraKeyEvent);

			//Toast.MakeText(context, "Key: "+ keyEvent, ToastLength.Short).Show();

			switch (keyEvent.KeyCode)
			{
				case Keycode.MediaPause:
				case Keycode.MediaPlay:
				case Keycode.MediaPlayPause:
					Task.Run(() =>
					{
						App.Controls.PlayPause(true);
					}); 
					break;
				case Keycode.MediaNext:
					Task.Run(() =>
					{
						// Thread.Sleep(2000); this was most certainly here because of the "Forward" announcement of the tozo 6 headphones
						App.Controls.Next(true);
					});
					break;
				case Keycode.MediaPrevious:
					Task.Run(() =>
					{
						App.Controls.Prev(true);
					});
					break;
			}
		}
	}
}
