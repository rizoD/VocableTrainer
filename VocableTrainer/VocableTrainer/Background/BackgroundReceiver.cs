using System;
using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.OS;

namespace VocableTrainer.Background
{
	[BroadcastReceiver]
	public class BackgroundReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			PowerManager pm = (PowerManager)context.GetSystemService(Context.PowerService);
			PowerManager.WakeLock wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "BackgroundReceiver");
			wakeLock.Acquire();

			// Run your code here

			wakeLock.Release();
		}
	}
}
