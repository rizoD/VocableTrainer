using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using VocableTrainer.Background;
using Android.Bluetooth;
using Android.Media;
using Android.Support.V4.App;

namespace VocableTrainer.Droid
{
    [Activity(Label = "VocableTrainer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
	    private MediaButtonReceiver mediaButtonReceiver;
	    private BlueToothDeviceBroadcastReciever bluetoothDeviceReceiver;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            PeriodicService.Start();
        }

        protected override void OnResume()
        {
			if (bluetoothDeviceReceiver == null)
			{
				bluetoothDeviceReceiver = new BlueToothDeviceBroadcastReciever();
			}
			RegisterReceiver(bluetoothDeviceReceiver, new IntentFilter(BluetoothDevice.ActionFound));

			if (mediaButtonReceiver == null)
			{
				mediaButtonReceiver = new MediaButtonReceiver();
			}
			RegisterReceiver(mediaButtonReceiver, new IntentFilter(Intent.ActionMediaButton));

			createNotification();

			base.OnResume();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private void createNotification()
        {
	        //Create notification
	        var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

	        //Create an intent to show ui
	        var uiIntent = new Intent(this, typeof(MainActivity));

	        //Use Notification Builder
	        NotificationCompat.Builder builder = new NotificationCompat.Builder(this);

	        //Create the notification
	        //we use the pending intent, passing our ui intent over which will get called
	        //when the notification is tapped.


	        var notification = builder.SetContentIntent(PendingIntent.GetActivity(this, 0, uiIntent, 0))
		        .SetSmallIcon(Resource.Drawable.navigation_empty_icon)
		        .SetTicker("Vocable Trainer")
		        .SetContentTitle("Vocable Trainer")
		        .SetContentText("Test")
		        //.AddAction(new NotificationCompat.Action())
		        //Set the notification sound
		        .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Notification))
		        //Auto cancel will remove the notification once the user touches it
		        .SetAutoCancel(false).Build();

	        //Show the notification
	        notificationManager.Notify(1, notification);
        }
    }
}