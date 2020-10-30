using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VocableTrainer.Data;

namespace VocableTrainer.Droid.Actions
{
	[BroadcastReceiver(Enabled = true)]
	[IntentFilter(new[] { ActionReceiver.IntentFilterID })]
	public class ActionReceiver : BroadcastReceiver
	{
		public const string IntentFilterID = "{02FE1E93-D1C7-4D66-9D4A-8ACBFB220D03}";
		public const string FlagAction = "Flag";
		public const string PlayAction = "Play";
		public const string NextAction = "Next";

		public override void OnReceive(Context context, Intent intent)
		{
			if (intent.HasExtra(FlagAction))
			{
				App.TrainingFlag();
			}
			else if (intent.HasExtra(PlayAction))
			{
				App.TogglePlay();
			} else if (intent.HasExtra(NextAction))
			{
				App.Next();
			}

			//This is used to close the notification tray
			Intent it = new Intent(Intent.ActionCloseSystemDialogs);
			context.SendBroadcast(it);
		}
	}
}