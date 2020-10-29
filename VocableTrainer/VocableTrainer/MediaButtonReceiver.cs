using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.Views;

namespace VocableTrainer
{
	[BroadcastReceiver] //(Enabled = true, Exported = false)
	[IntentFilter(new[] { "android.intent.action.MEDIA_BUTTON" })] // , BluetoothHeadset.ActionAudioStateChanged, BluetoothHeadset.ActionVendorSpecificHeadsetEvent
	public class MediaButtonReceiver : BroadcastReceiver
	{
		public string ComponentName { get { return Class.Name; } }

		public override void OnReceive(Context context, Intent intent)
		{
			if (intent.Action != Intent.ActionMediaButton)
				return;

			var keyEvent = (KeyEvent)intent.GetParcelableExtra(Intent.ExtraKeyEvent);

			switch (keyEvent.KeyCode)
			{
				case Keycode.MediaPause:
					App.TogglePlay();
					break;
				case Keycode.MediaPlay:
					App.TogglePlay();
					break;
				case Keycode.MediaPlayPause:
					App.TogglePlay();
					break;
				case Keycode.MediaNext:
					App.Next();
					break;
				case Keycode.MediaPrevious:
					App.TrainingFlag();
					break;
			}
		}
	}
}
