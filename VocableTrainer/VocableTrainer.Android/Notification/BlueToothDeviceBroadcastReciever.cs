using System.Linq;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Widget;

namespace VocableTrainer
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[]
	{
		BluetoothA2dp.ActionConnectionStateChanged,
		BluetoothA2dp.ActionPlayingStateChanged
	})]
	public class BlueToothDeviceBroadcastReciever : BroadcastReceiver
	{
		const int STATE_DISCONNECTED = 0;
		const int STATE_CONNECTING = 1;
		const int STATE_CONNECTED = 2;
		const int STATE_DISCONNECTING = 3;
		const int STATE_NOT_PLAYING = 11;
		const int STATE_PLAYING = 10;

		public override void OnReceive(Context context, Intent intent)
		{
			if (BluetoothA2dp.ActionConnectionStateChanged.Equals(intent.Action))
			{
				var conDevices =
					BluetoothAdapter.DefaultAdapter.BondedDevices.Where(item => item.BondState == Bond.Bonded);
				int state = intent.GetIntExtra(BluetoothAdapter.ExtraConnectionState, 0);
				Toast.MakeText(context, "ConState: "+state.ToString(), ToastLength.Short).Show();

				state = intent.GetIntExtra(BluetoothAdapter.ExtraState, 0);
				Toast.MakeText(context, "State: " + state.ToString(), ToastLength.Short).Show();
			}

			if (BluetoothA2dp.ActionPlayingStateChanged.Equals(intent.Action))
			{
				Toast.MakeText(context, "PlayingStateChanged", ToastLength.Short).Show();
			}

		}
	}
}
