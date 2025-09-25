using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Widget;
using Java.Util;

namespace VocableTrainer
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[]
	{
		BluetoothDevice.ActionAclDisconnected,
	})]
	public class BlueToothDeviceBroadcastReciever : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

			if (device != null)
			{
				if (device.BluetoothClass.DeviceClass == DeviceClass.AudioVideoHeadphones
				    || device.BluetoothClass.DeviceClass == DeviceClass.AudioVideoWearableHeadset)
				{
					Task.Run(App.Pause);
					Toast.MakeText(context, $"Training Paused", ToastLength.Short).Show();
				}
			}

		}
	}
}
