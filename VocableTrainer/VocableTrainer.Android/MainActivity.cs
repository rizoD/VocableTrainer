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
using VocableTrainer.Droid.Actions;
using VocableTrainer.Droid.Notification;
using Xamarin.Forms;

namespace VocableTrainer.Droid
{
    [Activity(Label = "VocableTrainer", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
	    private ActionReceiver receiver;
        private MediaButtonReceiver mediaButtonReceiver;
	    private BlueToothDeviceBroadcastReciever bluetoothDeviceReceiver;
	    //DeviceDiscoveredReceiver btreceiver;
	    //BluetoothAdapter btAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            Notifications.CreateNotificationChannel(this);
            receiver = new ActionReceiver();
            bluetoothDeviceReceiver = new BlueToothDeviceBroadcastReciever();
            mediaButtonReceiver = new MediaButtonReceiver();

            Notifications.CreateNotification(this);
            PeriodicService.Start();

            //btreceiver = new DeviceDiscoveredReceiver(this);
            //IntentFilter filter = new IntentFilter(BluetoothDevice.ActionFound);
            //RegisterReceiver(receiver, filter);
            //btAdapter = BluetoothAdapter.DefaultAdapter;
        }

        protected override void OnDestroy()
        {
	        UnregisterReceiver(receiver);
	        UnregisterReceiver(bluetoothDeviceReceiver);
	        base.OnDestroy();
        }

        protected override void OnResume()
        {
	        var am = (AudioManager)this.GetSystemService(AudioService);
	        var componentName = new ComponentName(PackageName, mediaButtonReceiver.ComponentName);
	        am.RegisterMediaButtonEventReceiver(componentName);
	        //RegisterReceiver(mediaButtonReceiver, new IntentFilter(Intent.ActionMediaButton));
            RegisterReceiver(receiver, new IntentFilter(ActionReceiver.IntentFilterID));
			RegisterReceiver(bluetoothDeviceReceiver, new IntentFilter(BluetoothDevice.ActionAclDisconnected));
            
			base.OnResume();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}