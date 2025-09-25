using Android.Content.PM;
using VocableTrainer.Droid.Notification;

namespace VocableTrainer.Android2
{
	[Activity(Label = "VocableTrainer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
	public class MainActivity : Activity
	{
		private ActionReceiver receiver;
		private MediaButtonReceiver mediaButtonReceiver;
		private BlueToothDeviceBroadcastReciever bluetoothDeviceReceiver;

		private Notifications notifications;

		protected override void OnCreate(Bundle? savedInstanceState)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			notifications = new Notifications();

			base.OnCreate(savedInstanceState);

			notifications.CreateNotificationChannel(this);
			receiver = new ActionReceiver();
			bluetoothDeviceReceiver = new BlueToothDeviceBroadcastReciever();
			mediaButtonReceiver = new MediaButtonReceiver();

			notifications.CreateNotification(this);
			PeriodicService.Start();

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);
		}
	}
}