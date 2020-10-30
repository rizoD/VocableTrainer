using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
			var startTimeSpan = TimeSpan.Zero;
			var periodTimeSpan = TimeSpan.FromSeconds(1);

			var timer = new System.Threading.Timer((e) =>
			{
				try
				{
					Task.Run(() =>
					{
						App.DoTraining();
					});
				}
				catch (Exception ex)
				{
					ex.ToString();
				}
			}, null, startTimeSpan, periodTimeSpan);


			return StartCommandResult.Sticky;
		}

	}
}
