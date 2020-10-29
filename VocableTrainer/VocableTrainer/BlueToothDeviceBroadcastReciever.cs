using Android.App;
using Android.Bluetooth;
using Android.Content;

namespace VocableTrainer
{
	[BroadcastReceiver]
	[IntentFilter(new[] { BluetoothDevice.ActionFound })]
	public class BlueToothDeviceBroadcastReciever : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			if (intent.Action == BluetoothDevice.ActionAclDisconnected)
			{
				App.Pause();
			}
		}
	}
}
