using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using VocableTrainer.Droid.Actions;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace VocableTrainer.Droid.Notification
{
	internal class Notifications
	{
		static readonly int NOTIFICATION_ID = 1000;
		static readonly string CHANNEL_ID = "location_notification";


		public static PendingIntent GetPendingAction(Context context, string actionValue, int i)
		{
			//This is the intent of PendingIntent
			Intent message = new Intent(ActionReceiver.IntentFilterID);
			// If desired, pass some values to the broadcast receiver.
			message.PutExtra(actionValue, actionValue);

			return PendingIntent.GetBroadcast(context, i, message, PendingIntentFlags.UpdateCurrent);
		}

		public static void CreateNotification(Context context)
		{
			// Build the notification:
			var builder = new NotificationCompat.Builder(context, CHANNEL_ID)
				.SetAutoCancel(false) // Dismiss the notification from the notification area when the user clicks on it
				//   .SetContentIntent(resultPendingIntent) // Start up this activity when the user clicks the intent.
				.SetContentTitle("Vocable Trainer") // Set the title
				.SetCustomContentView(BuildRemoteViews(context))
				.SetSmallIcon(Resource.Drawable.Icon)
				.SetSound(null); // This is the icon to display

			// Finally, publish the notification:
			var notificationManager = NotificationManagerCompat.From(context);
			notificationManager.Notify(NOTIFICATION_ID, builder.Build());
		}

		public static RemoteViews BuildRemoteViews(Context context)
		{
			RemoteViews expandedView = new RemoteViews(Forms.Context.PackageName, Resource.Layout.NotificaitonLayout);
			expandedView.SetOnClickPendingIntent(Resource.Id.flagImg, GetPendingAction(context, ActionReceiver.FlagAction, 1));
			expandedView.SetOnClickPendingIntent(Resource.Id.playImg, GetPendingAction(context, ActionReceiver.PlayAction, 2));
			expandedView.SetOnClickPendingIntent(Resource.Id.nextImg, GetPendingAction(context, ActionReceiver.NextAction, 3));

			return expandedView;
		}

		public static void CreateNotificationChannel(Context context)
		{
			if (Build.VERSION.SdkInt < BuildVersionCodes.O)
			{
				// Notification channels are new in API 26 (and not a part of the
				// support library). There is no need to create a notification 
				// channel on older versions of Android.
				return;
			}

			var channel = new NotificationChannel(CHANNEL_ID, "Local Notifications", NotificationImportance.Default)
			{
				Description = "The count from MainActivity."
			};

			var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
			notificationManager.CreateNotificationChannel(channel);
		}
	}
}