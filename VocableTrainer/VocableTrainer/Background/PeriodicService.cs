using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using VocableTrainer.Data;

namespace VocableTrainer.Background
{
	[Service]
	public class PeriodicService : Service
	{
		public static void Start()
		{
			var intent = new Intent(Android.App.Application.Context,
				typeof(PeriodicService));

			Android.App.Application.Context.StartService(intent);

		}

		public override IBinder OnBind(Intent intent)
		{
			return null;
		}

		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			// From shared code or in your PCL

			//CreateNotificationChannel();
			//string messageBody = "service starting";

			//var notification = new Notification.Builder(this, "10111")
			//.SetContentTitle("Foreground")
			//.SetContentText(messageBody)
			//.SetSmallIcon(Resource.Drawable.main)
			//.SetOngoing(true)
			//.Build();
			//StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);

			//=======you can do you always running work here.=====
			var startTimeSpan = TimeSpan.Zero;
			var periodTimeSpan = TimeSpan.FromSeconds(1);

			var timer = new System.Threading.Timer((e) =>
			{
				App.DoTraining();
			}, null, startTimeSpan, periodTimeSpan);


			return StartCommandResult.Sticky;
		}


		//void CreateNotificationChannel()
		//{
		//	if (Build.VERSION.SdkInt < BuildVersionCodes.O)
		//	{
		//		// Notification channels are new in API 26 (and not a part of the
		//		// support library). There is no need to create a notification
		//		// channel on older versions of Android.
		//		return;
		//	}

		//	var channelName = Resources.GetString(Resource.String.channel_name);
		//	var channelDescription = GetString(Resource.String.channel_description);
		//	var channel = new NotificationChannel("10111", channelName, NotificationImportance.Default)
		//	{
		//		Description = channelDescription
		//	};

		//	var notificationManager = (NotificationManager)GetSystemService(NotificationService);
		//	notificationManager.CreateNotificationChannel(channel);
		//}

	}
}
