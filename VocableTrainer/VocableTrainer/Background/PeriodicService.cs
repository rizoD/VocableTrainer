using System;
using System.Threading.Tasks;

namespace VocableTrainer.Background
{
	[Service]
	public class PeriodicService : Service
	{
		public static void Start()
		{
			var intent = new Intent(Application.Context, typeof(PeriodicService));
			Application.Context.StartService(intent);
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
