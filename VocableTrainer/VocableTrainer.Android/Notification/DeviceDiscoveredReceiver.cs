using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace VocableTrainer.Droid.Notification
{
	class DeviceDiscoveredReceiver : BroadcastReceiver
	{
		public static List<string> mDeviceList = new List<string>();
		Activity mainActivity;

		public DeviceDiscoveredReceiver(Activity activity)
		{
			this.mainActivity = activity;
		}

		public override void OnReceive(Context context, Intent intent)
		{
			String action = intent.Action;
			
			if (BluetoothDevice.ActionFound.Equals(action))
			{
				BluetoothDevice device = (BluetoothDevice) intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
				Toast.MakeText(context, "Device connected: " + device.Name, ToastLength.Short).Show();
			}
		}
	}
}